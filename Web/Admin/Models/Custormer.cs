namespace WebBanThuoc.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Custormer")]
    public partial class Custormer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Custormer()
        {
            Carts = new HashSet<Cart>();
            Conversations = new HashSet<Conversation>();
            Detailforums = new HashSet<Detailforum>();
            Evaluates = new HashSet<Evaluate>();
            fora = new HashSet<forum>();
            VoucherOrders = new HashSet<VoucherOrder>();
        }

        [Key]
        [StringLength(200)]
        public string uid { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(50)]
        public string adress { get; set; }

        public DateTime? birthday { get; set; }

        public bool? sex { get; set; }

        [StringLength(10)]
        public string telephone { get; set; }

        [StringLength(100)]
        public string email { get; set; }

        [StringLength(500)]
        public string password { get; set; }

        public bool? delete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Conversation> Conversations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Detailforum> Detailforums { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluate> Evaluates { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<forum> fora { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherOrder> VoucherOrders { get; set; }
    }
}
