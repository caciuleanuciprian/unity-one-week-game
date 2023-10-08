using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player") {
            Destroy(gameObject);
        }
    }
}
