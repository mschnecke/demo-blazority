namespace Demo.Blazor.Clarity.Server.Modules.Users.Commands;

using Demo.Blazor.Clarity.Server.Data;
using Demo.Blazor.Clarity.Server.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class CreateUserCommand
{
	public record CreateUserInput(string Email, string? FirstName, string? LastName) : IRequest<CreateUserPayload>;

	public record CreateUserPayload(User? User);

	public class Handler : IRequestHandler<CreateUserInput, CreateUserPayload>
	{
		private readonly ApplicationDbContext dbContext;

		public Handler(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <inheritdoc />
		public async Task<CreateUserPayload> Handle(CreateUserInput request, CancellationToken cancellationToken)
		{
			var entity = await this.dbContext
				.Users
				.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

			if (entity is not null)
			{
				return new CreateUserPayload(entity);
			}

			var user = new User
			           {
				           Id = Guid.NewGuid(),
				           Email = request.Email,
				           FirstName = request.FirstName,
				           LastName = request.LastName
			           };

			var entry = await this.dbContext.Users.AddAsync(user, cancellationToken);
			await this.dbContext.SaveChangesAsync(cancellationToken);

			return new CreateUserPayload(entry.Entity);
		}
	}
}