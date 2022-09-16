
using Demo.Blazor.Clarity.Server.Data;
using Demo.Blazor.Clarity.Server.Modules.Users;
using Demo.Blazor.Clarity.Server.Modules.Users.Commands;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                    {
                                                        options.UseSqlite(
                                                            builder.Configuration.GetConnectionString("Default"),
                                                            b =>
                                                            {
                                                                b.MigrationsAssembly(typeof(ApplicationDbContext)
                                                                    .Assembly.FullName);
                                                            });
                                                    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddGraphQLServer()
    .InitializeOnStartup()
    .AddQueryType()
    .AddMutationType()
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<UserMutations>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .RegisterDbContext<ApplicationDbContext>()
    .SetPagingOptions(new PagingOptions {IncludeTotalCount = true, MaxPageSize = 50})
    ;

builder.Services.AddMediatR(typeof(CreateUserCommand).Assembly);

var app = builder
    .Build()
    .InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapGraphQL();
app.MapFallbackToFile("index.html");

app.Run();

