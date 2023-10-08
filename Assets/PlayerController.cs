using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : MonoBehaviour {
    private Rigidbody rb;
    [SerializeField] private float speed = 5;
    [SerializeField] private float turnSpeed = 360;     
    private Vector2 move;    
    private Vector3 moveInput;
    
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool isGrounded; 
    public float groundDrag;
    
    Animator animator;
    private string currentState;

    private void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {
        Look();
        Move();
        SpeedControl();

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if(isGrounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            ChangeAnimationState("Run");
            Move();
        } else if(Input.GetKey(KeyCode.Space)) {
            ChangeAnimationState("Dash"); // Should be swapped and added to Dash logic to only trigger when the dash is happening
        } else {
            ChangeAnimationState("Idle");
        }
    }

    private void Look() {
        move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveInput = new Vector3(move.x, 0, move.y);
        if (moveInput == Vector3.zero) return;

        var rot = Quaternion.LookRotation(moveInput.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

        public void Teleport(Vector3 position, Quaternion rotation) {
        transform.position = position;
        Physics.SyncTransforms();
    }

    void ChangeAnimationState(string newState) {
        if(currentState == newState) return;
        animator.Play(newState);
    }

    private void Move() {
        rb.AddForce(moveInput.ToIso().normalized * speed, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}

public static class Helpers 
{
    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input);
}