using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace StateMachine.Tests
{
    [TestClass]
    public class ToasterOvenTests
    {
        private ToasterOven toaster = new ToasterOven();

        [TestMethod]
        public void CanOpenClose()
        {
            WakeUpInTheMorning();
            toaster.Open();
            toaster.Close();
            WakeUpInTheMorning();
            toaster.Open();
            toaster.Open(); // Nothing should happen.
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotToastAndBake()
        {
            WakeUpInTheMorning();
            toaster.Bake(300);
            toaster.Toast();
        }

        [TestMethod]
        public void CanTurnOnOff()
        {
            WakeUpInTheMorning();
            toaster.Off();
            toaster.Toast();
            toaster.Off();
        }

        public void WakeUpInTheMorning()
        {
            toaster.Sunrise();
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

            DoFollowing(DoorState.Opened, () => { Debug.WriteLine("Door is closing"); });
            DoWhen(DoorState.Opened, () => { Debug.WriteLine("Door is opening"); });
        }
    }

    public class HeatingElement : StateMachine<HeaterState>
    {
        public HeatingElement(ToasterOven toaster)
            : base(HeaterState.Off) /* initial state */
        {
            Valid(HeaterState.Off, new[] { HeaterState.Baking, HeaterState.Toasting });
            Valid(new[] { HeaterState.Toasting, HeaterState.Baking }, HeaterState.Off);

            DoFollowing(HeaterState.Off, () => { Debug.WriteLine("Heating Element Warming Up"); });
            DoWhenAny((mode) => {
                switch (mode)
                {
                    case HeaterState.Baking:
                        Debug.WriteLine("Baking at " + toaster.Temperature + " degrees");
                        break;
                    case HeaterState.Toasting:
                        Debug.WriteLine("Toasting");
                        break;
                    default:
                        // NOP
                        break;
                }
            });
        }
    }

    public class ToasterOven
    {
        private HeatingElement heater;
        private ToasterDoor door;
        private StateMachine<DayOfWeek> today;

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

        public void Sunrise()
        {
            if (today.State == DayOfWeek.Saturday)
                today.State = DayOfWeek.Sunday;
            else
                today.State = (DayOfWeek)(int)today.State + 1;
        }

        public ToasterOven()
        {
            heater = new HeatingElement(this);
            door = new ToasterDoor();
            today = new StateMachine<DayOfWeek>(DayOfWeek.Monday);
            today.ValidAny();
            today.DoWhenAny((dow) => { Debug.WriteLine("Good morning! Today is " + dow); });
        }
    }
}
