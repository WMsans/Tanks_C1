using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject networkPlayerTankPrefab;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            RegisterPlayerToLobbyListRpc(new RpcParams());

            NetworkManager.Singleton.SceneManager.OnSceneEvent += RequestNewPlayerTank;
        }
    }

    [Rpc(SendTo.Server)]
    private void RegisterPlayerToLobbyListRpc(RpcParams rpcParams)
    {
        GameObject.Find("PlayerListText").GetComponent<TextMeshProUGUI>().text +=
            "Player" + rpcParams.Receive.SenderClientId + "\n";
        UpdatePlayerListRpc(GameObject.Find("PlayerListText").GetComponent<TextMeshProUGUI>().text);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePlayerListRpc(string newListText)
    {
        GameObject.Find("PlayerListText").GetComponent<TMP_Text>().text = newListText;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnInNetworkedPlayerTankRpc(RpcParams rpcParams)
    {
        Debug.Log(networkPlayerTankPrefab.name);
        var newTank = Instantiate(networkPlayerTankPrefab);
        newTank.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }

    private void RequestNewPlayerTank(SceneEvent sceneEvent)
    {
        
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            SpawnInNetworkedPlayerTankRpc(new RpcParams());
        }
    }
}
