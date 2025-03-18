
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Logic.Helpers;
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

            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder()
            {
                DataSource = "localhost",
                InitialCatalog = "workoutoptimization",
                UserID = "sa",
                Password = "Horthy2000?",
                TrustServerCertificate = true,

            };

            builder.Services.AddDbContext<WorkoutOptimizationDbContext>(opt =>
            {
                opt.UseSqlServer(conn.ConnectionString).UseLazyLoadingProxies();
            });
            //----------------------------------------------

            builder.Services.AddScoped<IRepository<GyroscopeData>, Repository<GyroscopeData>>();
            builder.Services.AddScoped<IGyroscopeDataLogic, GyroscopeDataLogic>();
            
            //Automapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

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
