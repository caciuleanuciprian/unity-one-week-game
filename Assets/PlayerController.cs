using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    float moveSpeed = 4f;
 
    Vector3 forward, right;
    Animator animator;
    private string currentState;
 
    void Start ()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        animator = GetComponent<Animator>();
    }

    public void Teleport(Vector3 position, Quaternion rotation) {
        transform.position = position;
        Physics.SyncTransforms();
    }

    void ChangeAnimationState(string newState) {
        if(currentState == newState) return;
        animator.Play(newState);
    }
 
 
    void Update ()
    {
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            ChangeAnimationState("Run");
            Move();
        } else if(Input.GetKey(KeyCode.Space)) {
            ChangeAnimationState("Dash"); // Should be swapped and added to Dash logic to only trigger when the dash is happening
        } else {
            ChangeAnimationState("Idle");
        }
    }


 
    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
 
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
 
        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
}