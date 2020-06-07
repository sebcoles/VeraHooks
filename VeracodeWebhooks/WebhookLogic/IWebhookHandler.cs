using DataAccess;
using DataAccess.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VeracodeService.Models;
using VeracodeService.Repositories;
using WebhookLogic.Configuration;
using WebhookLogic.Models;

namespace WebhookLogic
{
    public interface IWebhookHandler
    {
        void CheckStatus(object stateInfo);
        void CallHome(object state);
    }

    public class WebhookHandler : IWebhookHandler
    {
        private IHttpPostService _httpPostService;
        private IGenericRepository<WebhookLog> _logRepository;
        private IGenericRepository<MitigationWebhook> _webhookRepository;
        private IVeracodeRepository _veracodeRepository;
        private IStatusService _statusService;
        private string _name;
        private string _callhomeaAddress;

        public WebhookHandler(
            IHttpPostService httpPostService,
            IGenericRepository<WebhookLog> logRepository,
            IGenericRepository<MitigationWebhook> webhookRepository,
            IVeracodeRepository veracodeRepository,
            IOptions<ContextConfiguration> contextOptions,
            IOptions<AgentConfiguration> agentOptions,
            IStatusService statusService
            )
        {
            _name = contextOptions.Value.Name;
            _callhomeaAddress = agentOptions.Value.CallHome;
            _httpPostService = httpPostService;
            _logRepository = logRepository;
            _webhookRepository = webhookRepository;
            _veracodeRepository = veracodeRepository;
            _statusService = statusService;
        }

        public async void CallHome(object state)
        {
            var message = new
            {
                Name = _name,
                IpAddress = LocalIPAddress().ToString(),
                VeracodeOk = _statusService.IsVeracodeConnectionOk(),
                DatabaseOk = _statusService.IsDatabaseConnectionOk()
            };

            var json = JsonConvert.SerializeObject(message);
            var response = await _httpPostService.SendMessage(json, _callhomeaAddress);
        }

        public void CheckStatus(object stateInfo)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Starting webhook check task");

            var webhooksToFire = _webhookRepository
                .GetAll()
                .Where(webhook =>
                    DateTime.Now > webhook.LastFired.AddSeconds(webhook.SecondsBetweenCheck)
                    || webhook.LastFired < webhook.Created).ToList();

            if (!webhooksToFire.Any())
            {
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : No webhooks ready to fire....Skipping");
                return;
            }

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {webhooksToFire.Count} webhooks are ready to fire if conditions have been met");
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Checking webhooks for new flaws");
            webhooksToFire.ForEach(x =>
            {
                GetAnyNewFlawIds(ref x);
            });

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Finding webhooks where mitigations meet the webhooks conditions");
            var webhookAndActions = webhooksToFire.SelectMany(webhook => webhook
                .Apps.Where(x => x.FlawString != null && x.FlawString.Length > 0)
                .SelectMany(app => _veracodeRepository.GetAllMitigationsForBuildAndFlaws(
                        $"{app.LastBuild}",
                        app.FlawString.Split(",", StringSplitOptions.None))
                    .Issue.SelectMany(issue => issue
                        .MitigationActions.Where(mitigationAction => webhook
                            .PropertyConditions.All(propertyCondition => propertyCondition.HaveIBeenMet(mitigationAction))
                            && mitigationAction.DateObject > webhook.LastFired)
                        .Select(action => new { webhook, app, action })))).ToList();

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {webhookAndActions.Count} are ready to fire and have met the webhook conditions!");
            var fireResults = webhookAndActions.Select(x => FireWebhook(x.webhook, x.action, x.app).Result).ToArray();

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Updating Database...");
            foreach (var item in fireResults)
                UpdateWebhook(item);

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Ending webhook check task");
        }

        public async Task<Tuple<string, string, int, MitigationWebhook, MitigationAction, App>> FireWebhook(MitigationWebhook webhook, MitigationAction mitigation, App app)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Preparing to fire {webhook.Name}");
            var webhookMessage = SlackPayload(mitigation, app);
            var json = JsonConvert.SerializeObject(webhookMessage);

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {webhook.Name} Fire!");
            var response = await _httpPostService.SendMessage(json, webhook.SendAddress);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {webhook.Name} webhook recieved HTTP Status {response.StatusCode}");
            return new Tuple<string, string, int, MitigationWebhook, MitigationAction, App>(json, content, (int)response.StatusCode, webhook, mitigation, app);
        }

        public void UpdateWebhook(Tuple<string, string, int, MitigationWebhook, MitigationAction, App> message)
        {
            _ = _logRepository.Create(new WebhookLog
            {
                WebHookName = message.Item4.Name,
                WebHookSendAddress = message.Item4.SendAddress,
                AppName = message.Item6.AppName,
                TimeFired = DateTime.Now,
                MessageSent = message.Item1,
                Response = message.Item2,
                StatusReturned = message.Item3
            });
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {message.Item4.Name} success has been logged");
            message.Item4.LastFired = DateTime.Now;
            message.Item4.TimesFired++;
            _ = _webhookRepository.Update(message.Item4.Id, message.Item4);
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : {message.Item4.Name} updated in database");
        }

        public void GetAnyNewFlawIds(ref MitigationWebhook webhook)
        {
            foreach (var app in webhook.Apps)
            {
                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Checking {app.AppName} for new flaws");

                var latestBuild = _veracodeRepository.GetAllBuildsForApp($"{app.AppId}")
                    .OrderBy(x => x.Build_id).FirstOrDefault();

                if (latestBuild == null || latestBuild.Build_id == app.LastBuild)
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : There are no new builds for {app.AppName} so no new flaws...Skipping");
                    continue;
                }

                var currentFlaws = new List<string>();
                if (app.FlawString != null)
                    currentFlaws.AddRange(app.FlawString.Split(",").ToList());

                var newFlaws = _veracodeRepository.GetFlawIds(latestBuild.Build_id)
                        .Where(f => !currentFlaws.Any(f2 => f2 == f)).ToList();

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Adding {newFlaws.Count()} new flaws for {app.AppName}");
                currentFlaws.AddRange(newFlaws);
                app.FlawString = String.Join(",", currentFlaws);

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : Updating last build for {app.AppName} to {latestBuild.Build_id}");
                app.LastBuild = latestBuild.Build_id;
            }
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        private dynamic SlackPayload(MitigationAction mitigation, App app)
        {
            return new
            {
                blocks = new dynamic[]{
                new {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = $"You have a new mitigation request:\n*<fakeLink.com|{app.AppName}>*"
                    }
                },
                new {
                    type = "section",
                    fields = new[]
                    {
                        new {
                            type = "mrkdwn",
                            text = $"*Action:*\n{mitigation.Action}"
                        },
                        new {
                            type = "mrkdwn",
                            text = $"*Comment:*\n{mitigation.Comment}"
                        },
                        new {
                            type = "mrkdwn",
                            text = $"*Submitted:*\n{mitigation.Date}*"
                        }
                    }
                },
                new {
                    type = "actions",
                    elements = new[]{
                        new {
                            type = "button",
                            text = new {
                                type = "plain_text",
                                emoji = true,
                                text = "Approve"
                            },
                            style = "primary",
                            value = "click_me_123"
                        },
                        new {
                            type = "button",
                            text = new {
                                type = "plain_text",
                                emoji = true,
                                text = "Reject"
                            },
                            style = "danger",
                            value = "click_me_123"
                        }
                    }
                }
            }};
        }
    }
}
