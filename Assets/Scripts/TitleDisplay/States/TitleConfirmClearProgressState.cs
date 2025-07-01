using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleConfirmClearProgressState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.ConfirmClearProgress;
    public override void OnEnterState()
    {
        base.OnEnterState();
        view.ConfirmClearProgressPanelObj.SetActive(true);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        view.ConfirmClearProgressPanelObj.SetActive(false);
    }
}
