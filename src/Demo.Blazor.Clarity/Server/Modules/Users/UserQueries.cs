namespace Demo.Blazor.Clarity.Server.Modules.Users;

using Demo.Blazor.Clarity.Server.Data;
using Demo.Blazor.Clarity.Server.Domain.Entities;

[ExtendObjectType(OperationTypeNames.Query)]
public class UserQueries
{
	[UsePaging]
	[UseProjection]
	[UseFiltering]
	[UseSorting]
	public IQueryable<User> GetUser(ApplicationDbContext dbContext)
	{
		return dbContext.Users;
	}
}