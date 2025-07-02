using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCountText : MonoSingleton<EnemyCountText>
{
    [SerializeField] private TextMeshProUGUI text;
    public void UpdateText(string txt) => text.text = txt;
}
