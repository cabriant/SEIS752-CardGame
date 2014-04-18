using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEIS752CardGame.Models.Login
{
    public class AuthResponse
    {
        public bool Authenticated { get; set; }
        public string Error { get; set; }
    }
}