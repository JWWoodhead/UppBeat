using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Uppbeat.Api.Data;

public class UppbeatDbContext : IdentityDbContext<UppbeatUser>
{
    public UppbeatDbContext(DbContextOptions<UppbeatDbContext> options)
    : base(options)
    {
    }
}
