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
    private bool _isChangingScene = false;

    protected override void Start()
    {
        if(changeStateOnStart) base.Start();
    }

    public override void ChangeState(BaseState next)
    {
        if(_isChangingScene)return;
        if (!next) return;
        if (next is not SceneState nextScene) return;
        base.ChangeState(nextScene);
    }

    public void ChangeScene(string sceneName)
    {
        if(_isChangingScene) return;
        var nextSceneStates = GetComponentsInChildren<SceneState>();
        var next = nextSceneStates.FirstOrDefault(x => x.GetSceneName().Equals(sceneName));
        ChangeState(next);
    }

    public void ChangeScene(SceneField sceneField)
    {
        if(_isChangingScene) return;
        var nextSceneStates = GetComponentsInChildren<SceneState>();
        var next = nextSceneStates.FirstOrDefault(x => x.GetSceneName().Equals(sceneField));
        ChangeState(next);
    }

    public void ChangeSceneOnDelay(string sceneName, float delay)
    {
        if(_isChangingScene) return;
        StartCoroutine(ChangeStateOnDelayCoroutine(sceneName, delay));
    }

    private IEnumerator ChangeStateOnDelayCoroutine(string sceneName, float delay)
    {
        _isChangingScene = true;
        yield return new WaitForSeconds(delay);
        _isChangingScene = false;
        ChangeScene(sceneName);
    }
}
