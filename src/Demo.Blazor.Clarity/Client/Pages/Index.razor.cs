// ----------------------------------------------------------------------------------------
//  <copyright file="Index.razor.cs" company="Cortado Holding">
//     Copyright (c) 2023, Cortado Holding. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------------------

namespace Demo.Blazor.Clarity.Client.Pages;

using System.Runtime.CompilerServices;
using Blazority;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public partial class Index
{
	[Inject]
	private ILogger<Index>? Logger { get; set; }

	[Inject]
	private IUserService? UserService { get; set; }

	private DatagridPagination<IUser_User_Edges>? Paginator = new();

	private Datagrid<IUser_User_Edges>? datagrid = new();

	private IUser_User? connection =
		new User_User_UserConnection(0, new User_User_PageInfo_PageInfo(false, false, null, null), null);

	private PagedQuery<IUser_User_Edges>? currentQuery = new PagedQuery<IUser_User_Edges>();

	private bool createUserDialogOpen;

	private bool deleteUserDialogOpen;

	private CreateUserModel createUserModel = new();
	
	private DeleteUserModel deleteUserModel = new();

	private IUser_User_Edges? selectedUser;

	public async Task OnImportAsync(InputFileChangeEventArgs e)
	{
		await Task.CompletedTask;
	}

	private async Task OnCreateUserOpen()
	{
		this.createUserModel = new();
		this.createUserDialogOpen = true;
		await Task.CompletedTask;
	}

	private async Task OnCreateUserCancel()
	{
		this.createUserModel = new();
		this.createUserDialogOpen = false;
		await Task.CompletedTask;
	}

	private async Task OnCreateUserSubmit()
	{
		this.createUserDialogOpen = false;

		await this.UserService.CreateUserAsync(
			this.createUserModel.Email,
			this.createUserModel.FirstName,
			this.createUserModel.LastName);

		await this.ReloadItems();
	}

	private async Task OnDeleteUserOpen()
	{
		if (this.selectedUser == null)
		{
			return;
		}
		
		this.deleteUserModel = new();
		this.deleteUserModel.Id = this.selectedUser.Node.Id;
		this.deleteUserModel.Email = this.selectedUser.Node.Email;
		this.deleteUserDialogOpen = true;
		await Task.CompletedTask;
	}

	private async Task OnDeleteUserCancel()
	{
		this.deleteUserModel = new();
		this.deleteUserDialogOpen = false;
		await Task.CompletedTask;
	}

	private async Task OnDeleteUserSubmit()
	{
		this.deleteUserDialogOpen = false;

		await this.UserService
			.DeleteUserAsync(this.deleteUserModel.Id);

		await this.ReloadItems();
	}

	private async Task ReloadItems()
	{
		if (this.datagrid != null)
		{
			await this.datagrid.RefreshViewItems(true);
		}
	}

	private async Task<PagedResult<IUser_User_Edges>> QueryItems(PagedQuery<IUser_User_Edges> query)
	{
		var queryVariables = new UserQueryVariables();

		// paging
		if (query.Skip == this.currentQuery.Skip + query.Limit)
		{
			// forward
			queryVariables.After = this.connection.PageInfo.EndCursor;
			queryVariables.First = Convert.ToInt32(query.Limit);
			queryVariables.Before = null;
			queryVariables.Last = null;
		}
		else if (query.Skip == this.currentQuery.Skip - query.Limit)
		{
			// backward
			queryVariables.After = null;
			queryVariables.First = null;
			queryVariables.Before = this.connection!.PageInfo.StartCursor;
			queryVariables.Last = Convert.ToInt32(query.Limit);
		}
		else if ((query.Skip + 10) * query.Limit >= this.connection?.TotalCount && this.connection?.TotalCount != 0
		         && this.connection != null)
		{
			// last
			queryVariables.After = null;
			queryVariables.First = null;
			queryVariables.Before = null;
			queryVariables.Last = Convert.ToInt32(query.Limit);
		}

		if (this.Paginator.Pager.StartIndex <= 0)
		{
			// first
			queryVariables.After = null;
			queryVariables.First = Convert.ToInt32(query.Limit);
			queryVariables.Before = null;
			queryVariables.Last = null;
		}

		// sorting
		queryVariables.Order = new List<UserSortInput>{new UserSortInput{Email = SortEnumType.Asc}};

		this.currentQuery = query;

		this.connection = await this.UserService
			.GetUsersAsync(queryVariables);

		var pagedResult = new PagedResult<IUser_User_Edges>();
		pagedResult.Items = this.connection.Edges;
		pagedResult.Total = this.connection.TotalCount;

		return await Task.FromResult(pagedResult);
	}
}

public class CreateUserModel
{
	public string? Email { get; set; } = string.Empty;

	public string? FirstName { get; set; } = string.Empty;

	public string? LastName { get; set; } = string.Empty;
}

public class DeleteUserModel
{
	public Guid Id { get; set; }

	public string? Email { get; set; }
}
