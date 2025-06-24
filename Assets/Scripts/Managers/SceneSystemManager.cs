using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneSystemManager : StateMachineRunner
{
    public static SceneSystemManager Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("More tha one Scene manager is on the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] private bool changeStateOnStart = false;

    protected override void Start()
    {
        if(changeStateOnStart) base.Start();
    }

    public override void ChangeState(BaseState next)
    {
        if (!next) return;
        if (next is not SceneState nextScene) return;
        base.ChangeState(nextScene);
    }

    public void ChangeScene(string sceneName)
    {
        var nextSceneStates = GetComponentsInChildren<SceneState>();
        var next = nextSceneStates.FirstOrDefault(x => x.GetSceneName().Equals(sceneName));
        ChangeState(next);
    }

    public void ChangeScene(SceneField sceneField)
    {
        var nextSceneStates = GetComponentsInChildren<SceneState>();
        var next = nextSceneStates.FirstOrDefault(x => x.GetSceneName().Equals(sceneField));
        ChangeState(next);
    }
}
