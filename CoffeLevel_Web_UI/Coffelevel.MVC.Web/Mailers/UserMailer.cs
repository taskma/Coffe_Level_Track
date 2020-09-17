using System.Collections.Generic;
using CoffeLevel.Client.Web.Infrastructure;
using CoffeLevel.Client.Web.Models;
using CoffeLevel.Data.Entities;
using Mvc.Mailer;

namespace CoffeLevel.Client.Web.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
        private const string AdminMail = "mail@mail.com.tr";

        public UserMailer()
		{
			MasterName="_Layout";
		}

      

        private static void SplitEmails(MvcMailMessage x, string email, bool isCC)
        {
            if (!email.IsNullOrEmpty())
            {
                foreach (var item in email.Split(';'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (isCC)
                        {
                            x.CC.Add(item.ToSafeTrim());
                        }
                        else
                        {
                            x.To.Add(item.ToSafeTrim());
                        }
                    }
                        
                }
            }
        }

      

        public virtual MvcMailMessage TestMail(string MailAdress)
        {
            return Populate(x =>
            {
                x.Subject = string.Concat("Test mail");
                x.ViewName = "TestMail";
                x.To.Add(MailAdress == null ? AdminMail : MailAdress);
            });
        }

 	}
}