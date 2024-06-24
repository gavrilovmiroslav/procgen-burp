using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallPattern
{
    [SerializeField]
    public GameObject Key;

    [SerializeField]
    public GameObject[] Targets;
}

public class BuildWallsGen : ProcGenStep
{
    public List<WallPattern> Walls = new();
    private Dictionary<string, GameObject[]> WallsByName = new();

    public void Start()
    {
        foreach (var wallPattern in Walls)
        {
            WallsByName.Add(wallPattern.Key.gameObject.name, wallPattern.Targets);
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        foreach (var wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(wall);
        }
        StopAllCoroutines();
    }

    public override void Gen()
    {
        base.Gen();
        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            var name = room.GetComponent<RoomName>().Name;
            if (WallsByName.ContainsKey(name))
            {
                var walls = WallsByName[name];
                var wall = walls[Random.Range(0, walls.Length)];
                Instantiate(wall, room.transform.position, room.transform.rotation);
            }
        }

        StartCoroutine(WaitForAWhile());
    }

    public IEnumerator WaitForAWhile()
    {
        yield return new WaitForSeconds(3.0f);
        ProcGen.Next();
    }
}
