using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using VeracodeService.Repositories;

namespace WebhookLogic
{
    public interface IStatusService
    {
        bool IsVeracodeConnectionOk();
        bool IsDatabaseConnectionOk();
    }

    public class StatusService : IStatusService
    {
        private ApplicationDbContext _context;
        private IVeracodeRepository _veracode;

        public StatusService(IVeracodeRepository veracode,
            ApplicationDbContext context)
        {
            _context = context;
            _veracode = veracode;
        }

        public bool IsDatabaseConnectionOk()
        {
            try
            {
                return !_context.Database.EnsureCreated();
            } catch (Exception) {
                return false;
            }
        }

        public bool IsVeracodeConnectionOk()
        {
            try
            {
                _ = _veracode.GetAllApps();
                return true;
            } catch
            {
                return false;
            }
        }
    }
}
