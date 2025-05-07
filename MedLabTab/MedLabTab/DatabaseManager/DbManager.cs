using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
using MedLabTab.Views.OtherViews;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace MedLabTab.DatabaseManager
{
    public static class DbManager
    {
        private static MedLabContext db = new MedLabContext();
        private static UsersManager usersManager = new UsersManager();
        private static TestsManager testsManager = new TestsManager();
        private static SchedulesManager schedulesManager = new SchedulesManager();
        private static VisitsManager visitsManager = new VisitsManager();
        private static CategoriesManager categoriesManager = new CategoriesManager();
        private static TestHistoryManager testHistoryManager = new TestHistoryManager();
        private static ReportsManager reportsManager = new ReportsManager();
       
        public static bool LogInUser(string username, ref SignedInUser user) {return usersManager.LogInUser(db, username, ref user); }
        public static bool CheckUser(string username, string password) {return usersManager.CheckUser(db,username,password); }
        public static bool IsPESELTaken(string PESEL){return usersManager.IsPESELTaken(db, PESEL);}
        public static bool IsLoginTaken(string login){ return usersManager.IsLoginTaken(db, login);}
        public static bool IsLoginTakenByAnotherUser(string login, int userId){return usersManager.IsLoginTakenByAnotherUser(db, login, userId);}
        public static bool AddUser(User user){ return usersManager.AddUser(db, user);}
        public static List<Test> GetActiveTests() {return testsManager.GetActiveTests(db); }
        public static List<User> GetActivePatients() {return usersManager.GetActivePatients(db);}
        public static List<Test> GetAllTests() {return testsManager.GetAllTests(db); }
        public static Test GetTest(int Id){return testsManager.GetTest(db, Id);}
        public static bool EditUserCommon(string login, string password, string phoneNumber, int userId){ return usersManager.EditUserCommon(db, login, password, phoneNumber, userId);}
        public static User GetUser(string PESEL) {return usersManager.GetUser(db, PESEL); }
        public static User GetUserById(int Id) { return usersManager.GetUserById(db, Id);}
        public static Schedule GetSchedule(int Id){return schedulesManager.GetSchedule(db, Id);}
        public static bool IsTestNameTaken(string testName)
        {
            try
            {
                var test = db.Tests.Where(t => t.TestName == testName).FirstOrDefault();
                if (test != null) { return true; }
                return false;
            }
            catch { return false; }
        }
        public static bool AddTest(Test test) { return testsManager.AddTest(db, test); }
        public static bool EditTest(Test test, Test newData){return testsManager.EditTest(db, test, newData); }
        public static bool DeactivateVisit(Visit visit){ return visitsManager.DeactivateVisit(db, visit);}
        public static bool ChangeTestStatus(Test test) { return testsManager.ChangeTestStatus(db, test); }
        public static bool ChangeTestStatus(int testId)
        {
            try
            {
                var test=db.Tests.Where(t=>t.id==testId).FirstOrDefault();
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
        public static List<CategoryDictionary> GetCategories() {return categoriesManager.GetCategories(db);}
        public static Dictionary<int, string> GetCategoriesDictionary() { return categoriesManager.GetCategoriesDictionary(db); }
        public static List<User> LoadUsers()  { return usersManager.LoadUsers(db);}
        public static bool ChangeUserStatus(int userId)
        {
            try
            {
                var user = db.Users.Where(u => u.id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.IsActive = !user.IsActive;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public static bool EditUserAdmin(int userId, string login, int userType, string password, string name, string surname, string PESEL, string? phone = null)
        {
            try
            {
                var user = db.Users.Where(u => u.id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Login = login;
                    user.UserType = userType;
                    user.Password = password;
                    user.Name = name;
                    user.Surname = surname;
                    user.PESEL = PESEL;
                    user.PhoneNumber = phone;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public static bool EditUserAdmin(User user, string login, int userType, string password, string name, string surname, string PESEL, string? phone = null)
        {
            try
            {
                if (user != null)
                {
                    user.Login = login;
                    user.UserType = userType;
                    user.Password = password;
                    user.Name = name;
                    user.Surname = surname;
                    user.PESEL = PESEL;
                    user.PhoneNumber = phone;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }    
        public static bool EditUserAdmin(User user, User newData)
        {
            try
            {
                if (user != null&&newData!=null)
                {
                    user.Login = newData.Login;
                    user.UserType = newData.UserType;
                    user.Password = newData.Password;
                    user.Name = newData.Name;
                    user.Surname = newData.Surname;
                    user.PESEL = newData.PESEL;
                    user.PhoneNumber = newData.PhoneNumber;
                    user.IsActive = newData.IsActive;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public static List<Visit> GetMyVisits(int userId) {  return visitsManager.GetMyVisits(db, userId);}

        //public static bool DeactivateVisit(Visit visit)
        //{
        //    try
        //    {
        //        var visitToUpdate = db.Visits.FirstOrDefault(v => v.id == visit.id);
        //        if (visitToUpdate == null) return false;

        //        visitToUpdate.IsActive = false;
        //        db.SaveChanges();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public static List<Schedule> GetAllDates() { return schedulesManager.GetAllDates(db); }
        public static List<Visit> GetAllVisits(){return visitsManager.GetAllVisits(db); }
        public static bool EditVisit(Visit oldVisit, Visit newVisit) { return visitsManager.EditVisit(db, oldVisit, newVisit); }
        public static bool AddTestHistory(TestHistory newTest) { return testHistoryManager.AddTestHistory(db, newTest); }
        public static bool RemoveTestHistory(int visitId) { return testHistoryManager.RemoveTestHistory(db, visitId); }
        public static List<TestHistory> GetTestsInVisit(int visitId) { return testHistoryManager.GetTestsInVisit(db, visitId); }
        public static List<TestHistory> GetCompletedTests() { return testHistoryManager.GetCompletedTests(db);}

        //to do poprawki - lista wizyt dla danej pielegniarki, aktywne, status = "Sample to be collected"
        //done uwu
        public static List<Visit> GetNurseVisits(int nurseId)
        {
            return db.Visits
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories
                    .Where(th => th.Status == 2))
                    .ThenInclude(th => th.Test)  
                .Include(v => v.Patient)
                .Where(v => v.IsActive == true && v.TimeSlot != null && v.TimeSlot.NurseId == nurseId)
                .AsNoTracking()
                .ToList();
        }

        //to do poprawki - lista wizyt dla analizy - status = "Sample collected" lub "Analysis pending"
        public static List<TestHistory> GetAnalystTests() //int analystId
        {
            return db.TestHistories
                .Include(th => th.Test)
                    .ThenInclude(t => t.CategoryNavigation)
                .Include(th => th.Patient)
                .Include(th => th.Visit)
                    .ThenInclude(v => v.TimeSlot)
                .Where(th=>th.Status==3 ||  th.Status==4) //.Where(th=>th.Status==3 || (th.Status==4 && th.AnalystId==analystId))
                .ToList();
        }


        public static bool EditTestHistory(TestHistory odlTest, TestHistory newTest)
        {
            try
            {
                odlTest.id = newTest.id;
                odlTest.VisitId = newTest.VisitId;
                odlTest.TestId = newTest.TestId;
                odlTest.PatientId = newTest.PatientId;
                odlTest.Status = newTest.Status;
                odlTest.AnalystId = newTest.AnalystId;
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public static string GetHashedPassword(string username)  {  return usersManager.GetHashedPassword(db, username);}
        public static bool AddReport(Report report){ return reportsManager.AddReport(db, report); }
        public static bool EditReport(Report report, Report newReport) { return reportsManager.EditReport(db, report, newReport); }
        public static int GetNurseIdFromTestHistory(TestHistory test) {return testHistoryManager.GetNurseIdFromTestHistory(db, test);}

    }
}

