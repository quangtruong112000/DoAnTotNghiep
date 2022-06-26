namespace WebBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VoucherOrderDetail")]
    public partial class VoucherOrderDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VoucherOrderDetail()
        {
            Evaluates = new HashSet<Evaluate>();
        }

        public long voucherId { get; set; }

        public int id { get; set; }

        public int? product_id { get; set; }

        public decimal? grossAmount { get; set; }

        public decimal? discountAmount { get; set; }

        public int? quantity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluate> Evaluates { get; set; }

        public virtual Product Product { get; set; }

        public virtual VoucherOrder VoucherOrder { get; set; }
    }
}
