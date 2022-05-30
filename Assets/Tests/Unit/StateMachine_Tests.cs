using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using System.Diagnostics;

namespace UnitTests
{
    [TestFixture]
    public class StateMachineTests
    {
        public class MockState : IState
        {
            public bool OnEnterCalled { get; private set; }
            public bool UpdateCalled { get; private set; }
            public bool OnExitCalled { get; private set; }

            public void OnEnter() { OnEnterCalled = true; }
            public void Update() { UpdateCalled = true; }
            public virtual void OnExit() { OnExitCalled = true; }
        }

        public class StateA : MockState { }
        public class StateB : MockState { }


        public class SwitchStateOnExit : MockState
        {
            public SwitchStateOnExit(StateMachine stateMachine)
            {
                _stateMachine = stateMachine;
            }
            public override void OnExit()
            {
                base.OnExit();

                _stateMachine.SwitchState(typeof(StateA));
            }
            StateMachine _stateMachine;
        }


        [SetUp]
        public void Init()
        {
            _stateMachine = new StateMachine();
            _stateA = new StateA();
            _stateB = new StateB();


            _stateMachine.AddState(_stateA);
            _stateMachine.AddState(_stateB);
        }

        [TearDown]
        public void Cleanup()
        { /* ... */ }

        [Test]
        public void ShouldSwitchToNewState()
        {
            _stateMachine.SwitchState(_stateA.GetType());
            _stateMachine.SwitchState(_stateB.GetType());

            Assert.True(_stateA.OnExitCalled);
            Assert.True(_stateB.OnEnterCalled);
            Assert.AreEqual(_stateMachine.CurrentState, _stateB);
        }

        [Test]
        public void ShouldThrowWhenSwitchingFromOnExitOfAnotherState()
        {
            _stateMachine.AddState(new SwitchStateOnExit(_stateMachine));
            _stateMachine.SwitchState(typeof(SwitchStateOnExit));

            Assert.Throws<UnityException>(() => _stateMachine.SwitchState(typeof(StateB)));
        }

        [Test]
        public void ShouldThrowWhenSwitchingToNotExistingState()
        {
            _stateMachine.SwitchState(_stateA.GetType());

            Assert.Throws<UnityException>(() => _stateMachine.SwitchState(typeof(SwitchStateOnExit)));
        }

        [Test]
        public void ShouldNotUpdateCurrentStateIfNotRunning()
        {
            _stateMachine.SwitchState(_stateA.GetType());

            _stateMachine.Update();

            Assert.False(_stateA.UpdateCalled);
        }

        [Test]
        public void ShouldUpdateCurrentStateIfRunning()
        {
            _stateMachine.SwitchState(_stateA.GetType());
            _stateMachine.IsRunning = true;

            _stateMachine.Update();

            Assert.True(_stateA.UpdateCalled);
        }

        [Test]
        public void ShouldCallOnExitOfCurrentStateOnDispose()
        {
            _stateMachine.SwitchState(_stateA.GetType());

            _stateMachine.Dispose();

            Assert.True(_stateA.OnExitCalled);
        }

        StateMachine _stateMachine;
        MockState _stateA;
        MockState _stateB;
    }
}