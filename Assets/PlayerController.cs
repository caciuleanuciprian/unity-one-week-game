using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    float moveSpeed = 4f;

    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 3f;
    private float lastDashTime = -Mathf.Infinity;

    [SerializeField] float pushForce = 5f;
    [SerializeField] float pushCooldown = 1f; // Adjust as needed
    private float lastPushTime = -Mathf.Infinity;
 
    Vector3 forward, right;
    Animator animator;
    private string currentState;

    public float coneAngle = 60f;
    public float coneLength = 3f;
 
    void Start ()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        animator = GetComponent<Animator>();
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
        } else {
            ChangeAnimationState("Idle");
        }
        if(Input.GetKeyDown(KeyCode.Space) && Time.time - lastDashTime >= dashCooldown) {
            ChangeAnimationState("Dash");
            Dash();
            lastDashTime = Time.time;
        } 
        if(Input.GetKeyDown(KeyCode.F)) {
            Push();
        }
        OnDrawGizmos();
    }

    void Push()
    {
        if (Time.time - lastPushTime >= pushCooldown)
        {
            float coneAngle = 60f; // Adjust the cone angle as needed

            Vector3 coneDirection = transform.forward;

            float halfConeAngleRad = Mathf.Deg2Rad * coneAngle * 0.5f;

            LayerMask pushableLayer = LayerMask.GetMask("Pushable"); // Make sure to assign the appropriate layer

            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, coneLength, pushableLayer); // Adjust the cone length as needed

            foreach (Collider collider in nearbyColliders)
            {
                if (collider.gameObject != gameObject)
                {
                    Vector3 playerToTarget = collider.transform.position - transform.position;

                    float angle = Vector3.Angle(coneDirection, playerToTarget);

                    if (angle <= coneAngle * 0.5f)
                    {
                        Rigidbody rigidbody = collider.gameObject.GetComponent<Rigidbody>();

                        if (rigidbody != null)
                        {
                            Vector3 forceToApply = coneDirection * pushForce;

                            rigidbody.AddForce(forceToApply, ForceMode.Impulse);
                        }
                    }
                }
            }

            // Update the last push time
            lastPushTime = Time.time;
        }
    }

    void OnDrawGizmos()
    {

        Vector3 coneDirection = transform.forward;

        // Calculate the half angles of the cone
        float halfConeAngleRad = Mathf.Deg2Rad * coneAngle * 0.5f;

        // Calculate the forward edge of the cone
        Vector3 forwardEdge = transform.position + coneDirection * coneLength;

        // Calculate the left and right edges of the cone
        Vector3 leftEdge = transform.position + Quaternion.Euler(0, -coneAngle * 0.5f, 0) * coneDirection * coneLength;
        Vector3 rightEdge = transform.position + Quaternion.Euler(0, coneAngle * 0.5f, 0) * coneDirection * coneLength;

        // Draw the cone in the scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, forwardEdge);
        Gizmos.DrawLine(transform.position, leftEdge);
        Gizmos.DrawLine(transform.position, rightEdge);

        // Draw lines connecting the edges to the player for clarity
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, leftEdge);
        Gizmos.DrawLine(transform.position, rightEdge);
    }

    void Dash() 
    {
        StartCoroutine(PerformDash());
    }

    IEnumerator PerformDash()
    {
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + dashDirection * dashDistance;

        while (Time.time - startTime < dashDuration)
        {
            float progress = (Time.time - startTime) / dashDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        // Ensure the player ends up at the exact dash destination
        transform.position = endPosition;

        // Transition back to the appropriate state
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            ChangeAnimationState("Run");
        }
        else
        {
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