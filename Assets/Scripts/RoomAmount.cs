using UnityEngine;


[System.Serializable]
public class RoomAmount
{
    [SerializeField]
    public GameObject Room;

    [SerializeField]
    [Range(0, 100)]
    public int Amount;
}