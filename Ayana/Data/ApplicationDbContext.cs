using Ayana.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ayana.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
 
        public ApplicationDbContext() : base(new DbContextOptions<ApplicationDbContext>())
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Cart { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public DbSet<Report> Reports { get; set; }
        //public DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }

        public virtual DbSet<ProductSales> ProductSales { get; set; }

        public DbSet<DtoRequest> DtoRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Discount>().ToTable("Discounts");
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Cart>().ToTable("Carts");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Report>().ToTable("Reports");
            //modelBuilder.Entity<Subscription>().ToTable("Subscriptions");
            modelBuilder.Entity<ProductOrder>().ToTable("ProductOrders");
            modelBuilder.Entity<ProductSales>().ToTable("ProductSales");

  
            modelBuilder.Entity<DtoRequest>().ToTable("DtoRequests");
            modelBuilder.Entity<ApplicationUser>().Property(e => e.FullName);
            modelBuilder.Entity<ApplicationUser>().Property(e => e.Id);
            modelBuilder.Entity<ApplicationUser>().Property(e => e.EmailAddress);
            modelBuilder.Entity<ApplicationUser>().Property(e => e.Password);
            modelBuilder.Entity<ApplicationUser>().Property(e => e.UserName);


            base.OnModelCreating(modelBuilder);
        }

    }
}