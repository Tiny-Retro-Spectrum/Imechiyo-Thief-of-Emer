using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform playerObj;
    // public CharacterController CC; the character controller component isn't designed for 2D, will have to find a replacement
    public Transform mainCamera;
    public float FOV;
    Vector3 movement = new Vector3 (0, 0, 0);
    float speed;
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;
    float fallSpeed = 9.81f; // not used yet cuz I still need to figure out 2D raycasts so I can add gravityyyyy
    bool grounded;

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

    void GetInput()
    {
        if (Input.GetKey("c")) // sets movement speed depending on movement state
        {
            speed = crouchSpeed;
        }
        else if (Input.GetKey("left shift"))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        movement.x = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // ammend for jump arcs and falling later
    }

    void Move()
    {
        if (Input.GetAxisRaw("Horizontal") != movement.x && Input.GetAxisRaw("Horizontal") != 0)
        {
            playerObj.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1); // flips player to face left or right
        }
        GetInput();
        if (movement.magnitude != 0) // updates movement only if the player isn't still
        {
            playerObj.position += movement; // changes player position based on movement value
            mainCamera.position = playerObj.position + new Vector3(movement.x * 10, movement.y * 10, -FOV); // camera stays ahead of the player by how fast it travels
        }
    }

    void Start()
    {
        mainCamera.position = playerObj.position + new Vector3(0, 0, -FOV); // snaps camera to player on start
    }

    void Update()
    {
        Move();
    }
}
