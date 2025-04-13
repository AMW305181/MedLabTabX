using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
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
        //zobaczyc czy podany caly user czy pojedynczo
        //public static bool EditUserAdmin (string login, int type, string password, )
        //{
        //    try { return true; }
        //    catch { return false; }
        //}

        public static bool IsTestNameTaken(string testName)
        {
            try
            {
                var test =db.Tests.Where(t=>t.TestName== testName).FirstOrDefault();
                if(test != null)   { return true; }
                return false;
            }  catch { return false; }
        }

    }
}

