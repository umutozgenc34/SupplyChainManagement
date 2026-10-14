using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Tests.Entities;

public class OrderTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Approved, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Canceled, true)]
    [InlineData(OrderStatus.Approved, OrderStatus.Shipped, true)]
    [InlineData(OrderStatus.Approved, OrderStatus.Canceled, true)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Pending, OrderStatus.Delivered, false)]
    [InlineData(OrderStatus.Pending, OrderStatus.Pending, false)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Canceled, false)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Canceled, false)]
    [InlineData(OrderStatus.Canceled, OrderStatus.Pending, false)]
    public void CanTransitionTo_FollowsOrderWorkflow(OrderStatus from, OrderStatus to, bool expected)
    {
        var order = new Order { Status = from };

        Assert.Equal(expected, order.CanTransitionTo(to));
    }

    [Theory]
    [InlineData(OrderStatus.Pending, true, true)]
    [InlineData(OrderStatus.Approved, false, false)]
    [InlineData(OrderStatus.Shipped, false, false)]
    [InlineData(OrderStatus.Delivered, false, false)]
    [InlineData(OrderStatus.Canceled, false, true)]
    public void EditableAndDeletable_DependOnStatus(OrderStatus status, bool editable, bool deletable)
    {
        var order = new Order { Status = status };

        Assert.Equal(editable, order.IsEditable);
        Assert.Equal(deletable, order.IsDeletable);
    }
}
