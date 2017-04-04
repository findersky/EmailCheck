using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            EmailCheckHelper emailCheck = new EmailCheckHelper();
            emailCheck.checkEmail("check1@verify-email.org");
            Console.WriteLine(emailCheck.ToString());
            emailCheck.checkEmail("jenny@blackducksoftware.com");
            Console.WriteLine(emailCheck.ToString());
            Console.Read();
        }
    }
}
