using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    public List<string> Tags;
    public UnityEvent OnEnterRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            OnEnterRoom.Invoke();
        }
    }
}