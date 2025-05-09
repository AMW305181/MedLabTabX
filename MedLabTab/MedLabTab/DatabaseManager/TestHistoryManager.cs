using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class TestHistoryManager
    {
        public bool AddTestHistory(MedLabContext db, TestHistory newTest)
        {
            try
            {
                db.TestHistories.Add(newTest);
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public bool RemoveTestHistory(MedLabContext db,int visitId)
        {
            try
            {
                db.TestHistories.RemoveRange(db.TestHistories.Where(t => t.VisitId == visitId));
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public List<TestHistory> GetTestsInVisit(MedLabContext db, int visitId)
        {
            try
            {
                List<TestHistory> TestsInVisit = db.TestHistories.Where(t => t.VisitId == visitId).ToList();
                if (TestsInVisit != null) { return TestsInVisit; }
                return null;
            }
            catch { return null; }
        }
        public List<TestHistory> GetCompletedTests(MedLabContext db)
        {
                return db.TestHistories
                    .Include(th => th.Test)
                    .Include(th => th.Visit)
                        .ThenInclude(v => v.TimeSlot)
                            .ThenInclude(ts => ts.Nurse)
                    .Include(th => th.Patient)
                    .Include(th => th.Analyst)
                    .Where(th =>  th.Status==5)
                    .OrderByDescending(th => th.Visit.TimeSlot.Date)
                    .ThenByDescending(th => th.Visit.TimeSlot.Time)
                    .ToList();

        }
        public int GetNurseIdFromTestHistory(MedLabContext db, TestHistory test)
        {
            return db.TestHistories.Where(th => th.id == test.id)
           .Select(th => th.Visit.TimeSlot.NurseId)
           .FirstOrDefault();
        }
    }
}
