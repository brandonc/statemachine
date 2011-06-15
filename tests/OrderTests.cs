using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

namespace StateMachine.Tests
{
    public enum OrderState
    {
        Created,
        Ordered,
        Cancelled,
        Preparing,
        Shipped
    };

    public class Order
    {
        public StateMachine<OrderState> Status { get; set; }

        public void Complete()
        {
            this.Status.State = OrderState.Ordered;
        }

        public void Cancel()
        {
            this.Status.State = OrderState.Cancelled;
        }

        public void Prepare()
        {
            this.Status.State = OrderState.Preparing;
        }

        public void Ship()
        {
            this.Status.State = OrderState.Shipped;
        }

        public Order()
        {
            // You could also subclass StateMachine<T> and encapsulate these details
            Status = new StateMachine<OrderState>(OrderState.Created);
            
            // Only transitions defined here are valid. Alternatively, you could call
            // ValidAny() to allow all state transitions
            Status.Valid(OrderState.Created,   OrderState.Ordered);
            Status.Valid(OrderState.Ordered,   new[] { OrderState.Cancelled, OrderState.Preparing });
            Status.Valid(OrderState.Preparing, OrderState.Shipped);

            Status.Transitions.Enter_Shipped((Action)delegate()
            {
                Debug.WriteLine("Order Shipped!");
            });

            // Enter transition events
            Status.DoWhenAny((status) =>
            {
                // Useful for writing a transition history log
                Debug.WriteLine("[{0}] Order Status => {1}", (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds, status);
            });

            // Or DoFollowingAny() for exit transitions
        }
    }

    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        public void CanCancelOrder()
        {
            Order o = new Order();
            o.Complete();
            o.Cancel();
        }

        [TestMethod]
        public void CanShipOrder()
        {
            Order o = new Order();
            o.Complete();
            o.Prepare();
            o.Ship();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotCancelPreparedOrder()
        {
            Order o = new Order();
            o.Complete();
            o.Prepare();
            o.Cancel();
        }
    }
}
