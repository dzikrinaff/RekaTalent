using RekaTalent.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static RekaTalent.Data.RekaTalentDbContext;
using System.IO;
using RekaTalent.Data;



namespace RekaTalent.Data
{
    public class RekaTalentDbContext : IdentityDbContext<ApplicationUser>
    {
        public RekaTalentDbContext(DbContextOptions<RekaTalentDbContext> options) : base(options) { }

        public class ApplicationUser : IdentityUser
        {
        }

        
        public DbSet<FileModel> Files { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);





        }
    }
}
