using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerUiEvents : MonoBehaviour
{
    [SerializeField] private SceneField gameScene;
    public void HostServer()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ConnectServer()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartMultiplayerGame()
    {
        OnlineManager.Instance.IsLocal = false;
        NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }
}
