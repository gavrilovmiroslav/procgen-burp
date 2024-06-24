using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractDelauneyGen : ProcGenStep
{
    public float MinEdgeLength = 0.0f;
    public float MaxEdgeLength = 3.0f;

    protected List<IEdge> Edges = new();
    protected Dictionary<IPoint, GameObject> PointsInBlock = new();
    protected Dictionary<GameObject, GameObject> BlocksInRoom = new();
    protected Dictionary<int, GameObject> IdentityInRoom = new();
    protected Dictionary<int, int> RoomInSet = new();
    protected DisjointSet ConnectedRooms;

    public override void Gen()
    {
        base.Gen();
        var del = MakeDelauney();

        del.ForEachTriangleEdge(edge =>
        {
            var pRoom = GetRoomIdForPoint(edge.P);
            var qRoom = GetRoomIdForPoint(edge.Q);

            var p = new Vector2((float)edge.P.X, (float)edge.P.Y);
            var q = new Vector2((float)edge.Q.X, (float)edge.Q.Y);
            var d = Vector2.Distance(p, q);
            if (d >= MinEdgeLength && d < MaxEdgeLength)
            {
                Edges.Add(edge);
                ConnectedRooms.UnionSet(pRoom, qRoom);
            }
        });
    }

    public override void Cleanup()
    {
        base.Cleanup();
        Edges.Clear();
        PointsInBlock.Clear();
        BlocksInRoom.Clear();
        IdentityInRoom.Clear();
        RoomInSet.Clear();
    }

    public int GetRoomIdForPoint(IPoint p)
    {
        return BlocksInRoom[PointsInBlock[p]].GetComponent<Identity>().Id;
    }

    public Delaunator MakeDelauney()
    {
        var points = new List<IPoint>();
        var roomCount = GameObject.FindGameObjectsWithTag("Room").Length;
        ConnectedRooms = new DisjointSet(roomCount);
        foreach (var block in GameObject.FindGameObjectsWithTag("RoomBlock"))
        {
            var room = block.transform.parent.gameObject;
            var point = new Point(block.transform.position.x, block.transform.position.z);
            var id = room.GetComponent<Identity>().Id;

            BlocksInRoom.Add(block, room);
            PointsInBlock.Add(point, block);
            if (!IdentityInRoom.ContainsKey(id))
            {
                IdentityInRoom.Add(id, room);
                ConnectedRooms.MakeSet(id);
                RoomInSet.Add(id, 0);
            }

            points.Add(point);
        }

        return new Delaunator(points.ToArray());
    }
}
