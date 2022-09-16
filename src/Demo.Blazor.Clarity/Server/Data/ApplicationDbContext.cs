namespace Demo.Blazor.Clarity.Server.Data;

using Demo.Blazor.Clarity.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<User> Users => this.Set<User>();
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	/// <inheritdoc />
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlite("Data Source=users.db;",
			b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
