using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineManager : MonoSingleton<OnlineManager>
{
    public bool IsLocal { get; set; } = true;
}
