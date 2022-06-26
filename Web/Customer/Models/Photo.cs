namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Photo
    {
        public int? product_id { get; set; }

        [StringLength(500)]
        public string url { get; set; }

        public int id { get; set; }

        public virtual Product Product { get; set; }
    }
}
