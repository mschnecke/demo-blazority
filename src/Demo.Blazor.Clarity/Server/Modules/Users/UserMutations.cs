namespace Demo.Blazor.Clarity.Server.Modules.Users;

using Demo.Blazor.Clarity.Server.Modules.Users.Commands;
using MediatR;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class UserMutations
{
	public async Task<CreateUserCommand.CreateUserPayload> CreateUser(
		[GraphQLNonNullType] CreateUserCommand.CreateUserInput input,
		[Service] IMediator mediator,
		[Service] IHttpContextAccessor httpContextAccessor)
	{
		return await mediator.Send(input, httpContextAccessor.HttpContext!.RequestAborted);
	}

	public async Task<DeleteUserCommand.DeleteUserPayload> DeleteUser(
		[GraphQLNonNullType] DeleteUserCommand.DeleteUserInput input,
		[Service] IMediator mediator,
		[Service] IHttpContextAccessor httpContextAccessor)
	{
		return await mediator.Send(input, httpContextAccessor.HttpContext!.RequestAborted);
	}
}