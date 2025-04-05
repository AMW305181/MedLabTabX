using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            catch (Exception)
            {
                return false;
            }
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
            catch (Exception)
            {
                return false;
            }
        }
    }
}

