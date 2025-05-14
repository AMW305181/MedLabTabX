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
        public bool RemoveTestHistory(MedLabContext db, int visitId)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
            {
                try
                {
                    db.TestHistories.RemoveRange(db.TestHistories.Where(t => t.VisitId == visitId));
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }
        public List<TestHistory> GetTestsInVisit(MedLabContext db, int visitId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<TestHistory> TestsInVisit = db.TestHistories.Where(t => t.VisitId == visitId).ToList();
                    scope.Complete();
                    if (TestsInVisit != null) { return TestsInVisit; }
                    return null;
                }
                catch { return null; }
            }
        }
        public List<TestHistory> GetCompletedTests(MedLabContext db, int? patientId = null)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
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

                    List < TestHistory > list= query
                        .OrderByDescending(th => th.Visit.TimeSlot.Date)
                        .ThenByDescending(th => th.Visit.TimeSlot.Time)
                        .ToList();
                     scope.Complete();
                    return list;
                }
                catch
                {
                    return new List<TestHistory>();
                }
            }
        }
        public int GetNurseIdFromTestHistory(MedLabContext db, TestHistory test)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                int id= db.TestHistories.Where(th => th.id == test.id)
           .Select(th => th.Visit.TimeSlot.NurseId)
           .FirstOrDefault();
                scope.Complete();
                return id;
            }
        }
        public List<TestHistory> GetAnalystTests(MedLabContext db, int analystId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<TestHistory> analystTests = db.TestHistories
                    .Include(th => th.Test)
                        .ThenInclude(t => t.CategoryNavigation)
                    .Include(th => th.Patient)
                    .Include(th => th.Visit)
                        .ThenInclude(v => v.TimeSlot)
                    .Where(th => th.Status == 3 || (th.Status == 4 && th.AnalystId == analystId))
                    .ToList();
                    scope.Complete();
                    return analystTests;
                }
                catch { return null; }
            }
        }
        public bool EditTestHistory(MedLabContext db, TestHistory oldTest, TestHistory newTest)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
            {
                try
                {
                    //oldTest.id = newTest.id;
                    oldTest.VisitId = newTest.VisitId;
                    oldTest.TestId = newTest.TestId;
                    oldTest.PatientId = newTest.PatientId;
                    oldTest.Status = newTest.Status;
                    oldTest.AnalystId = newTest.AnalystId;
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }
        public List<TestHistory> GetAllTestHistories(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<TestHistory> AllTests = db.TestHistories
                        .Include(t => t.Test)
                            .ThenInclude(te => te.CategoryNavigation)
                        .Include(t => t.Patient)
                        .Include(t => t.Visit)
                            .ThenInclude(v => v.TimeSlot)
                        .Include(t => t.StatusNavigation)
                        .ToList();
                    scope.Complete();
                    return AllTests;
                }
                catch { return null; }
            }
        }
        public List<TestHistory> GetPatientResults(MedLabContext db, int patientId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<TestHistory> patientResults =
                     db.TestHistories
                        .Include(th => th.Test)
                        .Include(th => th.Visit)
                            .ThenInclude(v => v.TimeSlot)
                        .Include(th => th.Patient)
                        .Include(th => th.StatusNavigation) // Dodane załadowanie słownika statusów
                        .Include(th => th.Analyst) // Dodane załadowanie analityka
                        .Include(th => th.Reports) // Dodane załadowanie raportów
                        .Where(th => th.PatientId == patientId)
                        .OrderByDescending(th => th.Visit.TimeSlot.Date)
                        .ThenByDescending(th => th.Visit.TimeSlot.Time)
                        .ToList();
                    scope.Complete();
                    return patientResults;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
