using System;
using UnityEngine;

public class PressureState
{
    public float Duration { get; private set; }
    public bool MaxPressureReached { get; private set; }
    public float MaxPressure { get; private set; }

    public PressureState NextState { get; private set; }

    public Func<bool> RequirementsMet { get; private set; }

    public Action action { get; private set; }

    public string Name{ get; private set; }

    public float InitialPressure{  get; private set; }

    public PressureState( string Name, float Duration, float MaxPressure, PressureState NextState, Func<bool> RequirementsMet, Action action)
    {
        this.Duration = Duration;
        this.MaxPressure = MaxPressure;
        this.NextState = NextState;
        this.RequirementsMet = RequirementsMet;
        this.action = action;
        this.Name = Name;
        InitialPressure = MaxPressure;
    }

    public void SetMaxPressure(float val)
    {
        MaxPressure = val;
    }

    public void SetMaxPressureReached(bool val)
    {
        MaxPressureReached = val;
    }

    public void SetDuration(float val)
    {
        Duration = val;
    }
    
    public void SetNextState(PressureState state)
    {
        this.NextState= state;
    }

    public void InvokeActionMethod()
    {
        action.Invoke();
    }

    public void ResetMaxPressure()
    {
        MaxPressure = InitialPressure;
    }
}
