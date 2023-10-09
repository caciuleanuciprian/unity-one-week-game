using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour {
    [SerializeField]
    float moveSpeed = 4f;

    public ParticleSystem dust;
    public ParticleSystem vomit;

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
    public AudioSource burp;
    public AudioSource fart;
    public AudioSource walk;


    public float coneAngle = 60f;
    public float coneLength = 3f;
 
    void Start ()
    {
        transform.position = new Vector3(Random.Range(3f, -3f), 3f, Random.Range(3f, -3f));
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        animator = GetComponent<Animator>();
    }

    void ChangeAnimationState(string newState) {
        if(currentState == newState) return;
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
        animator.Play(newState);
        }
    }

    public void PlayBurp(){
        burp.Play();
    }

    public void PlayFart(){
        fart.Play();
    }

    public void PlayWalk(){
        walk.Play();
    }
 
 
    void Update ()
    {

        if(!IsOwner) return;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            ChangeAnimationState("Run");
            PlayWalk();
            Move();
        } else {
            ChangeAnimationState("Idle");
        }
        if(Input.GetKeyDown(KeyCode.Space) && Time.time - lastDashTime >= dashCooldown) {
            ChangeAnimationState("Dash");
            Dash();
            PlayFart();
            lastDashTime = Time.time;
        } 
        if(Input.GetKeyDown(KeyCode.F)) {
            Push();
            ChangeAnimationState("Push");
            PlayBurp();
        }
    }

    void Push()
    {
        if (Time.time - lastPushTime >= pushCooldown)
        {
            CreateVomit();
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

    void Dash() 
    {
        CreateDust();
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

    }
    
    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * moveSpeed * NetworkManager.Singleton.ServerTime.FixedDeltaTime * Input.GetAxis("Horizontal");
        Vector3 upMovement = forward * moveSpeed * NetworkManager.Singleton.ServerTime.FixedDeltaTime * Input.GetAxis("Vertical");
 
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
 
        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    void CreateDust() {
        dust.Play();
    }

    void CreateVomit() {
        vomit.Play();
    }
}