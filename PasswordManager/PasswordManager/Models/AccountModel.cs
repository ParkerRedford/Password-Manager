using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordGeneratorCore.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
    }
}
