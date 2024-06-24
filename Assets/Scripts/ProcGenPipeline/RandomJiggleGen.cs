using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;

public class RandomJiggleGen : ProcGenStep
{
    public List<LevelTemplate> LevelTemplates = new();

    public override void Cleanup()
    {
        base.Cleanup();

        StopAllCoroutines();
        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            GameObject.Destroy(room.gameObject);
        }
    }

    public override void Gen()
    {
        base.Gen();

        var template = LevelTemplates[Random.Range(0, LevelTemplates.Count)];
        int id = 0;
        foreach (var room in template.Rooms)
        {
            for (int i = 0; i < room.Amount; i++)
            {
                var start = Random.insideUnitCircle * template.Spread;
                var pos = new Vector3(start.x, 0.0f, start.y);
                var theta = Random.Range(0, 3) * 90.0f;
                var instance = Instantiate(room.Room, pos, Quaternion.Euler(0.0f, theta, 0.0f));
                instance.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere - pos.normalized * 2.0f);
                instance.AddComponent<RoomName>().Name = room.Room.gameObject.name;
                instance.AddComponent<Identity>().Id = id;
                id++;
            }
        }

        StartCoroutine(WaitForJiggleToStop());
    }

    IEnumerator WaitForJiggleToStop()
    {
        yield return new WaitForSeconds(10.0f);
        ProcGen.Next();
    }
}
