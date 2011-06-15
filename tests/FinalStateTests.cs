using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StateMachine.Tests
{
    enum StoplightState
    {
        Green,
        Yellow,
        Red // Final State
    };

    [TestClass]
    public class FinalStateTests
    {
        private StateMachine<StoplightState> BuildStoplight(StoplightState initalState)
        {
            var light = new StateMachine<StoplightState>(initalState);
            light.Valid(StoplightState.Green, StoplightState.Yellow);
            light.Valid(StoplightState.Yellow, StoplightState.Red);

            return light;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotTransitionFromFinalState()
        {
            var light = BuildStoplight(StoplightState.Red);
            light.State = StoplightState.Yellow;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotTransitionPastFinalState()
        {
            var light = BuildStoplight(StoplightState.Green);
            light.State = StoplightState.Yellow;
            light.State = StoplightState.Red;
            light.State = StoplightState.Green;
        }

        [TestMethod]
        public void CanTransitionToFinalState()
        {
            var light = BuildStoplight(StoplightState.Green);
            light.State = StoplightState.Yellow;
            light.State = StoplightState.Red;
        }
    }

    [TestClass]
    public class OtherFinalStateTests
    {
        private static StateMachine<StoplightState> light;

        [ClassInitialize]
        public static void BuildUp(TestContext ctx)
        {
            light = new StateMachine<StoplightState>(StoplightState.Green);
            light.Valid(StoplightState.Green, StoplightState.Yellow);
            light.Valid(StoplightState.Yellow, StoplightState.Red);
        }

        
    }
}
