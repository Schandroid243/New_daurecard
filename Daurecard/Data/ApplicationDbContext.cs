using System;
using Daurecard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Daurecard.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Entreprise> Entreprises { get; set; }

        public DbSet<DownloadVCard> DownloadVCards { get; set; }

        public DbSet<Subscribe> Subscribes { get; set; }
    }
}
