using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StateMachine.Tests
{
    [TestClass]
    public class ToasterOvenTests
    {
        private ToasterOven toaster = new ToasterOven();

        [TestMethod]
        public void CanOpenClose()
        {
            toaster.Open();
            toaster.Close();
            toaster.Open();
            toaster.Open(); // Nothing should happen.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CantToastAndBake()
        {
            toaster.Bake(300);
            toaster.Toast();
        }

        [TestMethod]
        public void CanTurnOff()
        {
            toaster.Off();
            toaster.Toast();
            toaster.Off();
        }
    }

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

    public class ToasterDoor : StateMachine<DoorState>
    {
        public ToasterDoor()
            : base(DoorState.Closed) /* initial state */
        {
            ValidTwoWay(DoorState.Closed, DoorState.Opened);

            Exit(DoorState.Opened, () => { Console.WriteLine("Door is closing"); });
            Enter(DoorState.Opened, () => { Console.WriteLine("Door is opening"); });
        }
    }

    public class HeatingElement : StateMachine<HeaterState>
    {
        public HeatingElement(ToasterOven toaster)
            : base(HeaterState.Off) /* initial state */
        {
            Valid(HeaterState.Off, new[] { HeaterState.Baking, HeaterState.Toasting });
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
    }
}
