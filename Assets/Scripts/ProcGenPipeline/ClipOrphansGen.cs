using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClipOrphansGen : AbstractDelauneyGen
{
    public override void Gen()
    {
        base.Gen();
        
        var colors = new Dictionary<int, Color>();
        for (int i = 0; i < IdentityInRoom.Count; i++)
        {
            colors.Add(i, Random.ColorHSV());
        }

        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            var id = room.GetComponent<Identity>().Id;
            var set = ConnectedRooms.FindSet(id);
            RoomInSet[set] = RoomInSet[set] + 1;

            var color = colors[set];
            room.transform.BroadcastMessage("Colorize", color, SendMessageOptions.DontRequireReceiver);
        }

        var largest = RoomInSet.OrderBy(f => f.Value).Last().Key;
        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            var id = room.GetComponent<Identity>().Id;
            var set = ConnectedRooms.FindSet(id);
            if (set != largest)
            {                
                DestroyImmediate(room);
            }
        }

        var newId = 0;
        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            var id = room.GetComponent<Identity>();
            id.Id = newId;
            newId++;

            room.GetComponent<Rigidbody>().isKinematic = true;
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
        StopAllCoroutines();
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if ((Object)ProcGen.Instance.Active == this)
            {
                foreach (var edge in Edges)
                {
                    Gizmos.DrawLine(new Vector3((float)edge.P.X, 0.0f, (float)edge.P.Y), new Vector3((float)edge.Q.X, 0.0f, (float)edge.Q.Y));
                }
            }
        }
    }
}
