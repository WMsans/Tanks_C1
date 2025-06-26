using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/TankConfig", fileName = "New Enemy Config")]
public class TankConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5.0f; 
    public float rotSpeed = 2.0f; 
    public float moveAccel = 1;
    public float rotAccel = 1;
}
