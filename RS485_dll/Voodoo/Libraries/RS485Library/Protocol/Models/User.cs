using System;
using System.Collections.Generic;
using System.Text;

namespace Voodoo.Libraries.RS485Library.Protocol.Models
{
    public class User
    {
        private String username;
        private String password;

        public User(String username, String password)
        {
            this.username = username;
            this.password = password;
        }

        public String UserName
        {
            get { return username; }
        }

        public String Password
        {
            get { return password; }
        }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(username) &&
                   !String.IsNullOrEmpty(password);
        }
    }
}
