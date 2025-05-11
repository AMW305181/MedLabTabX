using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MedLabTab.DatabaseModels;

namespace MedLabTab.DatabaseManager
{
    internal class SchedulesManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public Schedule GetSchedule(MedLabContext db,int Id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var schedule = db.Schedules.Where(s => s.id == Id).FirstOrDefault();
                    if (schedule != null) { return schedule; }
                    return null;
                }
                catch { return null; }
            }
        }
        public List<Schedule> GetAllDates(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Schedule> AvailableDates = db.Schedules.ToList();
                    return AvailableDates;
                }
                catch { return null; }
            }
        }
    }
}
