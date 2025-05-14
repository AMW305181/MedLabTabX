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

        public static List<User> GetActiveNurses() { return usersManager.GetActiveNurses(db); }
        public static List<Test> GetAllTests() {return testsManager.GetAllTests(db); }
        public static Test GetTest(int Id){return testsManager.GetTest(db, Id);}
        public static bool EditUserCommon(string login, string password, string phoneNumber, int userId){ return usersManager.EditUserCommon(db, login, password, phoneNumber, userId);}
        public static User GetUser(string PESEL) {return usersManager.GetUser(db, PESEL); }
        public static User GetUserById(int Id) { return usersManager.GetUserById(db, Id);}
        public static Schedule GetSchedule(int Id){return schedulesManager.GetSchedule(db, Id);}
        public static bool IsTestNameTaken(string testName)  {return testsManager.IsTestNameTaken(db, testName); }
        public static bool AddTest(Test test) { return testsManager.AddTest(db, test); }
        public static bool EditTest(Test test, Test newData){return testsManager.EditTest(db, test, newData); }
        public static bool DeactivateVisit(Visit visit){ return visitsManager.DeactivateVisit(db, visit);}
        public static bool ChangeTestStatus(Test test) { return testsManager.ChangeTestStatus(db, test); }
        public static List<CategoryDictionary> GetCategories() {return categoriesManager.GetCategories(db);}
        public static Dictionary<int, string> GetCategoriesDictionary() { return categoriesManager.GetCategoriesDictionary(db); }
        public static List<User> LoadUsers()  { return usersManager.LoadUsers(db);}
        public static bool ChangeUserStatus(int userId) { return usersManager.ChangeUserStatus(db, userId); }
 
        public static List<Visit> GetMyVisits(int userId) {  return visitsManager.GetMyVisits(db, userId);}
        public static List<Schedule> GetAllDates() { return schedulesManager.GetAllDates(db); }
        public static List<Schedule> GetAvailableSlotsForDate(DateOnly date) { return schedulesManager.GetAvailableSlotsForDate(db, date); }
        public static List<Schedule> GetAllSlotsForDate(DateOnly date) { return schedulesManager.GetAllSlotsForDate(db, date); }
        public static bool AddScheduleSlots(List<Schedule> slots) { return schedulesManager.AddScheduleSlots(db, slots); }
        public static List<Visit> GetAllVisits(){return visitsManager.GetAllVisits(db); }
        public static bool EditVisit(Visit oldVisit, Visit newVisit) { return visitsManager.EditVisit(db, oldVisit, newVisit); }
        public static bool AddTestHistory(TestHistory newTest) { return testHistoryManager.AddTestHistory(db, newTest); }
        public static bool RemoveTestHistory(int visitId) { return testHistoryManager.RemoveTestHistory(db, visitId); }
        public static List<TestHistory> GetTestsInVisit(int visitId) { return testHistoryManager.GetTestsInVisit(db, visitId); }
        public static List<TestHistory> GetCompletedTests(MedLabContext db, int? patientId) { return testHistoryManager.GetCompletedTests(db);}
        public static List<TestHistory> GetCompletedTests() { return testHistoryManager.GetCompletedTests(db); }
        public static List<Visit> GetNurseVisits(int nurseId) {   return visitsManager.GetNurseVisits(db, nurseId); }
        public static List<TestHistory> GetAnalystTests(int analystId)    { return testHistoryManager.GetAnalystTests(db, analystId); }
        public static bool EditTestHistory(TestHistory oldTest, TestHistory newTest)    { return testHistoryManager.EditTestHistory(db, oldTest, newTest);  }
        public static List<TestHistory> GetAllTestHistories()    {  return testHistoryManager.GetAllTestHistories(db);   }
        public static Report GetReport(int Id)   { return reportsManager.GetReport(db, Id); }
        public static List<TestHistory> GetPatientResults(int patientId)    {   return testHistoryManager.GetPatientResults(db, patientId); }
        public static string GetHashedPassword(string username)  {  return usersManager.GetHashedPassword(db, username);}
        public static bool AddReport(Report report){ return reportsManager.AddReport(db, report); }
        public static bool EditReport(Report report, Report newReport) { return reportsManager.EditReport(db, report, newReport); }
        public static int GetNurseIdFromTestHistory(TestHistory test) {return testHistoryManager.GetNurseIdFromTestHistory(db, test);}

       
    }
}

