using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLocalMainState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.LocalMain;
    public override void OnEnterState()
    {
        base.OnEnterState();
        OnlineManager.Instance.IsLocal = true;
        if (GameDataSystemManager.Instance.HasProgress()) view.LocalMainPanelObj.SetActive(true);
        else view.SetState(TitleDisplayState.PickLevel);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        view.LocalMainPanelObj.SetActive(false);
    }
}
