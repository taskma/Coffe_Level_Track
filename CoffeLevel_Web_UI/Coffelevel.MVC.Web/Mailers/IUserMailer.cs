using System.Collections.Generic;
using CoffeLevel.Client.Web.Infrastructure;
using CoffeLevel.Client.Web.Models;
using CoffeLevel.Data.Entities;
using Mvc.Mailer;

namespace CoffeLevel.Client.Web.Mailers
{ 
    public interface IUserMailer
    {
            MvcMailMessage TestMail(string MailAdress);
	}
}