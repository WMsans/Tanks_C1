using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TitleDisplayBaseState : BaseState
{
    public abstract TitleDisplayState State { get; }
    protected TitleDisplayView view;
    public override void OnEnterState()
    {
        view = FindFirstObjectByType<TitleDisplayView>();
    }

    public static implicit operator TitleDisplayBaseState(TitleDisplayState state)
    {
        var baseStates = FindObjectsByType<TitleDisplayBaseState>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        return baseStates.FirstOrDefault(x => x.State == state);
    }
}
