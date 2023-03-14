using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Text;
using ToDo;
using ToDo.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(
                    authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
                    configureOptions: options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
                            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Token").Value)),
                        };
                    });

// tambahkan konfigurasi Authorization
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();
else
	app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
