using MedLabTab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Navigation;

namespace MedLabTab.DatabaseManager
{
    internal class UsersManager
    {
        TransactionOptions options = new TransactionOptions {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public bool LogInUser(MedLabContext db,string username, ref SignedInUser user)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    if (db.Users.Where(u => u.Login == username).FirstOrDefault() == null)
                        return false;

                    user = new SignedInUser(db.Users.Where(u => u.Login == username).FirstOrDefault());
                    scope.Complete();
                    return true;
                }
                catch (Exception) { return false; }
            }
        }

        public string GetHashedPassword (MedLabContext db, string username)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                string password = db.Users.Where(u => u.Login == username).Select(u => u.Password).First();
                scope.Complete();
                return password;
            }
        }

        public bool CheckUser(MedLabContext db, string username, string password)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var user = db.Users.Where(u => u.Login == username && u.Password == password).FirstOrDefault();
                    scope.Complete();
                    if (user != null)
                        return true;
                    else
                        return false;
                }
                catch (Exception) { return false; }
            }
        }
        public bool IsPESELTaken(MedLabContext db, string PESEL)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                var user = db.Users.Where(u => u.PESEL == PESEL).FirstOrDefault();
                scope.Complete();
                if (user == null) { return false; }
                else { return true; }
            }
        }
        public bool IsLoginTaken(MedLabContext db, string login)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                var user = db.Users.Where(u => u.Login == login).FirstOrDefault();
                scope.Complete();
                if (user == null) { return false; }
                else { return true; }
            }
        }
        public bool IsLoginTakenByAnotherUser(MedLabContext db, string login, int userId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                bool exists= db.Users.Any(u => u.Login == login && u.id != userId);
                scope.Complete();
                return exists;
            }
        }
        public bool AddUser(MedLabContext db, User user)
        {

            try
            {
                user.Password=PasswordHasher.Hash(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }
        public List<User> GetActivePatients(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<User> ActiveUsers = db.Users.Where(t => t.IsActive == true && t.UserType == 4).ToList();
                    scope.Complete();
                    return ActiveUsers;
                }
                catch { return null; }
            }
        }
        public List<User> GetActiveNurses(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<User> ActiveUsers = db.Users.Where(t => t.IsActive == true && t.UserType == 2).ToList();
                    scope.Complete();
                    return ActiveUsers;
                }
                catch { return null; }
            }
        }
        public bool EditUserCommon(MedLabContext db, string login, string password, string phoneNumber, int userId)
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
                    var user = db.Users.Where(u => u.id == userId).FirstOrDefault();

                    if (user != null)
                    {
                        if (!string.IsNullOrWhiteSpace(password))
                        {
                            password = PasswordHasher.Hash(password);
                            user.Password = password;
                        }
                        user.Login = login;
                        user.PhoneNumber = phoneNumber;
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
        public User GetUser(MedLabContext db, string PESEL)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var user = db.Users.Where(u => u.PESEL == PESEL).FirstOrDefault();
                    scope.Complete();
                    if (user != null) { return user; }
                    return null;
                }
                catch { return null; }
            }
        }
        public User GetUserById(MedLabContext db, int Id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var user = db.Users.Where(u => u.id == Id).FirstOrDefault();
                    if (user != null) { return user; }
                    return null;
                }
                catch { return null; }
            }
        }
        public List<User> LoadUsers(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
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
}
