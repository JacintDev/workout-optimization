
using Microsoft.EntityFrameworkCore;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string conn = "Server=localhost;Port=3306;Database=workoutoptimization;Uid=root;Pwd=;";
            builder.Services.AddDbContext<WorkoutOptimizationDbContext>(opt =>
            {
                opt.UseMySql(conn, new MySqlServerVersion("8.0.30")).UseLazyLoadingProxies();
            });

            builder.Services.AddTransient<IRepository<GyroscopeData>, Repository<GyroscopeData>>();
            builder.Services.AddTransient<IGyroscopeDataLogic, GyroscopeDataLogic>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run("http://0.0.0.0:5135");
        }
    }
}
