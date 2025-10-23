using BSC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSC.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		// Tablas
		public DbSet<User> User => Set<User>();
		public DbSet<Role> Role => Set<Role>();
		public DbSet<Product> Product => Set<Product>();
		public DbSet<Inventory> Inventory => Set<Inventory>();
		public DbSet<Order> Order => Set<Order>();
		public DbSet<OrderItem> OrderItem => Set<OrderItem>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// --- Roles ---
			modelBuilder.Entity<Role>().HasData(
				new Role { RoleId = 1, Name = "Administrador" },
				new Role { RoleId = 2, Name = "Administrativo" },
				new Role { RoleId = 3, Name = "Vendedor" }
			);

			// --- Users ---
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Username)
				.IsUnique();

			modelBuilder.Entity<User>()
				.Property(u => u.Username)
				.IsRequired()
				.HasMaxLength(50);

			modelBuilder.Entity<User>()
				.Property(u => u.PasswordHash)
				.IsRequired();

			modelBuilder.Entity<User>()
				.HasOne(u => u.Role)
				.WithMany(r => r.Users)
				.HasForeignKey(u => u.RoleId)
				.OnDelete(DeleteBehavior.Restrict);

			// --- Products ---
			modelBuilder.Entity<Product>()
				.HasIndex(p => p.SKU)
				.IsUnique();

			modelBuilder.Entity<Product>()
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(250);

			modelBuilder.Entity<Product>()
				.HasOne(p => p.Inventory)
				.WithOne(i => i.Product)
				.HasForeignKey<Inventory>(i => i.ProductId);

			// --- Inventory ---
			modelBuilder.Entity<Inventory>()
				.Property(i => i.Quantity)
				.IsRequired();

			// --- Orders ---
			modelBuilder.Entity<Order>()
				.HasOne(o => o.Seller)
				.WithMany(u => u.OrdersPlaced)
				.HasForeignKey(o => o.SellerUserId)
				.OnDelete(DeleteBehavior.Restrict);

			// --- OrderItems ---
			modelBuilder.Entity<OrderItem>()
				.HasOne(oi => oi.Order)
				.WithMany(o => o.Items)
				.HasForeignKey(oi => oi.OrderId);

			modelBuilder.Entity<OrderItem>()
				.HasOne(oi => oi.Product)
				.WithMany(p => p.OrderItems)
				.HasForeignKey(oi => oi.ProductId);

			// -- Triggers --
			modelBuilder.Entity<Inventory>()
				.ToTable(tb => tb.HasTrigger("trg_InventoryAfterUpdate"));
		}
	}
}
