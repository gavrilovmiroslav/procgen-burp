using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleRandomlyForTime : MonoBehaviour
{
    public float Strength = 2.0f;

    public void Start()
    {
        StartCoroutine(LoseJiggleOverTime());
    }

    public IEnumerator LoseJiggleOverTime()
    {
        for (int i = 0; i < 200; i++)
        {
            var pos = this.transform.position;
            var wanted = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
            this.GetComponent<Rigidbody>().AddForce((pos - wanted).normalized * 2.0f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(this);
    }

    public void OnCollisionStay(Collision collision)
    {
        this.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * Strength, ForceMode.Impulse);
    }
}
