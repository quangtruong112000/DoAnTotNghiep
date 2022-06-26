namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evaluate")]
    public partial class Evaluate
    {
        public int id { get; set; }

        [StringLength(200)]
        public string uid { get; set; }

        public int? point { get; set; }

        public string Message { get; set; }

        public int? idproduct { get; set; }

        public int? idorderdetail { get; set; }

        public virtual Custormer Custormer { get; set; }

        public virtual Product Product { get; set; }

        public virtual VoucherOrderDetail VoucherOrderDetail { get; set; }
    }
}
