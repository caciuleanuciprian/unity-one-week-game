using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] GameObject player;

    private void OnTriggerEnter(Collider other) {
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport() {
        yield return new WaitForSeconds(0.25f);
        player.transform.position = new Vector3(
            destination.transform.position.x, 
            destination.transform.position.y, 
            destination.transform.position.z);
            
    }
}