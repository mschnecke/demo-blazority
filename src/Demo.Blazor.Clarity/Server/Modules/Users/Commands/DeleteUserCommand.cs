namespace Demo.Blazor.Clarity.Server.Modules.Users.Commands;

using Demo.Blazor.Clarity.Server.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteUserCommand
{
	public record DeleteUserInput(Guid Id) : IRequest<DeleteUserPayload>;

	public record DeleteUserPayload(Guid Id);

	public class Handler : IRequestHandler<DeleteUserInput, DeleteUserPayload>
	{
		private readonly ApplicationDbContext dbContext;

		public Handler(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <inheritdoc />
		public async Task<DeleteUserPayload> Handle(DeleteUserInput request, CancellationToken cancellationToken)
		{
			var entity = await this.dbContext
				.Users
				.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

			if (entity is null)
			{
				return new DeleteUserPayload(request.Id);
			}

			this.dbContext.Remove(entity);
			await this.dbContext.SaveChangesAsync(cancellationToken);

			return new DeleteUserPayload(request.Id);
		}
	}
}