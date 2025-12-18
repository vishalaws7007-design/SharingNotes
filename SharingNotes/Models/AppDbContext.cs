using Microsoft.EntityFrameworkCore;

namespace SharingNotes.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        public DbSet<SignupViewModel> signupViewModels {  get; set; }
    }
}
