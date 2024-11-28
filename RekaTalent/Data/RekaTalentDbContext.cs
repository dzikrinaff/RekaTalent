using RekaTalent.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static RekaTalent.Data.RekaTalentDbContext;
using RekaTalent.Data;
using UserAuthApi.Models;


namespace RekaTalent.Data
{
    public class RekaTalentDbContext : IdentityDbContext<ApplicationUser>
    {
        public RekaTalentDbContext(DbContextOptions<RekaTalentDbContext> options) : base(options) { }

        public class ApplicationUser : IdentityUser
        {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Psychotest> Psychotests { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewSchedule> InterviewSchedules { get; set; }
        public DbSet<PsychotestSchedule> PsychotestSchedules { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<LoginModel> LoginModels { get; set; }
        public DbSet<RegisterModel> RegisterModels { get; set; }




        // Pastikan relasi diatur di sini
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);




        }
    }
}