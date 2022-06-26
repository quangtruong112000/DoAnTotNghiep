namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cart")]
    public partial class Cart
    {
        public long id { get; set; }

        public int? product_id { get; set; }

        public int? quantity { get; set; }

        public decimal? intomoney { get; set; }

        [StringLength(200)]
        public string uid { get; set; }

        public virtual Custormer Custormer { get; set; }

        public virtual Product Product { get; set; }
    }
}
