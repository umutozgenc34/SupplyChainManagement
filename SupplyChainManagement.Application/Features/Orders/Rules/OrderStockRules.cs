using Core.Application.Responses;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Features.Orders.Rules;

public static class OrderStockRules
{
    public static ServiceResult<List<OrderItem>> CreateOrderItems(
        IEnumerable<OrderItemDto> requestedItems,
        IReadOnlyDictionary<Guid, Product> products,
        bool reserveStock)
    {
        var orderItems = new List<OrderItem>();

        foreach (var group in requestedItems.GroupBy(i => i.ProductId))
        {
            if (!products.TryGetValue(group.Key, out var product))
                return ServiceResult<List<OrderItem>>.Failure($"Product with ID {group.Key} not found.");

            var quantity = group.Sum(i => i.Quantity);

            if (reserveStock)
            {
                if (product.Quantity < quantity)
                    return ServiceResult<List<OrderItem>>.Failure(
                        $"Insufficient stock for product '{product.Name}'. Available: {product.Quantity}, requested: {quantity}.");

                product.Quantity -= quantity;
            }

            orderItems.Add(new OrderItem(product.Id, quantity, product.Price));
        }

        return ServiceResult<List<OrderItem>>.SuccessResult(orderItems);
    }

    public static void ReleaseStock(IEnumerable<OrderItem> orderItems, IReadOnlyDictionary<Guid, Product> products)
    {
        foreach (var item in orderItems)
        {
            if (products.TryGetValue(item.ProductId, out var product))
                product.Quantity += item.Quantity;
        }
    }

    public static bool HoldsStock(OrderStatus status) => status != OrderStatus.Canceled;
}
