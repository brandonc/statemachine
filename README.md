# statemachine

> A simple, templated state machine class for c#

## Example ##

    public enum Door
    {
      Opened,
      Closed
    };

    public enum Heater
    {
      Toasting,
      Baking,
      Off
    }

    public class ToasterDoor : StateMachine<Door>
    {
      public ToasterDoor()
        : base(Door.Closed)
      {
        ValidTwoWay(Door.Closed, Door.Opened);

        Exit(Door.Opened, () => { Console.WriteLine("Door is closing"); });
        Enter(Door.Opened, () => { Console.WriteLine("Door is opening"); });
      }
    }

    public class HeatingElement : StateMachine<Heater>
    {
      public HeatingElement()
        : base(Heater.Off)
      {
        Valid(Heater.Off, new [] { Heater.Baking, Heater.Toasting });
        Valid(new [] { Heater.Toasting, Heater.Baking }, Heater.Off);

        Exit(Heater.Off, () => { Console.WriteLine("Heating Element Warming Up"); });
        Enter(Heater.Baking, () => { Console.WriteLine("Oven Mode"); });
        Enter(Heater.Toasting, () => { Console.WriteLine("Toaster Mode"); });
      }
    }

    public class ToasterOven
    {
      private HeatingElement heater;
      private ToasterDoor door;

      public int Temperature { get; set; }

      public void Toast()
      {
        heater.State = Heater.Toasting;
      }

      public void Bake(int temp)
      {
        Temperature = temp;
        heater.State = Heater.Baking;
      }

      public void Open()
      {
        door.State = Door.Opened;
      }

      public void Close()
      {
        door.State = Door.Closed;
      }

      public void Off()
      {
        heater.State = Heater.Off;
      }

      public ToasterOven()
      {
        // Initial state
        heater = new HeatingElement();
        door = new ToasterDoor();
      }
    }

    class Program {
      static void Main(string[] args)
      {
        ToasterOven toaster = new ToasterOven();
        toaster.Open();
        toaster.Close();
        toaster.Bake(300);
        toaster.Off();
      }
    }

