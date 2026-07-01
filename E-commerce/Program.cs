
using E_Commerce.Model;
using E_Commerce.Repository;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CS"));
                //.LogTo(Console.WriteLine, LogLevel.Information)
                //.EnableSensitiveDataLogging();
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.AddCors(options =>
            {

                options.AddPolicy("MyPolicy", Policy =>
                {
                    Policy.WithOrigins("http://127.0.0.1:5500")
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                });

            });



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters()
                {

                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey
                                (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:secret"]!)),


                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            builder.Services.AddScoped<IproductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepostiry>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartService>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            builder.Services.AddScoped<IProfileService, ProfileService>();

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            try
            {
                using var scope = app.Services.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (db != null)
                    db.Database.CanConnect();

            }
            catch (Exception ex)
            {
                app.Logger.LogCritical(ex, "Database Cann't Connect to Database");
                throw;
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"message\":\"An error occurred\",\"error\":\"{ex.Message}\"}}");
                }
            });


            //app.UseExceptionHandler(configure =>
            //{
            //    configure.Run(async context =>
            //    {
            //        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            //        Console.WriteLine($"ERROR: {exception?.Message}");
            //        context.Response.StatusCode = 500;
            //        context.Response.ContentType = "application/json";
            //        await context.Response.WriteAsync($"{{\"message\":\"An error occurred\",\"error\":\"{exception?.Message}\"}}");
            //    });
            //});


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }




            // Log every Reqeust and URl 
            app.Use(async (context, next) =>
            {

                Console.WriteLine($" Request :  {context.Request.Method} {context.Request.Path} ");

                await next();

                Console.WriteLine($" Response {context.Response.StatusCode}");

            });

            app.UseCors("MyPolicy");


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }
    }
}
