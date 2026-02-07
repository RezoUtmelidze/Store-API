using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store_API;
using Store_API.Configurations;
using Store_API.Data;
using Store_API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof (AutoMapperConfig));

builder.Services.AddDbContext<StoreWADbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("StoreAppDbConnection"));
});
builder.Services.AddScoped<IStoreRepos, StoreRepos>();
builder.Services.AddScoped<IEmployeeRepos, EmployeeRepos>();
builder.Services.AddScoped<ICategoryRepos, CategoryRepos>();
builder.Services.AddScoped<IProductRepos, ProductRepos>();
builder.Services.AddScoped(typeof(IStoreAPIRepos<>), typeof(StoreAPIRepos<>));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;
}).AddEntityFrameworkStores<StoreWADbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
