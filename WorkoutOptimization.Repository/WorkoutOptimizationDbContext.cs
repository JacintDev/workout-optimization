using Microsoft.EntityFrameworkCore;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Repository
{
    public class WorkoutOptimizationDbContext : DbContext
    {
        public DbSet<GyroscopeData> GyrosScropeData { get; set; }

        public WorkoutOptimizationDbContext(DbContextOptions<WorkoutOptimizationDbContext> opt) : base(opt)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GyroscopeData>().HasData(new GyroscopeData()
            {
                AccelX = 1,
                AccelY = 1,
                AccelZ = 1,
                GyrosX = 1,
                GyrosY = 1,
                GyrosZ = 1,
                Date = DateTime.Now,
                GyroscopeDataId = 1
            });
        }
    }
}
