// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);
builder.AddObservable();

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<TscCaller>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.WebHost.UseKestrel(option =>
{
    option.ConfigureHttpsDefaults(options =>
    options.ServerCertificate = new X509Certificate2(Path.Combine("Certificates", "7348307__lonsid.cn.pfx"), "cqUza0MN"));
});

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddMasaIdentityModel(IdentityType.MultiEnvironment, options =>
{
    options.Environment = "environment";
    options.UserName = "name";
    options.UserId = "sub";
})
.AddMasaStackComponentsForServer("wwwroot/i18n", builder.Configuration["Masa:Auth:ServiceBaseAddress"], builder.Configuration["Masa:Mc:ServiceBaseAddress"])
.AddMasaOpenIdConnect(builder.Configuration.GetSection("Masa:OIDC").Get<MasaOpenIdConnectOptions>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();