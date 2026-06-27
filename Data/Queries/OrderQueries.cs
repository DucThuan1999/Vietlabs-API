using VietLab.Models;

namespace VietLab.Data.Queries;

public static class OrderQueries
{
    public static IQueryable<Order> WithLinkedOrderCount(this IQueryable<Order> orders, ApplicationDbContext context)
    {
        return orders.Select(o => new Order
        {
            OrderId = o.OrderId,
            ClientId = o.ClientId,
            ParentOrderId = o.ParentOrderId,
            LinkedOrderIndex = o.LinkedOrderIndex,
            LinkedOrderCount = o.ParentOrderId == null
                ? context.Orders.Count(c => c.ParentOrderId == o.OrderId)
                : null,
            ContactId = o.ContactId,
            CustomerCode = o.CustomerCode,
            CustomerName = o.CustomerName,
            CustomerTaxCode = o.CustomerTaxCode,
            AgentName = o.AgentName,
            CustomerAddress = o.CustomerAddress,
            SampleSenderName = o.SampleSenderName,
            SampleSenderEmail = o.SampleSenderEmail,
            SampleSenderPhone = o.SampleSenderPhone,
            PayerName = o.PayerName,
            PayerEmail = o.PayerEmail,
            PayerPhone = o.PayerPhone,
            IssueInvoice = o.IssueInvoice,
            IsApproved = o.IsApproved,
            RejectionNote = o.RejectionNote,
            ApprovalNote = o.ApprovalNote,
            QuotationId = o.QuotationId,
            CreatedAt = o.CreatedAt,
            CreatedBy = o.CreatedBy,
            ExpectedCompletionDate = o.ExpectedCompletionDate,
            DebtType = o.DebtType,
            DebtStatus = o.DebtStatus,
            OrderStatus = o.OrderStatus,
            TestingPurpose = o.TestingPurpose,
            TestMethod = o.TestMethod,
            ResultTurnaroundTimeRequirement = o.ResultTurnaroundTimeRequirement,
            ResultDeliveryChannel = o.ResultDeliveryChannel,
            Language = o.Language,
            Technique = o.Technique,
            ComparisonStandard = o.ComparisonStandard,
            Total = o.Total,
            SubtotalBeforeVat = o.SubtotalBeforeVat,
            Vat = o.Vat,
            PaymentAmount = o.PaymentAmount,
            SampleReceiptMethod = o.SampleReceiptMethod,
            LaboratorySampleRetention = o.LaboratorySampleRetention,
            AdditionalInformation = o.AdditionalInformation,
            TestRequestConfirmation = o.TestRequestConfirmation,
            MailDocumentConfirmation = o.MailDocumentConfirmation,
            PaymentMethod = o.PaymentMethod,
            Client = o.Client,
            Contact = o.Contact,
            Quotation = o.Quotation,
            CreatedByAccount = o.CreatedByAccount,
            ParentOrder = o.ParentOrder
        });
    }
}
