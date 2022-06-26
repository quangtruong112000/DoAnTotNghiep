using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebBanThuoc.Models
{
    public partial class WebBanThuocDB : DbContext
    {
        public WebBanThuocDB()
            : base("name=WebBanThuocDB")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Custormer> Custormers { get; set; }
        public virtual DbSet<Detailforum> Detailforums { get; set; }
        public virtual DbSet<Employer> Employers { get; set; }
        public virtual DbSet<Evaluate> Evaluates { get; set; }
        public virtual DbSet<forum> fora { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<VoucherOrder> VoucherOrders { get; set; }
        public virtual DbSet<VoucherOrderDetail> VoucherOrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Passoword)
                .IsFixedLength();

            modelBuilder.Entity<Cart>()
                .Property(e => e.intomoney)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Category)
                .HasForeignKey(e => e.category_id);

            modelBuilder.Entity<Custormer>()
                .Property(e => e.telephone)
                .IsFixedLength();

            modelBuilder.Entity<Custormer>()
                .HasMany(e => e.Conversations)
                .WithOptional(e => e.Custormer)
                .HasForeignKey(e => e.idcustormer);

            modelBuilder.Entity<Employer>()
                .Property(e => e.telephone)
                .IsFixedLength();

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.Conversations)
                .WithOptional(e => e.Employer)
                .HasForeignKey(e => e.idemployer);

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.VoucherOrders)
                .WithOptional(e => e.Employer)
                .HasForeignKey(e => e.approvedby);

            modelBuilder.Entity<forum>()
                .HasMany(e => e.Detailforums)
                .WithOptional(e => e.forum)
                .HasForeignKey(e => e.idforum);

            modelBuilder.Entity<Product>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Carts)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Evaluates)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.idproduct);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Photos)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.VoucherOrderDetails)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<VoucherOrder>()
                .Property(e => e.telephone)
                .IsFixedLength();

            modelBuilder.Entity<VoucherOrder>()
                .Property(e => e.grossAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VoucherOrder>()
                .Property(e => e.discountAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VoucherOrder>()
                .Property(e => e.shiper)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VoucherOrder>()
                .HasMany(e => e.VoucherOrderDetails)
                .WithRequired(e => e.VoucherOrder)
                .HasForeignKey(e => e.voucherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<VoucherOrderDetail>()
                .Property(e => e.grossAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VoucherOrderDetail>()
                .Property(e => e.discountAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<VoucherOrderDetail>()
                .HasMany(e => e.Evaluates)
                .WithOptional(e => e.VoucherOrderDetail)
                .HasForeignKey(e => e.idorderdetail);
        }
    }
}
