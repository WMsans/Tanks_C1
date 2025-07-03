using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWallDoor : MonoBehaviour, IDoor
{
    [SerializeField] private GameObject doorObj;
    public void OpenDoor()
    {
        Debug.Log(transform.position);
        doorObj.SetActive(false);
    }

    public void CloseDoor()
    {
        doorObj.SetActive(true);
    }
}
