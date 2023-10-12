using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DrTrottoirApi.Entities
{
    public class DrTrottoirDbContext : IdentityDbContext<
        User,
        Role,
        Guid,
        IdentityUserClaim<Guid>,
        UserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public DrTrottoirDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DrTrottoirDbContext()
        {

        }

        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Round> Rounds { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyGarbageCollection> CompanyGarbageCollections { get; set; }
        public virtual DbSet<GarbageCollection> GarbageCollections { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<Syndic> Syndics { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<WorkArea> WorkAreas { get; set; }
        public virtual DbSet<GarbageType> GarbageTypes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<GarbageCollectionGarbageType> GarbageCollectionGarbageTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(x =>
            {
                x.HasKey(y => new { y.UserId, y.RoleId });

                x.HasOne(y => y.User)
                    .WithMany(y => y.UserRoles)
                    .HasForeignKey(y => y.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(y => y.Role)
                    .WithMany(y => y.UserRoles)
                    .HasForeignKey(y => y.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshToken>(x =>
            {
                x.HasOne(y => y.User)
                    .WithMany(y => y.RefreshTokens)
                    .HasForeignKey(y => y.UserId)
                    .IsRequired();
            });
        }

    }
}
