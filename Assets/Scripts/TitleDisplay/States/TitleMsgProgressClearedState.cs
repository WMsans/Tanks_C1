using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMsgProgressClearedState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.MsgProgressCleared;
    public override void OnEnterState()
    {
        base.OnEnterState();
        GameDataSystemManager.Instance.ClearProgress();
        view.MsgProgressClearedPanelObj.SetActive(true);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        view.MsgProgressClearedPanelObj.SetActive(false);
    }
}
