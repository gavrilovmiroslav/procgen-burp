using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelTemplate", menuName = "Data/LevelTemplate", order = 1)]
public class LevelTemplate : ScriptableObject
{
    public List<RoomAmount> Rooms = new();
    public float Spread = 10.0f;
}