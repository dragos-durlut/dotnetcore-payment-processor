using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PaymentProcessor.Models.Entity
{
    [Table("PaymentState", Schema ="dbo")]
    public class PaymentState
    {
        [Required]
        [Key]        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaymentStateId { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Column(nameof(CreatedDate), TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public long PaymentId { get; set; }
        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; }
    }
}
