using System;
using Microsoft.AspNetCore.Identity;

namespace Daurecard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
