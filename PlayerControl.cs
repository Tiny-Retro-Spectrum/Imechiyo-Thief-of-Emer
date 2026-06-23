using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform playerObj;
    public Rigidbody2D RB;
    public Rigidbody2D.SlideMovement SM = new Rigidbody2D.SlideMovement();
    public Rigidbody2D.SlideResults SR;
    public Transform mainCamera;
    public Collider2D capsCollider;
    float FOV = 10;
    Vector3 movement = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    float speed;
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;

    float fallSpeed = 0.5f; 
    public bool grounded;
    public float jumpCooldown;
    bool readyToJump;

    RaycastHit2D groundCheckHit;
    LayerMask ground;

    Vector2 ToVector2D(Vector3 vector) // converts 3D vectors to 2D
    {
        Vector2 output = new Vector2(vector.x, vector.y);
        return output;
    }

    Vector3 ToVector3D(Vector2 vector) // converts 2D vectors to 3D
    {
        Vector3 output = new Vector3(vector.x, vector.y, 0);
        return output;
    }

    bool OnSlope()
    {
        return (Vector2.up != groundCheckHit.normal);
    }

    Vector3 SlopeMove(Vector3 movementVector)
    {
        return Vector3.ProjectOnPlane(movementVector, ToVector3D(groundCheckHit.normal));
    }

    public MovementState playerState;

    public enum MovementState
    {
        walking,
        running,
        crouching,
        airborne,
        mantling
    }

    void GroundSnap() // snaps player relative to groundcheck position if the player isn't aligned to the ground properly
    {
        Vector3 a = playerObj.position;
        Vector3 b = ToVector3D(groundCheckHit.point);
        if (a.y != b.y + 0.6f)
        {
            Vector3 c = new Vector3(0, a.y - (b.y + 0.6f), 0);
            playerObj.position -= c;
        }
    }

    void GetInput()
    {
        if (Input.GetKey("c") && grounded) // sets movement speed depending on movement state
        {
            speed = crouchSpeed;
            playerState = MovementState.crouching;
        }
        else if (Input.GetKey("left shift") && grounded)
        {
            speed = runSpeed;
            playerState = MovementState.running;
        }
        else if (grounded)
        {
            speed = walkSpeed;
            playerState = MovementState.walking;
        }
        else if (!grounded)
        {
            playerState = MovementState.airborne;
        }

        if (grounded)
        {
            velocity.y = 0;
            movement.x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            if (Input.GetButtonDown("Jump") && readyToJump != false)
            {
                grounded = false;
                velocity.y = 2 * Mathf.Sqrt(16f * fallSpeed);
                readyToJump = false;
                Invoke(nameof(ResetJump), jumpCooldown);
            }

            if (Input.GetAxisRaw("Horizontal") != movement.x && Input.GetAxisRaw("Horizontal") != 0)
            {
                playerObj.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1); // flips player to face left or right
            }
        }
        else if (!grounded)
        {
            velocity.y -= fallSpeed * Time.deltaTime;
        }
    }

    void GroundCheck()
    {   // creates a raycheck from the position of the capsule collider
        groundCheckHit = Physics2D.Raycast(ToVector2D(playerObj.position), Vector2.down, 0.6f, ground, -1f, 1f);
        if (groundCheckHit != false && grounded == false)
        {
            if (velocity.y < 0)
            {
                movement.y = 0;
            }
            velocity.y = 0;
            grounded = true;
            if (readyToJump != false)
            {
                GroundSnap();
            }
        }
        else if (groundCheckHit == false && grounded != false)
        {
            grounded = false;
        }
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void Move()
    {
        GroundCheck();
        GetInput();
        if (movement.magnitude != 0 || velocity.y != 0) // updates movement only if the player isn't still or has no momentum
        {
            if (grounded && !OnSlope())
            {
                SR = RB.Slide(movement, Time.deltaTime, SM);
            }
            else if(grounded && OnSlope())
            {   // movement is snapped to slope as long as the player isn't jumping
                if (readyToJump != false) 
                {
                    SR = RB.Slide(movement, Time.deltaTime, SM);
                }
                else
                {
                    SR = RB.Slide(movement, Time.deltaTime, SM);
                }
            }
            else if (!grounded)
            {
                movement += velocity * Time.deltaTime;
                SR = RB.Slide(movement, Time.deltaTime, SM);
            }
        }
    }

    void Start()
    {
        mainCamera.position = playerObj.position + new Vector3(0, 0, -FOV); // snaps camera to player on start
        ground = LayerMask.GetMask("Ground");
        readyToJump = true;
    }

    void Update()
    {
        mainCamera.position = playerObj.position + new Vector3(movement.x * 10, movement.y * 10, -FOV);
    }

    void FixedUpdate()
    {
        Move();
    }
}
