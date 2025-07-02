using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMultiplayerState : TitleDisplayBaseState
{
    public override TitleDisplayState State => TitleDisplayState.Multiplayer;
    public override void OnEnterState()
    {
        base.OnEnterState();
        SceneSystemManager.Instance.ChangeScene("3_Multiplayer");
    }
}
