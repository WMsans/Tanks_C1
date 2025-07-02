using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerBehaviour : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            RegisterPlayerToLobbyListRpc(new RpcParams());
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
}
