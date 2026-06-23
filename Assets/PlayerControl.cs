using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{

    Rigidbody2D rigid_body;
    float move_force = 15f;
    float jump_force = 30f;
    float drag = .25f;
    Vector2 cur_move_vec;
    bool jumping = false;
    float jump_length = 2f;
    float jump_start = 0f;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rigid_body.AddForceX(-Mathf.Sign(rigid_body.linearVelocityX) * Mathf.Pow(rigid_body.linearVelocityX, 2) * drag);
        rigid_body.AddForceX(cur_move_vec.x * move_force);

        if (cur_move_vec.y > 0)
        {
            if (!jumping)
            {
                ContactPoint2D[] contacts = new ContactPoint2D[rigid_body.attachedColliderCount];
                rigid_body.GetContacts(contacts);
                foreach (var contact in contacts)
                {
                    print($"{contact.normal.x}, {contact.normal.y}");
                    if (contact.normal.y == 1)
                    {
                        jumping = true;
                        jump_start = Time.fixedTime;
                        break;
                    }
                }
            }
        }
        else if (jumping)
        {
            jumping = false;
        }

        if (jumping)
        {
            float jump_time = Time.fixedTime - jump_start;
            if (jump_time >= jump_length)
                jumping = false;
            if (jumping)
            {
                float jump_resist = Mathf.Pow((jump_time / jump_length) * jump_force, 2);
                if (jump_resist < jump_force)
                    rigid_body.AddForceY(jump_force - jump_resist);
                else
                    jumping = false;
            }
        }
    }

    public void OnMove(InputValue value)
    {
        cur_move_vec = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
    }

    public void OnInteract(InputValue value)
    {
    }

}
