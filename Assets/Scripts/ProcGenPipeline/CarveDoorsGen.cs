using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarveDoorsGen : AbstractDelauneyGen
{
    protected List<IEdge> SafeDoors = new();
    protected List<IEdge> ExtraDoors = new();

    [Range(0.0f, 1.0f)]
    public float ChanceForLoop = 0.4f;
    public GameObject[] DoorModels;

    protected long HashDoor(int a, int b)
    {
        long SingleHash(int a, int b) => (a + b) * (a + b + 1) / 2 + a;
        return (long)Mathf.Min(SingleHash(a, b), SingleHash(b, a));
    }

    protected Vector3 DoorPos(IEdge edge)
    {
        var a = new Vector3((float)edge.P.X, 0.0f, (float)edge.P.Y);
        var b = new Vector3((float)edge.Q.X, 0.0f, (float)edge.Q.Y);
        return Vector3.Lerp(a, b, 0.5f) + new Vector3(0.0f, 0.5f, 0.0f);
    }

    protected Quaternion DoorOrient(IEdge edge)
    {
        var a = new Vector3((float)edge.P.X, 0.0f, (float)edge.P.Y);
        var b = new Vector3((float)edge.Q.X, 0.0f, (float)edge.Q.Y);
        var dx = Mathf.Abs(a.x - b.x);
        var dy = Mathf.Abs(a.z - b.z);
        if (dx > dy)
        {
            return Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public override void Gen()
    {
        base.Gen();

        var mst = MinimalSpanningTree.Make(Edges);

        var alreadyConnected = new HashSet<long>();

        // Doors we can't live without
        foreach (var edge in mst)
        {
            var pRoom = this.GetRoomIdForPoint(edge.P);
            var qRoom = this.GetRoomIdForPoint(edge.Q);
            if (pRoom != qRoom)
            {
                var hashed = HashDoor(pRoom, qRoom);
                if (!alreadyConnected.Contains(hashed))
                {
                    SafeDoors.Add(edge);
                    alreadyConnected.Add(hashed);

                    var pos = DoorPos(edge);
                    var orient = DoorOrient(edge);
                    var door = GameObject.Instantiate(DoorModels[Random.Range(0, DoorModels.Length)], pos, orient);
                }
            }
        }

        Edges = Edges.Where(edge =>
        {
            var pRoom = this.GetRoomIdForPoint(edge.P);
            var qRoom = this.GetRoomIdForPoint(edge.Q);
            var hashed = HashDoor(pRoom, qRoom);
            return pRoom != qRoom && !alreadyConnected.Contains(hashed);
        }).Reverse().ToList();

        // Doors we very well CAN live without
        foreach (var edge in Edges)
        {
            var pRoom = this.GetRoomIdForPoint(edge.P);
            var qRoom = this.GetRoomIdForPoint(edge.Q);
            var hashed = HashDoor(pRoom, qRoom);
            if (!alreadyConnected.Contains(hashed))
            {
                if (Random.value > ChanceForLoop)
                {
                    ExtraDoors.Add(edge);
                    alreadyConnected.Add(hashed);

                    var pos = DoorPos(edge);
                    var orient = DoorOrient(edge);
                    var door = GameObject.Instantiate(DoorModels[Random.Range(0, DoorModels.Length)], pos, orient);
                }
            }
        }

        StartCoroutine(WaitForAWhile());
    }

    protected IEnumerator WaitForAWhile()
    {
        yield return new WaitForSeconds(3.0f);
        ProcGen.Next();
    }

    public override void Cleanup()
    {
        base.Cleanup();
        SafeDoors.Clear();
        ExtraDoors.Clear();

        foreach (var door in GameObject.FindGameObjectsWithTag("Door"))
        {
            Destroy(door);
        }
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if ((Object)ProcGen.Instance.Active == this)
            {
                foreach (var edge in SafeDoors)
                {
                    var a = new Vector3((float)edge.P.X, 0.0f, (float)edge.P.Y);
                    var b = new Vector3((float)edge.Q.X, 0.0f, (float)edge.Q.Y);
                    var c = Vector3.Lerp(a, b, 0.5f);
                    c.y = 1.0f;
                    Gizmos.DrawSphere(c, 0.33f);
                }

                Gizmos.color = Color.yellow;
                foreach (var edge in ExtraDoors)
                {
                    var a = new Vector3((float)edge.P.X, 0.0f, (float)edge.P.Y);
                    var b = new Vector3((float)edge.Q.X, 0.0f, (float)edge.Q.Y);
                    var c = Vector3.Lerp(a, b, 0.5f);
                    c.y = 1.0f;
                    Gizmos.DrawSphere(c, 0.25f);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}
