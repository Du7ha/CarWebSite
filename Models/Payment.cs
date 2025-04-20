using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentId { get; set; }  

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; private set; }

    [Required]
    public DateTime PaymentDate { get; private set; }

    [Required]
    public PaymentMethod Method { get; private set; }

    [Required]
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "EGP";

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; private set; }

    // Navigation property
    public virtual Order? Order { get; set; }

    public Payment(decimal amount, PaymentMethod method, int orderId)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));

        if (orderId <= 0)
            throw new ArgumentException("Valid OrderId must be provided", nameof(orderId));

        Amount = amount;
        Method = method;
        OrderId = orderId;
        PaymentDate = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Payment is already completed");

        Status = PaymentStatus.Completed;
    }

    public bool IsValid()
    {
        return Amount > 0 &&
               PaymentDate <= DateTime.UtcNow &&
               OrderId > 0;
    }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    PayPal,
    BankTransfer,
    Cryptocurrency
}
}
