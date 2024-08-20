using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Rewrite;
using SuggestionSiteUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Will use Azure auth
app.UseAuthentication();
app.UseAuthorization();

// Overwrite logout redirect
app.UseRewriter(new RewriteOptions().Add(context =>
{
    if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/SignedOut")
        context.HttpContext.Response.Redirect("/");
}));

app.MapControllers();   // for azure auth

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
