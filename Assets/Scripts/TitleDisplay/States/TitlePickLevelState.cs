using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePickLevelState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.PickLevel;
    public override void OnEnterState()
    {
        base.OnEnterState();
        StartCoroutine(view.GenerateLevelGrid());
    }

    public override void OnExitState()
    {
        base.OnExitState();
        StopCoroutine(view.GenerateLevelGrid());
        view.PickLevelPanelObj.SetActive(false);
    }
}
