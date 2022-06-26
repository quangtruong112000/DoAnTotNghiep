namespace AppBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Conversation")]
    public partial class Conversation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        [StringLength(200)]
        public string idcustormer { get; set; }

        [StringLength(200)]
        public string idemployer { get; set; }

        public DateTime? createDate { get; set; }

        public string message { get; set; }

        public virtual Custormer Custormer { get; set; }

        public virtual Employer Employer { get; set; }
    }
}
