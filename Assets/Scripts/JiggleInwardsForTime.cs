using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleInwardsForTime : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(LoseJiggleOverTime());
    }

    public IEnumerator LoseJiggleOverTime()
    {
        for (int i = 0; i < 200; i++)
        {
            var pos = this.transform.position;
            this.GetComponent<Rigidbody>().AddForce(-pos * Random.Range(1.0f, 2.5f) + Random.onUnitSphere * 0.15f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(this);
    }
}
