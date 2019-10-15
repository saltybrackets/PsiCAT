namespace PsiCat.Server
{
    using Microsoft.EntityFrameworkCore;
    using global::PsiCat.Server.Models;


    public class UserDataContext : DbContext
    {
        public DbSet<User> Users { get; set; }


        public UserDataContext() : base()
        {
        }
        
        public UserDataContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Set the filename of the database to be created
            optionsBuilder.UseSqlite("Data Source=db.sqlite");
        }
 
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
 
            //Define the Table(s) and References to be created automatically
            modelBuilder.Entity<User>(user =>
                {
                    user.HasKey(entry => entry.Id);
                    user.Property(entry => entry.Id).ValueGeneratedOnAdd();
                    user.Property(entry => entry.DisplayName).IsRequired().HasMaxLength(255);
                    user.Property(entry => entry.FirstName).IsRequired().HasMaxLength(255);
                    user.Property(entry => entry.LastName).IsRequired().HasMaxLength(255);
                    user.ToTable("Users");
                });
        }
    }
}