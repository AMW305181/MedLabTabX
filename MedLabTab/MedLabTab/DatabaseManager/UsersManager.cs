using MedLabTab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Navigation;

namespace MedLabTab.DatabaseManager
{
    internal class UsersManager
    {
        public bool LogInUser(MedLabContext db,string username, ref SignedInUser user)
        {
            try
            {
                if (db.Users.Where(u => u.Login == username).FirstOrDefault() == null)
                    return false;

                user = new SignedInUser(db.Users.Where(u => u.Login == username).FirstOrDefault());
                return true;
            }
            catch (Exception) { return false; }
        }

        public string GetHashedPassword (MedLabContext db, string username)
        {
            string password = db.Users.Where(u => u.Login == username).Select(u => u.Password).First();
            return password;
        }

        public bool CheckUser(MedLabContext db, string username, string password)
        {
            try
            {
                var user = db.Users.Where(u => u.Login == username && u.Password == password).FirstOrDefault();
                if (user != null)
                    return true;
                else
                    return false;
            }
            catch (Exception) { return false; }
        }
        public bool IsPESELTaken(MedLabContext db, string PESEL)
        {
            var user = db.Users.Where(u => u.PESEL == PESEL).FirstOrDefault();
            if (user == null) { return false; }
            else { return true; }
        }
        public bool IsLoginTaken(MedLabContext db, string login)
        {
            var user = db.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user == null) { return false; }
            else { return true; }
        }
        public bool IsLoginTakenByAnotherUser(MedLabContext db, string login, int userId)
        {
            return db.Users.Any(u => u.Login == login && u.id != userId);
        }
        public bool AddUser(MedLabContext db, User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }
        public List<User> GetActivePatients(MedLabContext db)
        {
            try
            {
                List<User> ActiveUsers = db.Users.Where(t => t.IsActive == true && t.UserType == 4).ToList();
                return ActiveUsers;
            }
            catch { return null; }
        }
        public bool EditUserCommon(MedLabContext db,string login, string password, string phoneNumber, int userId)
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
            }
            catch { return false; }
        }
        public User GetUser(MedLabContext db, string PESEL)
        {
            try
            {
                var user = db.Users.Where(u => u.PESEL == PESEL).FirstOrDefault();
                if (user != null) { return user; }
                return null;
            }
            catch { return null; }
        }
        public User GetUserById(MedLabContext db, int Id)
        {
            try
            {
                var user = db.Users.Where(u => u.id == Id).FirstOrDefault();
                if (user != null) { return user; }
                return null;
            }
            catch { return null; }
        }
        public List<User> LoadUsers(MedLabContext db)
        {
            try
            {
                return db.Users
                                 .Include(u => u.UserTypeNavigation)
                                 .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania użytkowników: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

    }
}
