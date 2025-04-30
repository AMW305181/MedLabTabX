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

namespace MedLabTab.DatabaseManager
{
    public static class DbManager
    {
        private static MedLabContext db = new MedLabContext();

        public static bool LogInUser(string username, ref SignedInUser user)
        {
            try
            {
                if (db.Users.Where(u => u.Login == username).FirstOrDefault() == null)
                    return false;

                user = new SignedInUser(db.Users.Where(u => u.Login == username).FirstOrDefault());

                return true;
            }
            catch (Exception)  { return false;}
        }
        public static bool CheckUser(string username, string password)
        {
            try
            {
                var user = db.Users.Where(u => u.Login == username && u.Password == password).FirstOrDefault();
                if (user != null)
                    return true;
                else
                    return false;
            }
            catch (Exception){return false;}
        }

        public static bool IsPESELTaken(string PESEL)
        {
            var user = db.Users.Where(u => u.PESEL == PESEL).FirstOrDefault();
            if (user == null){  return false; }
            else { return true;}
        }

        public static bool IsLoginTaken(string login)
        {
            var user = db.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user == null) {  return false; }
            else  { return true; }
        }

        public static bool IsLoginTakenByAnotherUser(string login, int userId)
        {
            return db.Users.Any(u => u.Login == login && u.id != userId);
        }

        public static bool AddUser(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public static List<Test> GetActiveTests()
        {
            try
            {
                List<Test> ActiveTests = db.Tests.Where(t => t.IsActive == true).ToList();
                return ActiveTests;
            }
            catch { return null; }
        }

        public static List<Test> GetAllTests()
        {
            try
            {
                List<Test> AllTests = db.Tests.ToList();
                return AllTests;
            }
            catch { return null; }
        }

        public static bool EditUserCommon(string login, string password, string phoneNumber, int userId)
        {
            try
            {
                var user = db.Users.Where(u => u.id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Login = login;
                    user.Password = password;
                    user.PhoneNumber = phoneNumber;
                    db.SaveChanges();
                    return true;
                }
                return false; 
            }catch { return false; }
        }

        public static User GetUser(string PESEL)
        {
            try 
            {
                var user = db.Users.Where(u=>u.PESEL==PESEL).FirstOrDefault();
                if (user != null) { return user; }
                return null;
            }
            catch { return null; }
        }

        public static bool IsTestNameTaken(string testName)
        {
            try
            {
                var test =db.Tests.Where(t=>t.TestName== testName).FirstOrDefault();
                if(test != null)   { return true; }
                return false;
            }  catch { return false; }
        }
        //@Matylda usun niepotrzebne
        public static bool AddTest(Test test)
        {
            try
            {
                db.Tests.Add(test);
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public static bool EditTest(Test test, Test newData)
        {
            try 
            {
                test.TestName= newData.TestName;
                test.Description= newData.Description;
                test.Price= newData.Price;
                test.Category= newData.Category;
                test.IsActive = newData.IsActive;
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public static bool EditTest(Test test, string testName, string description, float price, int category)
        {
            try
            {
                test.TestName = testName;
                test.Description = description;
                test.Price = price;
                test.Category = category;
              
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
        public static bool DeleteTest(Test test)
        {
            try
            {
                if (test != null)
                {
                    test.IsActive = false;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public static bool ChangeTestStatus(Test test)
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

        public static List<CategoryDictionary> GetCategories()
        {
            try { return db.CategoryDictionaries.ToList();  }
            catch { return null; }
        }
        public static Dictionary<int, string> GetCategoriesDictionary()
        {
            try 
            {
                Dictionary<int, string> categories = new Dictionary<int, string>();
                List<CategoryDictionary> categoryDictionaries = db.CategoryDictionaries.ToList();
                foreach (var category in categoryDictionaries) 
                {
                    categories.Add(category.id,category.CategoryName);
                }
                return categories;

            }
            catch { return null; }
        }

        public static List<User> LoadUsers()
        {
            try {
                return db.Users
                                 .Include(u => u.UserTypeNavigation)
                                 .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania użytkowników: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return null; }
        }
        public static bool ChangeUserStatus(int userId)
        {
            try {
                var user = db.Users.Where(u => u.id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.IsActive = !user.IsActive;
                    db.SaveChanges();
                    return true;
                }
                return false; }
            catch { return false; }
        }

        public static bool EditUserAdmin(int userId, string login, int userType, string password, string name, string surname, string PESEL, string? phone=null )
        {
            try 
            {
                var user=db.Users.Where(u=>u.id == userId).FirstOrDefault();
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

    }
}

