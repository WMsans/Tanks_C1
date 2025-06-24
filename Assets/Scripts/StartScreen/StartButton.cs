using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private SceneField startScene;

    public void OnPressed()
    {
        SceneSystemManager.Instance.ChangeScene(startScene);
    }
}
