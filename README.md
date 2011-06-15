# statemachine

> A simple, templated state machine class for c#

## Example ##

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

            // Enter transition events
            Status.DoWhenAny((status) =>
            {
                // Useful for writing a transition history log
                Debug.WriteLine("[{0}] Order Status => {1}", (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds, status);
            });

            // Or DoFollowingAny() for exit transitions
        }
    }

## Dynamic Events ##
	
If you're using .net 4.0, you can use dynamic binding in the style of rails:

	Status.Transitions.Enter_Shipped((Action)delegate()
    {
		Debug.WriteLine("Order Shipped!");
    });

	Status.Transitions.Exit_Ordered((Action)delegate()
    {
		Debug.WriteLine("Order Processed!");
    });