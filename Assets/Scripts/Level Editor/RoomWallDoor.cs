using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWallDoor : MonoBehaviour, IDoor
{
    [SerializeField] private GameObject doorObj;
    public void OpenDoor()
    {
        doorObj.SetActive(false);
    }

    public void CloseDoor()
    {
        doorObj.SetActive(true);
    }
}
