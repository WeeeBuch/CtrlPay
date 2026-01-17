using CtrlPay.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.DB
{
    public class CtrlPayDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LoyalCustomer> LoyalCustomers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PayrollApproval> PayrollApprovals { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkRecord> WorkRecords { get; set; }

        private static DatabaseSettings? _settings;

        public static void Initialize(DatabaseSettings? settings)
        {
            _settings = settings;
        }
        // DI konstruktor
        public CtrlPayDbContext(DbContextOptions<CtrlPayDbContext> options) : base(options) {}

        public CtrlPayDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(true);
            if (optionsBuilder.IsConfigured)
                return;

            if (_settings == null)
                throw new InvalidOperationException("DbContext not initialized");

            switch (_settings.Type.ToLower())
            {
                case "mysql":
                    string connectionString = $"Server={_settings.ProviderIp};Port={_settings.ProviderPort};Database={_settings.DbName};Uid={_settings.DbName};Pwd={_settings.DbPassword};";
                    optionsBuilder.UseMySQL(connectionString);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported database type");
            }
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PayrollApproval>()
                .HasKey(pa => new { pa.ApproverId, pa.PayrollId });
        }
    }
}
