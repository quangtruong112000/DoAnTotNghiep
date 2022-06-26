namespace WebBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Employer")]
    public partial class Employer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employer()
        {
            Conversations = new HashSet<Conversation>();
            VoucherOrders = new HashSet<VoucherOrder>();
        }

        [Key]
        [StringLength(200)]
        public string uid { get; set; }

        [StringLength(200)]
        public string name { get; set; }

        [StringLength(10)]
        public string telephone { get; set; }

        [StringLength(100)]
        public string email { get; set; }

        [StringLength(100)]
        public string position { get; set; }

        public bool? delete { get; set; }

        [StringLength(500)]
        public string password { get; set; }

        [Column(TypeName = "date")]
        public DateTime? birthday { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Conversation> Conversations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherOrder> VoucherOrders { get; set; }
    }
}
