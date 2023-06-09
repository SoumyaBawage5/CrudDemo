using CrudDemo.Data;
using CrudDemo.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
internal class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<CrudDemoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ContactsCS")));

        builder.Services.AddOptions<AwsConfig>()
            .Bind(builder.Configuration.GetSection("AwsConfig"));
        var optionsBuilder = new DbContextOptionsBuilder<CrudDemoDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();
        //builder.Services.AddControllers().AddNewtonsoftJson(); // or .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null) for System.Text.Json


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





    }
}