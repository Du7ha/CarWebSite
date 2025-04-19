using System;

namespace CarWebSite.Models
{
	public class Payment
	{
		public int PyamentId {get; set;}
		public decimal Amount {get; private set;}
		public DateTime PaymentDate {get; private set;}
		public PaymentMethod Method {get; private set;}
		public PaymentStatus Status {get; private set;} = PaymentStatus.Pendeing;
    	public string Currency { get; set; } = "EGP";
		public int OrderId {get; private set;}
		public virtual Order Order {get; set;}


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