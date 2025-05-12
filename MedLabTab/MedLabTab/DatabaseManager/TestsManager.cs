using MedLabTab.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MedLabTab.DatabaseManager
{
    internal class TestsManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public List<Test> GetActiveTests(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Test> ActiveTests = db.Tests.Where(t => t.IsActive == true).ToList();
                    scope.Complete();
                    return ActiveTests;
                }
                catch { return null; }
            }
        }
        public List<Test> GetAllTests(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Test> AllTests = db.Tests.ToList();
                    scope.Complete();
                    return AllTests;
                }
                catch { return null; }
            }
        }
        public Test GetTest(MedLabContext db,int Id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var test = db.Tests.Where(t => t.id == Id).FirstOrDefault();
                    scope.Complete();
                    if (test != null) { return test; }
                    return null;
                }
                catch { return null; }
            }
        }
        public bool AddTest(MedLabContext db,Test test)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    bool testExists = db.Tests.Any(t => t.TestName==test.TestName);
                    if (!testExists)
                    {
                        db.Tests.Add(test);
                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    scope.Complete();
                    return false;
                }
                catch { return false; }
            }
        }
        public  bool EditTest(MedLabContext db, Test test, Test newData)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
            {
                try
                {
                    test.TestName = newData.TestName;
                    test.Description = newData.Description;
                    test.Price = newData.Price;
                    test.Category = newData.Category;
                    test.IsActive = newData.IsActive;
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }
        public bool ChangeTestStatus(MedLabContext db,Test test)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
            {
                try
                {
                    if (test != null)
                    {
                        test.IsActive = !test.IsActive;
                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    scope.Complete();
                    return false;
                }
                catch { return false; }
            }
        }
    }
}
