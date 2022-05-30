using System;
using System.Collections.Generic;
using Odyssey;
using UnityEngine;

public interface IStateMachine : IDisposable
{
    public bool IsRunning { get; set; }
    public void SwitchState(Type newState);
    public void AddState(IState state);
    public IState CurrentState { get; }
    public void Update();
}

public class StateMachine : IStateMachine
{
    public bool IsRunning { get; set; } = false;

    public IState CurrentState { get; internal set; }

    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    private bool isSwitching = false;

    public void SwitchState(Type newStateType)
    {
        if (isSwitching)
        {
            throw new UnityException("Cannot switch state inside OnEnter or OnExit!");
        }

        if (!states.ContainsKey(newStateType))
        {
            throw new UnityException("Trying to switch to not existing state " + newStateType.ToString());
        }

        IState prevState = CurrentState;
        IState newState = states[newStateType];

        isSwitching = true;

        if (prevState != null)
        {
            prevState.OnExit();
        }

        isSwitching = false;

        CurrentState = newState;

        newState.OnEnter();
    }

    public void Update()
    {
        if (CurrentState == null) return;
        if (!IsRunning) return;

        CurrentState.Update();
    }

    public void AddState(IState state)
    {
        states[state.GetType()] = state;
    }

    public void Dispose()
    {
        CurrentState?.OnExit();

        IsRunning = false;
    }
}
