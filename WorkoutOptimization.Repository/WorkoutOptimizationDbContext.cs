using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Repository
{
    public class WorkoutOptimizationDbContext : IdentityDbContext<User>
    {
        public DbSet<GyroscopeData> GyrosScropeData { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        public WorkoutOptimizationDbContext(DbContextOptions<WorkoutOptimizationDbContext> opt) : base(opt)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>()
               .HasMany(x => x.Exercises)
               .WithMany(x => x.Users)
               .UsingEntity<Training>(
               x => x.HasOne(x => x.Exercise).
               WithMany().HasForeignKey(x => x.ExerciseId).OnDelete(DeleteBehavior.Cascade),
               x => x.HasOne(x => x.User).
               WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade));



            modelBuilder.Entity<User>()
                .HasMany(x => x.GyroscopeData)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Training>()
                .HasMany(x => x.GyroscopeData)
                .WithOne(x => x.Training)
                .HasForeignKey(x => x.TrainingId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Training>().HasData(new Training()
            {
                Start = DateTime.Now,
                End = DateTime.Now,
                ExerciseId = 1,
                TrainingId = 1,
                UserId = "70a9df3f-03b8-4420-a6a5-8f713c3efbb2"

            });
            var user= new User()
            {
                Id = "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true, // Ha az email megerősítés szükséges
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()

            };
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "admin");

            modelBuilder.Entity<User>().HasData(user);

            modelBuilder.Entity<GyroscopeData>().HasData(new GyroscopeData()
            {
                AccelX = 1,
                AccelY = 1,
                AccelZ = 1,
                GyrosX = 1,
                GyrosY = 1,
                GyrosZ = 1,
                Date = DateTime.Now,
                GyroscopeDataId = 1,
                UserId = "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                TrainingId = 1,

            });
            modelBuilder.Entity<Exercise>().HasData(new Exercise()
            {
                ExerciseId = 1,
                Name = "Fekvenyomás",
                Description = "Feküdj le a padra, és egy rudat tolj el a mellkasodtól, majd engedd rá vissza",
                MuscleGroup = MuscleGroup.Chest
            });

            modelBuilder.Entity<Promotion>().HasData(new Promotion()
            {
                PromotionId = 1,
                Name = "Valós idejű visszajelzés",
                Description = "A valós idejű visszajelzés rendkívül hasznos dolog az edzés közben, mert azon nyomban látja ön is, hogy az adott gyakorlatot megfelelően végzi-e",
            });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole()
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }, new IdentityRole()
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                }

            );
        }
    }
}
