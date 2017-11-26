using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CreativeSandbox.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("DefaultConnection") { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }
    }
}