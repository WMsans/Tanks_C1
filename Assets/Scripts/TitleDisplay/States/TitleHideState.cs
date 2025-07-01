using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleHideState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.Hide;
    public override void OnEnterState()
    {
        base.OnEnterState();
        view.HideAllPanel();
    }
}
