using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;

namespace MedLabTab.ViewModels
{
    public class SignedInUser : User
    {
        public SignedInUser(User user)
        {
            this.id = user.id;
            this.Login=user.Login;
            this.UserType = user.UserType;
            this.Password = user.Password;
            this.IsActive = user.IsActive;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.PESEL = user.PESEL;
            this.PhoneNumber = user.PhoneNumber;
        }
        public SignedInUser()
        {
            this.id = 0;
            this.Login = string.Empty;
            this.Password = string.Empty;
            this.IsActive = false;
            this.Name = string.Empty;
            this.Surname = string.Empty;           
            this.PESEL = string.Empty;
            this.PhoneNumber = string.Empty;
        }
    }
}
