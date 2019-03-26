using Microsoft.EntityFrameworkCore;
using SkillsHeroes.IssuesApi.Data.Models;

namespace SkillsHeroes.IssuesApi.Data
{
    public class IssuesContext : DbContext
    {
        public IssuesContext(DbContextOptions<IssuesContext> dbContextOptions)
            :base(dbContextOptions)
        { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ApiKey).IsRequired();
                entity.HasIndex(x => x.ApiKey).IsUnique();
                entity
                    .HasMany(x => x.Issues)
                    .WithOne(i => i.Application)
                    .HasForeignKey(x => x.ApplicationId);
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity
                    .HasMany(x => x.Comments)
                    .WithOne(x => x.Issue)
                    .HasForeignKey(x => x.IssueId);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }
    }
}
