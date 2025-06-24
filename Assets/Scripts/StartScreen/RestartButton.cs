using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private SceneField titleScene;

    public void OnPressed()
    {
        SceneSystemManager.Instance.ChangeScene(titleScene);
    }
}
