using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;

namespace CashFlow.Domain.Extensions;
public static class PaymentTypeExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => ResourceReportPaymentType.CASH,
            PaymentType.CreditCard => ResourceReportPaymentType.CREDIT_CARD,
            PaymentType.DebitCard => ResourceReportPaymentType.DEBIT_CARD,
            PaymentType.EletronicTransfer => ResourceReportPaymentType.ELETRONIC_TRANSFER,
            _ => string.Empty
        };
    }
}
