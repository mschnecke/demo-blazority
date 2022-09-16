namespace Demo.Blazor.Clarity.Server.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class User
{
	[Key]
	public Guid Id { get; set; }

	public string? Email { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }
}