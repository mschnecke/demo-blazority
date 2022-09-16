namespace Demo.Blazor.Clarity.Client.Pages;

using StrawberryShake;

public interface IUserService
{
	Task<IUser_User?> GetUsersAsync(UserQueryVariables queryVariables,
		CancellationToken cancellationToken = default);

	Task<ICreateUser_CreateUser?> CreateUserAsync(
		string email,
		string firstName,
		string lastName,
		CancellationToken cancellationToken = default);

	Task<IDeleteUser_DeleteUser?> DeleteUserAsync(
		Guid id,
		CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
	private readonly BlazorityClient client;

	public UserService(BlazorityClient client)
	{
		this.client = client;
	}

	public async Task<ICreateUser_CreateUser?> CreateUserAsync(string email,
		string firstName,
		string lastName,
		CancellationToken cancellationToken = default)
	{
		var result = await this.client
			.CreateUser
			.ExecuteAsync(new CreateUserInput {Email = email, FirstName = firstName, LastName = lastName},
				cancellationToken);

		if (result.IsSuccessResult())
		{
			return result.Data!.CreateUser;
		}

		return null;
	}

	public async Task<IDeleteUser_DeleteUser?> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var result = await this.client
			.DeleteUser
			.ExecuteAsync(new DeleteUserInput {Id = id}, cancellationToken);

		if (result.IsSuccessResult())
		{
			return result.Data!.DeleteUser;
		}

		return null;
	}

	public async Task<IUser_User?> GetUsersAsync(UserQueryVariables queryVariables,
		CancellationToken cancellationToken = default)
	{
		var result = await this.client
			.User
			.ExecuteAsync(queryVariables.First,
				queryVariables.After,
				queryVariables.Last,
				queryVariables.Before,
				queryVariables.Filter,
				queryVariables.Order,
				cancellationToken);

		if (result.IsSuccessResult())
		{
			return result.Data!.User;
		}

		return null;
	}
}

public class UserQueryVariables
{
	public int? First { get; set; }

	public int? Last { get; set; }

	public string? After { get; set; }

	public string? Before { get; set; }

	public UserFilterInput? Filter { get; set; }

	public IReadOnlyList<UserSortInput> Order { get; set; } =
		new List<UserSortInput> {new UserSortInput {Email = SortEnumType.Asc}};
}