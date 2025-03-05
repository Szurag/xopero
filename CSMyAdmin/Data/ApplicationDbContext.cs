using Microsoft.EntityFrameworkCore;

namespace CSMyAdmin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options);