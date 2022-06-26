namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        [StringLength(200)]
        public string UseName { get; set; }

        [StringLength(200)]
        public string Passoword { get; set; }

        [Required]
        [StringLength(300)]
        public string Uid { get; set; }

        public long id { get; set; }
    }
}
