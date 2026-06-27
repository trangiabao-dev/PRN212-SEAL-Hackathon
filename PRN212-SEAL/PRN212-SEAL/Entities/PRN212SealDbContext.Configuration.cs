using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PRN212_SEAL.Entities;

public partial class PRN212SealDbContext
{
    private string GetConnectionString()
    {
        try
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string? conn = config["ConnectionStrings:DefaultConnection"];
            if (!string.IsNullOrWhiteSpace(conn))
            {
                return conn;
            }
        }
        catch
        {
            // Fallback an toàn nếu đọc file JSON gặp lỗi quyền hạn hoặc không tìm thấy đường dẫn
        }

        // Chuỗi kết nối mặc định khớp 100% appsettings.json
        return "Server=(local);Database=PRN212_SEAL_DB;User Id=sa;Password=12345;TrustServerCertificate=True;";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }
}
