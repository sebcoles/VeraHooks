using Microsoft.Extensions.Options;
using System.Threading;
using WebhookLogic.Configuration;

namespace WebhookLogic
{
    public interface ITimerService
    {
        void GenerateTimer();
        void GenerateCallHomeTimer();
    }
    public class TimerService : ITimerService
    {
        private TimerConfiguration _config;
        private IWebhookHandler _webhookHandler;
        private Timer _stateTimer;

        public TimerService(
            IOptions<TimerConfiguration> config,
            IWebhookHandler webhookHandler)
        {
            _webhookHandler = webhookHandler;
            _config = config.Value;
        }
        public void GenerateTimer()
        {
            var autoEvent = new AutoResetEvent(false);
            _stateTimer = new Timer(TimerCallback, autoEvent, 
                _config.PollFrequency, Timeout.Infinite);
            autoEvent.WaitOne();
        }

        void TimerCallback(object state)
        {
            _webhookHandler.CheckStatus(state);
            _stateTimer.Change(_config.PollFrequency, Timeout.Infinite);
        }

        public void GenerateCallHomeTimer()
        {
            var autoEvent = new AutoResetEvent(false);
            var stateTimer = new Timer(_webhookHandler.CallHome,
                                       autoEvent, 0, 10000);
            autoEvent.WaitOne();
        }
    }
}
