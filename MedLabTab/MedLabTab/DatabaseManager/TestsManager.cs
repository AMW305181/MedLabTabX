using MedLabTab.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLabTab.DatabaseManager
{
    internal class TestsManager
    {
        public List<Test> GetActiveTests(MedLabContext db)
        {
            try
            {
                List<Test> ActiveTests = db.Tests.Where(t => t.IsActive == true).ToList();
                return ActiveTests;
            }
            catch { return null; }
        }
        public List<Test> GetAllTests(MedLabContext db)
        {
            try
            {
                List<Test> AllTests = db.Tests.ToList();
                return AllTests;
            }
            catch { return null; }
        }
        public Test GetTest(MedLabContext db,int Id)
        {
            try
            {
                var test = db.Tests.Where(t => t.id == Id).FirstOrDefault();
                if (test != null) { return test; }
                return null;
            }
            catch { return null; }
        }
        public bool AddTest(MedLabContext db,Test test)
        {
            try
            {
                db.Tests.Add(test);
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public  bool EditTest(MedLabContext db, Test test, Test newData)
        {
            try
            {
                test.TestName = newData.TestName;
                test.Description = newData.Description;
                test.Price = newData.Price;
                test.Category = newData.Category;
                test.IsActive = newData.IsActive;
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public bool ChangeTestStatus(MedLabContext db,Test test)
        {
            try
            {
                if (test != null)
                {
                    test.IsActive = !test.IsActive;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
    }
}
