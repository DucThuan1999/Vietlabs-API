using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Helpers;

public static class OrderLinkValidator
{
    public static async Task<string?> ValidateForCreateAsync(ApplicationDbContext context, Order order)
    {
        order.LinkedOrderCount = null;

        if (order.ParentOrderId == null)
        {
            order.LinkedOrderIndex = null;
            return null;
        }

        if (!order.LinkedOrderIndex.HasValue || order.LinkedOrderIndex.Value < 1)
        {
            return "LinkedOrderIndex is required and must be >= 1 when ParentOrderId is set.";
        }

        return await ValidateParentChildLinkAsync(context, order.ParentOrderId.Value, order.ClientId, order.LinkedOrderIndex.Value);
    }

    public static async Task<string?> ValidateForUpdateAsync(
        ApplicationDbContext context,
        Order existing,
        Order updated)
    {
        updated.LinkedOrderCount = null;

        if (updated.ParentOrderId != existing.ParentOrderId)
        {
            return "ParentOrderId cannot be changed after the order is created.";
        }

        if (updated.LinkedOrderIndex != existing.LinkedOrderIndex)
        {
            return "LinkedOrderIndex cannot be changed after the order is created.";
        }

        if (updated.ParentOrderId == null)
        {
            updated.LinkedOrderIndex = null;
            return null;
        }

        if (!updated.LinkedOrderIndex.HasValue || updated.LinkedOrderIndex.Value < 1)
        {
            return "LinkedOrderIndex is required and must be >= 1 when ParentOrderId is set.";
        }

        return await ValidateParentChildLinkAsync(
            context,
            updated.ParentOrderId.Value,
            updated.ClientId,
            updated.LinkedOrderIndex.Value,
            updated.OrderId);
    }

    private static async Task<string?> ValidateParentChildLinkAsync(
        ApplicationDbContext context,
        Guid parentOrderId,
        Guid clientId,
        int linkedOrderIndex,
        Guid? excludeOrderId = null)
    {
        var parent = await context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == parentOrderId);

        if (parent == null)
        {
            return $"Parent order with ID {parentOrderId} does not exist.";
        }

        if (parent.ParentOrderId != null)
        {
            return "ParentOrderId must reference a root order (only one parent-child level is allowed).";
        }

        if (parent.ClientId != clientId)
        {
            return "Child order must have the same ClientId as its parent order.";
        }

        var duplicateIndex = await context.Orders.AnyAsync(o =>
            o.ParentOrderId == parentOrderId
            && o.LinkedOrderIndex == linkedOrderIndex
            && (!excludeOrderId.HasValue || o.OrderId != excludeOrderId.Value));

        if (duplicateIndex)
        {
            return $"LinkedOrderIndex {linkedOrderIndex} already exists for this parent order.";
        }

        return null;
    }
}
