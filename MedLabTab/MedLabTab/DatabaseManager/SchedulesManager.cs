using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;

namespace MedLabTab.DatabaseManager
{
    internal class SchedulesManager
    {
        public Schedule GetSchedule(MedLabContext db,int Id)
        {
            try
            {
                var schedule = db.Schedules.Where(s => s.id == Id).FirstOrDefault();
                if (schedule != null) { return schedule; }
                return null;
            }
            catch { return null; }
        }
        public List<Schedule> GetAllDates(MedLabContext db)
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
