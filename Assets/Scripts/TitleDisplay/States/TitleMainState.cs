using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMainState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.Main;
    public override void OnEnterState()
    {
        base.OnEnterState();
        view.MainPanelObj.SetActive(true);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        view.MainPanelObj.SetActive(false);
    }
}
