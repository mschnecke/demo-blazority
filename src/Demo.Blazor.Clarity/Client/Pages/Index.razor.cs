// ----------------------------------------------------------------------------------------
//  <copyright file="Index.razor.cs" company="Cortado Holding">
//     Copyright (c) 2022, Cortado Holding. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------------------

namespace Demo.Blazor.Clarity.Client.Pages;

using Blazority;
using Blazority.Shared;
using Microsoft.AspNetCore.Components;

public partial class Index
{
	[Inject]
	private ILogger<Index>? Logger { get; set; }

	[Inject]
	private IUserService? UserService { get; set; }

	private DatagridPagination<IUser_User_Edges>? Paginator = new();

	private Datagrid<IUser_User_Edges>? Datagrid { get; set; } = new();

	private IUser_User? connection;

	private PagedQuery<IUser_User_Edges>? currentQuery = new PagedQuery<IUser_User_Edges>();

	private IUser_User_Edges SelectedItem { get; set; }

	private bool DeleteDisabled => this.SelectedItem == null;

	private bool createUserDialogOpen;

	private CreateUserModel createUserModel = new();

	private async Task OnCreateUserOpen()
	{
		await Task.Yield();
		this.createUserModel = new();
		this.createUserDialogOpen = true;
	}

	private async Task OnCreateUserCancel()
	{
		await Task.Yield();
		this.createUserModel = new();
		this.createUserDialogOpen = false;
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

	public async Task ReloadItems()
	{
		if (this.Datagrid != null)
		{
			await this.Datagrid.RefreshViewItems(true);
		}
	}

	public async Task<PagedResult<IUser_User_Edges>> QueryItems(PagedQuery<IUser_User_Edges> query)
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
