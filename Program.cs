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
            string input = "";
            bool isSucess=emailCheck.checkEmail("check1@verify-email.org");
            Console.WriteLine(emailCheck.ToString());
            Console.WriteLine("请输入电子邮件地址:");
            while ((input = Console.ReadLine()) != "")
            {
                emailCheck.checkEmail(input);
                Console.WriteLine(emailCheck.ToString());
                Console.WriteLine("请输入电子邮件地址:");
            }
            Console.Read();
        }
    }
}
