using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Models;

namespace P7CreateRestApi.Data
{
    public class LocalDbContext : IdentityDbContext<User>
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles =
            [
                new IdentityRole {
                    Id="eaa2b49c-8ec2-47ef-b5b1-50dfcba22239", 
                    Name = "Admin", NormalizedName = "ADMIN"
                },
                new IdentityRole {
                    Id="f42eb403-6a4e-4a32-8cd1-6ded86b42792",
                    Name = "User", NormalizedName = "USER"
                }
            ];
            builder.Entity<IdentityRole>().HasData(roles);
        }

        public DbSet<BidList> BidLists { get; set; } = null!;
        public DbSet<CurvePoint> CurvePoints { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<RuleName> RuleNames { get; set; } = null!;
        public DbSet<Trade> Trades { get; set; } = null!;
    }
}