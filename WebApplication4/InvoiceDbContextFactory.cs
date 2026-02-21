using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebApplication4.Data;

namespace WebApplication4;

public class InvoiceDbContextFactory : IDesignTimeDbContextFactory<InvoiceDbContext>
{
    public InvoiceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InvoiceDb;User Id=sa;Password=StrongPassw0rd!;Encrypt=True;TrustServerCertificate=True;");

        return new InvoiceDbContext(optionsBuilder.Options);
    }
}