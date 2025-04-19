using System;

namespace CarWebSite.Models
{
	public class Payment
	{
		public int PyamentId {get; set;}
		public decimal Amount {get; set;}
		public DateTime PaymentDate {get;set;}
		public PaymentMethod Method {get; set;}
		public PaymentStatus Status {get; set;}
	}
	private enum PaymentStatus
	{
		Pendeing,
		Completed
	}
	private enum PaymentMethod
	{
		Cash,
		CreditCard,
		PayPal
	}
}

