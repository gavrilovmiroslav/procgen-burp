using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;

public class GravityGen : ProcGenStep
{
    public override void Cleanup()
    {
        base.Cleanup();
        StopAllCoroutines();
    }

    public override void Gen()
    {
        base.Gen();
        foreach(var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            room.AddComponent<JiggleInwardsForTime>();
        }
 
        StartCoroutine(WaitForJiggleToStop());
    }

    IEnumerator WaitForJiggleToStop()
    {
        yield return new WaitForSeconds(10.0f);
        ProcGen.Next();
    }
}
