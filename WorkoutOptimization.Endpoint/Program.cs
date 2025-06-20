
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using WorkoutOptimization.Endpoint.Helpers;
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // JWT Authentication hozzáadása a Swaggerhez
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Adja meg a JWT tokent a következõ formátumban: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy
                        .WithOrigins("http://localhost:4200") //  Itt add meg az Angular URL-jét!
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()); //  Ezt csak konkrét origin esetén lehet!
            });



            //mysql connection string create
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
            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
            }).AddEntityFrameworkStores<WorkoutOptimizationDbContext>().AddDefaultTokenProviders();


            builder.Services.AddScoped<IRepository<GyroscopeData>, Repository<GyroscopeData>>();
            builder.Services.AddScoped<IGyroscopeDataLogic, GyroscopeDataLogic>();
            builder.Services.AddScoped<IAuthorizationLogic, AuthorizationLogic>();
            builder.Services.AddScoped<IRepository<Exercise>, Repository<Exercise>>();
            builder.Services.AddScoped<IExerciseLogic, ExerciseLogic>();
            builder.Services.AddScoped<IRepository<Training>, Repository<Training>>();
            builder.Services.AddScoped<ITrainingLogic, TrainingLogic>();
            builder.Services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            builder.Services.AddScoped<IPromotionLogic, PromotionLogic>();
            builder.Services.AddSingleton<IBicepsCurlLogic, BicepsCurlLogic>();
            builder.Services.AddSingleton<ConcurrentQueue<GyroscopeDataDto>>();
            builder.Services.AddHostedService<GyroscopeDataProcessor>();

            builder.Services.AddSignalR();

            //Automapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });
            //exception handler
            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add<ExceptionFilter>();
                opt.Filters.Add<ValidationFilter>();

            });

        
            
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "http://www.security.org",
                    ValidIssuer = "http://www.security.org",
                    IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes("nagyonhosszutitkoskodhelyenagyonhosszutitkoskodhelye"))
                };
            });
            var app = builder.Build();

            app.UseCors("AllowFrontend"); // CORS middleware aktiválása

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseExceptionHandler(c => c.Run(async context =>
            //{
            //    var exception = context.Features
            //    .Get<IExceptionHandlerPathFeature>()
            //    .Error;
            //    var response = new { error = exception.Message };
            //    await context.Response.WriteAsJsonAsync(response);
            //}));

            //Authentication
            app.UseWebSockets();
          
            app.UseAuthentication();

            app.UseAuthorization();
            app.MapHub<ExerciseHub>("/exercisehub");

            app.MapControllers();

            app.Run("http://0.0.0.0:5135");
        }
    }
}
