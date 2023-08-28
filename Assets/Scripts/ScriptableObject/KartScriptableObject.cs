using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data",
    menuName = "ScriptableObjects/KartScriptableObject", order = 1)]
public class KartScriptableObject : ScriptableObject
{
    public Vector3[] spawnPoints;
    public List<Color> playerColors;
}