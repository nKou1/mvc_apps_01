using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc_apps_01.Models;

namespace mvc_apps_01.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<mvc_apps_01.Models.TmDbTrending> TmDbTrendings { get; set; }
        public DbSet<mvc_apps_01.Models.TmDbComingSoon> TmDbComingSoon { get; set; }
    }
}
