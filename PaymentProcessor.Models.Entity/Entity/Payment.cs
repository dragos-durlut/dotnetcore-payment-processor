using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentProcessor.Models.Entity
{
    [Table("Payment", Schema = "dbo")]
    public class Payment
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PaymentId { get; set; }
        [Required]        
        public string CreditCardNumber { get; set; }
        [Required]
        public string CardHolder { get; set; }
        [Required]        
        public DateTime ExpirationDate { get; set; }        
        [Column(nameof(SecurityCode), TypeName = "nvarchar(3)")]
        public string SecurityCode { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        [Column(nameof(Amount), TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [InverseProperty(nameof(PaymentState.Payment))]
        public virtual ICollection<PaymentState> PaymentStates { get; set; }
    }
}
