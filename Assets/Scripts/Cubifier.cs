using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cubifier : MonoBehaviour
{
    public Vector3 SectionCount = new Vector3(1, 10, 10);
    private Vector3 SectionSize;
    private Vector3 OriginalSize;
    private Vector3 FillStartPos;

    private void Start()
    {
        OriginalSize = gameObject.transform.lossyScale;
        SectionSize = new Vector3(
            OriginalSize.x / SectionCount.x,
            OriginalSize.y / SectionCount.y,
            OriginalSize.z / SectionCount.z);

        FillStartPos = gameObject.transform.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f))
            + gameObject.transform.TransformDirection(new Vector3(SectionSize.x, -SectionSize.y, SectionSize.z) * 0.5f);

        Divide();
        Destroy(gameObject, 0.1f);
    }

    void Divide()
    {
        for (int i = 0; i < SectionCount.x; i++)
        {
            for (int j = 0; j < SectionCount.y; j++)
            {
                for (int k = 0; k < SectionCount.z; k++)
                {
                    var Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Cube.transform.localScale = SectionSize;
                    Cube.transform.position = FillStartPos + gameObject.transform.TransformDirection(new Vector3(
                        SectionSize.x * i, -SectionSize.y * j, SectionSize.z * k
                    ));
                    Cube.transform.rotation = gameObject.transform.rotation;
                    Cube.tag = "WallTile";
                    Cube.layer = gameObject.layer;
                }
            }
        }
    }
}
