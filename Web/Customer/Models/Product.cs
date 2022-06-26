namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Carts = new HashSet<Cart>();
            Evaluates = new HashSet<Evaluate>();
            Photos = new HashSet<Photo>();
            VoucherOrderDetails = new HashSet<VoucherOrderDetail>();
        }

        public int id { get; set; }

        [StringLength(200)]
        public string partner_code { get; set; }

        [StringLength(100)]
        public string product_brand { get; set; }

        public int? product_type { get; set; }

        [StringLength(50)]
        public string product_code { get; set; }

        [StringLength(300)]
        public string name { get; set; }

        [StringLength(300)]
        public string name_normalized { get; set; }

        public string description { get; set; }

        [StringLength(100)]
        public string unit { get; set; }

        public decimal? price { get; set; }

        public int? discount { get; set; }

        [StringLength(50)]
        public string discount_from { get; set; }

        [StringLength(50)]
        public string discount_to { get; set; }

        public int? published { get; set; }

        public bool? is_discount { get; set; }

        public int? category_id { get; set; }

        public bool? delete { get; set; }

        public int? quantity { get; set; }

        public double? weight { get; set; }

        public string attributes { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluate> Evaluates { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Photo> Photos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherOrderDetail> VoucherOrderDetails { get; set; }
    }
}
