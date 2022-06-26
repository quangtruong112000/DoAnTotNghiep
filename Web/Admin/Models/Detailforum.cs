namespace WebBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Detailforum")]
    public partial class Detailforum
    {
        public int id { get; set; }

        [StringLength(200)]
        public string uid { get; set; }

        public string Message { get; set; }

        public int? idforum { get; set; }

        public DateTime? Time { get; set; }

        public string image { get; set; }

        public virtual Custormer Custormer { get; set; }

        public virtual forum forum { get; set; }
    }
}
