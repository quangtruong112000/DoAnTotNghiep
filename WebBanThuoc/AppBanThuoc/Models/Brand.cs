namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Brand
    {
        public long id { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(300)]
        public string url_image { get; set; }

        public bool? delete { get; set; }
    }
}
