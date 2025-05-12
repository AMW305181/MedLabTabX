using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class TestHistoryManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public bool AddTestHistory(MedLabContext db, TestHistory newTest)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    db.TestHistories.Add(newTest);
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
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
        public List<TestHistory> GetCompletedTests(MedLabContext db, int? patientId = null)
        {
           try
            {
                var query = db.TestHistories
                    .Include(th => th.Test)
                    .Include(th => th.Visit)
                        .ThenInclude(v => v.TimeSlot)
                            .ThenInclude(ts => ts.Nurse)
                    .Include(th => th.Patient)
                    .Include(th => th.Analyst)
                    .Include(th => th.StatusNavigation)
                    .Include(th => th.Test.CategoryNavigation)
                    .Where(th => th.Status == 5);

                if (patientId.HasValue)
                {
                    query = query.Where(th => th.PatientId == patientId.Value);
                }

                return query
                    .OrderByDescending(th => th.Visit.TimeSlot.Date)
                    .ThenByDescending(th => th.Visit.TimeSlot.Time)
                    .ToList();
            }
            catch
            {
                return null;
            }
        }
        public int GetNurseIdFromTestHistory(MedLabContext db, TestHistory test)
        {
            return db.TestHistories.Where(th => th.id == test.id)
           .Select(th => th.Visit.TimeSlot.NurseId)
           .FirstOrDefault();
        }
    }
}
