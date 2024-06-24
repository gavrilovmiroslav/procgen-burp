using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorized : MonoBehaviour
{
    public void Colorize(Color color)
    {
        Debug.Log(color);
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }
}
