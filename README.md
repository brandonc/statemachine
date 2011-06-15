# statemachine

> A simple, templated state machine class for c#

## Example ##

    public enum DoorState
    {
        Opened,
        Closed
    };

    public enum HeaterState
    {
        Toasting,
        Baking,
        Off
    }

    // StateMachine<T> can be used as a superclass or not.
    public class ToasterDoor : StateMachine<DoorState>
    {
        public ToasterDoor()
            : base(DoorState.Closed) /* initial state */
        {
            // Both Closed -> Open and Open -> Closed are valid
            ValidTwoWay(DoorState.Closed, DoorState.Opened);

            // Per-state Exit and Enter events
            Exit(DoorState.Opened, () => { Console.WriteLine("Door is closing"); });
            Enter(DoorState.Opened, () => { Console.WriteLine("Door is opening"); });
        }
    }

    public class HeatingElement : StateMachine<HeaterState>
    {
        public HeatingElement(ToasterOven toaster)
            : base(HeaterState.Off) /* initial state */
        {
            // Both Off -> Baking and Off -> Toasting are valid
            Valid(HeaterState.Off, new[] { HeaterState.Baking, HeaterState.Toasting });

            // Both Toasting -> Off and Baking -> Off are valid
            Valid(new[] { HeaterState.Toasting, HeaterState.Baking }, HeaterState.Off);

            Exit(HeaterState.Off, () => { Console.WriteLine("Heating Element Warming Up"); });
            Enter(HeaterState.Baking, () => { Console.WriteLine("Oven Mode: " + toaster.Temperature + " degrees"); });
            Enter(HeaterState.Toasting, () => { Console.WriteLine("Toaster Mode"); });
        }
    }

    public class ToasterOven
    {
        private HeatingElement heater;
        private ToasterDoor door;

        public int Temperature { get; set; }

        public void Toast()
        {
            heater.State = HeaterState.Toasting;
        }

        public void Bake(int temp)
        {
            Temperature = temp;
            heater.State = HeaterState.Baking;
        }

        public void Open()
        {
            door.State = DoorState.Opened;
        }

        public void Close()
        {
            door.State = DoorState.Closed;
        }

        public void Off()
        {
            heater.State = HeaterState.Off;
        }

        public ToasterOven()
        {
            heater = new HeatingElement(this);
            door = new ToasterDoor();
        }

        public void ThrowsArgumentException()
        {
            heater.State = HeaterState.Baking;
            heater.State = HeaterState.Toasting;
        }
    }