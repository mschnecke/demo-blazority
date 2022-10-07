namespace Demo.Blazor.Clarity.Server.Data;

using Bogus;
using Demo.Blazor.Clarity.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

public static class DatabaseSeeder
{
	public static WebApplication InitializeDatabase(this WebApplication application)
	{
		MigrateDatabaseContext<ApplicationDbContext>(application.Services);

		using var serviceScope = application.Services.CreateScope();
		var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

		if (dbContext is not null)
		{
			if (dbContext.Users.Any() == false)
			{
				Randomizer.Seed = new Random(8675309);
				var count = Random.Shared.Next(1, 6);

				for (var i = 0; i < count; i++)
				{
					var user = new Faker<User>()
							.StrictMode(true)
							.RuleFor(u => u.Id, f => Guid.NewGuid())
							.RuleFor(u => u.FirstName, f => f.Name.FirstName())
							.RuleFor(u => u.LastName, f => f.Name.LastName())
							.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
						;

					dbContext.Users.Add(user);
				}

				dbContext.SaveChanges();
			}
		}

		return application;
	}

	private static void MigrateDatabaseContext<T>(IServiceProvider serviceProvider) where T : DbContext
	{
		using var serviceScope = serviceProvider.CreateScope();
		var logger = serviceScope
			.ServiceProvider.GetRequiredService<ILoggerFactory>()
			.CreateLogger("DbInitializer");

		logger.LogInformation("Migrate {DbContext} ...", typeof(T).Name);

		try
		{
			var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
			var migrator = dbContext.GetInfrastructure().GetService<IMigrator>();

			var appliedMigrations = dbContext
				.Database
				.GetAppliedMigrations()
				.ToList();

			var pendingMigrations = dbContext
				.Database
				.GetPendingMigrations()
				.ToList();

			foreach (var appliedMigration in appliedMigrations)
			{
				logger.LogInformation("Applied migration: '{AppliedMigration}'", appliedMigration);
			}

			foreach (var pendingMigration in pendingMigrations)
			{
				logger.LogInformation("Pending migration: '{PendingMigration}'", pendingMigration);
			}

			foreach (var migration in pendingMigrations)
			{
				logger.LogInformation("Applying: '{Migration}'", migration);
				migrator!.Migrate(migration);
			}
		}
		catch (Exception exception)
		{
			logger.LogError("{Exception}", exception);
		}

		logger.LogInformation("Migrate {DbContext} ... finished", typeof(T).Name);
	}
}