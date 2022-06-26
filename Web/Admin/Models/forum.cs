namespace WebBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("forum")]
    public partial class forum
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public forum()
        {
            Detailforums = new HashSet<Detailforum>();
        }

        public int Id { get; set; }

        [StringLength(200)]
        public string uid { get; set; }

        public string Message { get; set; }

        public DateTime? Time { get; set; }

        public string image { get; set; }

        public virtual Custormer Custormer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Detailforum> Detailforums { get; set; }
    }
}
