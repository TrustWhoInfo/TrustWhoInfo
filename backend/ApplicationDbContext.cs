using Microsoft.EntityFrameworkCore;
using backend;

namespace backend
{
    public class ApplicationDbContext : DbContext
    {        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Analytic> Analytics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UseAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("author");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Url).HasColumnName("url");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Image).HasColumnName("image");
                entity.Property(e => e.Lang).HasColumnName("lang");
                entity.Property(e => e.Title).HasColumnName("title");
            });
            modelBuilder.Entity<Media>(entity =>
            {
                entity.ToTable("media");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Url).HasColumnName("url");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Image).HasColumnName("image");
                entity.Property(e => e.Lang).HasColumnName("lang");
            });
            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("article");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.MediaId).HasColumnName("media_id");
                entity.Property(e => e.AuthorId).HasColumnName("author_id");
                entity.Property(e => e.Url).HasColumnName("url");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Subject).HasColumnName("subject");
                entity.Property(e => e.Lang).HasColumnName("lang");
                entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            });
            modelBuilder.Entity<Analytic>(entity =>
            {
                entity.ToTable("analytic");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Url).HasColumnName("url");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Contacts).HasColumnName("contacts");
                entity.Property(e => e.Lang).HasColumnName("lang");
            });
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("review");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.AnalyticId).HasColumnName("analytic_id");
                entity.Property(e => e.ArticleId).HasColumnName("article_id");
                entity.Property(e => e.Level).HasColumnName("level");
                entity.Property(e => e.Url).HasColumnName("url");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            });
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("setting");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Code).HasColumnName("code");
                entity.Property(e => e.Json).HasColumnName("json");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.Avatar).HasColumnName("avatar");
                entity.Property(e => e.Created).HasColumnName("created");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Phone).HasColumnName("phone");
            });
            modelBuilder.Entity<UserAuth>(entity =>
            {
                entity.ToTable("user_auth");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Method).HasColumnName("method");
                entity.Property(e => e.External1).HasColumnName("external1");
                entity.Property(e => e.External2).HasColumnName("external2");
                entity.Property(e => e.LastLogin).HasColumnName("last_login");
                entity.Property(e => e.Created).HasColumnName("created");
            });
        }
    }
}