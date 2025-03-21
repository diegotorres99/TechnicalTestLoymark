using back_end.Repository;
using back_end.Helpers;

namespace TaskManager.Model.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            SQLitePCL.Batteries.Init();
            services.AddControllers();

            services.AddSingleton<IDatabaseHelper>(provider =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Database connection string is not configured.");
                }
                return new DatabaseHelper(connectionString);
            });

            services.AddHttpClient<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IActivitiesRepository, ActivitiesRepository>();

            return services;
        }
    }
}
