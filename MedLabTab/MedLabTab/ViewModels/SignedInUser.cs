using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;

namespace MedLabTab.ViewModels
{
    //to jest klasa tymczasowa, by katalog byl utworzony w mainie
    internal class SignedInUser : User
    {
        public SignedInUser(User user)
        {
            this.id = user.id;
            this.UserType = user.UserType;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.Password = user.Password;
            this.PESEL = user.PESEL;
            this.PhoneNumber = user.PhoneNumber;
        }
    }
}
