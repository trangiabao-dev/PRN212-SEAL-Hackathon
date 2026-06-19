using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PRN212_SEAL.Entities;

public partial class PRN212SealDbContext
{
    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return config["ConnectionStrings:DefaultConnection"]
            ?? throw new InvalidOperationException(
                "Không tìm thấy ConnectionStrings:DefaultConnection trong appsettings.json.");
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

}
