using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "GameOver") {
            Debug.Log("GameOver!");
            Destroy(gameObject);
        }
    }
}
