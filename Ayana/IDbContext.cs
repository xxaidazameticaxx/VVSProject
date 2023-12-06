using Ayana.Models;
using Microsoft.EntityFrameworkCore;

namespace Ayana
{
    public interface IDbContext
    {
        DbSet<Cart> Cart { get; set; }
    }
}
