using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLabTab.ViewModels
{
    public class LoginVM
    {
        public bool IsCredentialsNoneEmpty(string username, string password)
            => !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);


    }
}
