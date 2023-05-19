using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpStrength = 3.0f;
    public float deceleration = 5.0f;
    public float maxSlopeAngle = 80.0f;

    public Transform cam;


    private Rigidbody rigid;

    private bool isGrounded = false;
    private bool isOnMovingPlatform = false;
    private Vector3 platformLastPos;
    private GameObject movingPlatform;
    private bool isOnRotatingPlatform = false;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        
        //gets input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Vector3 movement = new Vector3(horizontal, 0.0f, vertical).normalized;

        //moving, based on camera rotation
        Vector3 cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = cameraForward * vertical + cam.transform.right * horizontal;
        moveDirection = Quaternion.FromToRotation(Vector3.up, GetGroundNormal()) * moveDirection;

        if (moveDirection.magnitude != 0.0f)
        {
            rigid.AddForce(moveDirection * speed * 10 * Time.deltaTime, ForceMode.VelocityChange);
            //transform.Translate(moveDirection * speed * 10 * Time.deltaTime);
        }
        else
        {
            //damping
            Vector3 decelerationForce = new Vector3(-rigid.velocity.x, 0f, -rigid.velocity.z).normalized * deceleration;
            rigid.AddForce(decelerationForce, ForceMode.Acceleration);
      
        }
        
        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, 10);

        //rotation
        transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

        //old damping
        //Vector3 velocity = rigid.velocity;
        //velocity.x *= 1.0f - 3f * Time.deltaTime;
        //velocity.y *= 1.0f - 3f * Time.deltaTime;
        //rigid.velocity = velocity;

        
        rigid.AddForce(Physics.gravity, ForceMode.Force);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

    }

    private void LateUpdate()
    {
        if (isOnMovingPlatform)
        {
            MoveWithPlatform();
        }
    }

    Vector3 GetGroundNormal()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            return hit.normal;
        }

        return Vector3.up;
    }

    void Jump()
    {
        float drag = rigid.drag;
        rigid.drag = 0.0f;
        isGrounded = false;
        rigid.AddForce(transform.up * jumpStrength, ForceMode.Impulse);
        rigid.drag = drag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if(collision.gameObject.CompareTag("Moving platform"))
        {
            //transform.SetParent(collision.gameObject.transform);
            isOnMovingPlatform = true;
            movingPlatform = collision.gameObject;
            platformLastPos = collision.gameObject.transform.position;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Moving platform"))
        {
            //transform.parent = null;
            isOnMovingPlatform = false;
            movingPlatform = null;
            platformLastPos = new Vector3(0, 0, 0);
        }
    }

    private void RotateWithPlatform()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {

        }
    }

    private void MoveWithPlatform()
    {
        Vector3 displacement = movingPlatform.transform.position - platformLastPos;
        //transform.position += displacement * Time.deltaTime;
        rigid.MovePosition(transform.position + displacement * Time.deltaTime);
        
    }
}
