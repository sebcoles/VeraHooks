using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;

namespace DataAccess
{

    public class WebhookRepository : GenericRepository<MitigationWebhook>, IGenericRepository<MitigationWebhook>
    {
        private string _demand;
        public WebhookRepository(ApplicationDbContext context) : base(context) { }
        public WebhookRepository(ApplicationDbContext context, IOptions<ContextConfiguration> config) : base(context) {
            _demand = config.Value.Name;
        }
                
        public new IQueryable<MitigationWebhook> GetAll()
        {
            return _dbContext
                .Set<MitigationWebhook>()
                .Where(x => x.Demand == _demand || _demand == "*")
                .Include(x => x.Apps)
                .Include(x => x.PropertyConditions)
                .AsNoTracking();
        }
    }
}
