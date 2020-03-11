using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Look")]
    public bool enableMouseLook = true;
    public float lookSensitivity = 0.2f;

    [Header("Move")]
    public float walkAcceleration = 0.2f;
    public float runAcceleration = 0.5f;
    public float maxWalkSpeed = 0.5f;
    public float maxRunSpeed = 1f;
    public float friction = 0.8f;

    [Header("Jump")]
    public float gravity = 0.98f;

    private Vector3 momentum = Vector3.zero;

    [Header("Debug")]
    public bool noClip = false;
    public float noClipMoveSpeed = 2f;

    private Transform cameraTransform;
    private CapsuleCollider capsuleCollider;
    private Vector2 lookRotation = Vector2.zero;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        //Move();

        if (enableMouseLook)
            Look();
    }
    
    private void Look()
    {
        lookRotation.y += Input.GetAxis("Mouse X");
        lookRotation.x += -Input.GetAxis("Mouse Y");
        lookRotation.x = Mathf.Clamp(lookRotation.x, -30f, 30f);
        transform.eulerAngles = new Vector2(0, lookRotation.y) * lookSensitivity;
        cameraTransform.localRotation = Quaternion.Euler(lookRotation.x * lookSensitivity, 0, 0);
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool jumpInput = Input.GetButton("Jump");
        bool crouchInput = Input.GetButton("Crouch");
        
        if (noClip)
        {
            Vector3 forwardMovement = transform.forward * moveInput.y * noClipMoveSpeed;
            Vector3 horizontalMovement = transform.right * moveInput.x * noClipMoveSpeed;
            Vector3 verticalMovement = transform.up * (jumpInput?noClipMoveSpeed:(crouchInput?-noClipMoveSpeed:0f));

            transform.position += Vector3.ClampMagnitude((forwardMovement + horizontalMovement + verticalMovement), noClipMoveSpeed);
        } else
        {
            Vector3 forwardMovement = transform.forward * moveInput.y;
            Vector3 horizontalMovement = transform.right * moveInput.x;

            momentum += forwardMovement * walkAcceleration;
            momentum += horizontalMovement * walkAcceleration;
            momentum *= friction;

            Vector3.ClampMagnitude(momentum, maxWalkSpeed);

            momentum -= Vector3.up * gravity * Time.deltaTime;

            RaycastHit hit;

            // raycast at feet
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 1.05f))
            {
                transform.position = hit.point + Vector3.up * 1.05f;
                momentum.Scale(new Vector3(1, 0, 1));
            }

            // raycast forward
            // raycast down from forward point
            // determine angle
            // resolve movement
            
            transform.position += momentum;
            
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Debug.Log(collision.contacts[i].normal);
        }
    }
}
