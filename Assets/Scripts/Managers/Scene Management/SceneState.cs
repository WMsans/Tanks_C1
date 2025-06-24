using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneState : BaseState
{
    [ReadOnly] public SceneField scene;
    public string GetSceneName() => scene;
    public override void OnEnterState()
    {
        SceneManager.LoadScene(scene);
    }
}
