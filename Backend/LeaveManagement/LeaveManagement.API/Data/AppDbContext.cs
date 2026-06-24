using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role>         Roles         { get; set; }
    public DbSet<User>         Users         { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()         .ToTable("roles");
        modelBuilder.Entity<User>()         .ToTable("users");
        modelBuilder.Entity<LeaveRequest>() .ToTable("leave_requests");

        modelBuilder.Entity<Role>(e => {
            e.Property(r => r.Id)        .HasColumnName("id");
            e.Property(r => r.RoleName)  .HasColumnName("role_name");
            e.Property(r => r.CreatedAt) .HasColumnName("created_at");
        });

        modelBuilder.Entity<User>(e => {
            e.Property(u => u.Id)            .HasColumnName("id");
            e.Property(u => u.FirstName)     .HasColumnName("first_name");
            e.Property(u => u.LastName)      .HasColumnName("last_name");
            e.Property(u => u.Email)         .HasColumnName("email");
            e.Property(u => u.PasswordHash)  .HasColumnName("password_hash");
            e.Property(u => u.RoleId)        .HasColumnName("role_id");
            e.Property(u => u.Department)    .HasColumnName("department");
            e.Property(u => u.Designation)   .HasColumnName("designation");
            e.Property(u => u.DateOfJoining) .HasColumnName("date_of_joining");
            e.Property(u => u.IsActive)      .HasColumnName("is_active");
            e.Property(u => u.CreatedAt)     .HasColumnName("created_at");
            e.Property(u => u.UpdatedAt)     .HasColumnName("updated_at");
        });

        modelBuilder.Entity<LeaveRequest>(e => {
            e.Property(l => l.Id)         .HasColumnName("id");
            e.Property(l => l.EmployeeId) .HasColumnName("employee_id");
            e.Property(l => l.FromDate)   .HasColumnName("from_date");
            e.Property(l => l.ToDate)     .HasColumnName("to_date");
            e.Property(l => l.Reason)     .HasColumnName("reason");
            e.Property(l => l.Status)     .HasColumnName("status")
                                          .HasConversion<string>();
            e.Property(l => l.ReviewedBy) .HasColumnName("reviewed_by");
            e.Property(l => l.ReviewedAt) .HasColumnName("reviewed_at");
            e.Property(l => l.Remarks)    .HasColumnName("remarks");
            e.Property(l => l.CreatedAt)  .HasColumnName("created_at");
            e.Property(l => l.UpdatedAt)  .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(u => u.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Reviewer)
            .WithMany()
            .HasForeignKey(l => l.ReviewedBy)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.RoleName).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = "Admin",    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = 2, RoleName = "Employee", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
