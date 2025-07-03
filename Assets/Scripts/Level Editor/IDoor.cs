using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDoor
{
    public GameObject gameObject { get; }
    public void OpenDoor();
    public void CloseDoor();
}
