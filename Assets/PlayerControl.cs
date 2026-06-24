using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

enum PlayerState
{
    Idle,
    Attack,
    Crouch_Down,
    Crouch_Up,
    Damaged,
    Death,
    Jump,
    Run,
    Sneak,
    Turn
};

public class PlayerControl : MonoBehaviour
{
    public Sprite[] idle_frames;
    public Sprite[] run_frames;

    Rigidbody2D rigid_body;
    SpriteRenderer sprite_renderer;
    float move_force = 15f;
    float jump_force = 40f;
    float drag = .25f;
    Vector2 cur_move_vec;
    bool jumping = false;
    float jump_length = 1.7f;
    float jump_start = 0f;
    int cur_frame = 0;
    float last_frame_time = 0f;
    float idle_frame_time = 1f / 6f;
    float run_frame_time = 1f / 11f;
    bool flipped = false;
    PlayerState state = PlayerState.Idle;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (state == PlayerState.Idle && Time.fixedTime - last_frame_time >= idle_frame_time)
        {
            cur_frame = (cur_frame + 1) % idle_frames.Length;
            last_frame_time = Time.fixedTime;
            sprite_renderer.sprite = idle_frames[cur_frame];
        }
        else if (state == PlayerState.Run && Time.fixedTime - last_frame_time >= run_frame_time)
        {
            cur_frame = (cur_frame + 1) % run_frames.Length;
            last_frame_time = Time.fixedTime;
            sprite_renderer.sprite = run_frames[cur_frame];
        }

        sprite_renderer.flipX = flipped;
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

        if (rigid_body.linearVelocityX != 0)
        {
            run_frame_time = 1f / (1.7f * Mathf.Abs(rigid_body.linearVelocityX));
            state = PlayerState.Run;
            flipped = rigid_body.linearVelocityX < 0;
        }
        else
        {
            state = PlayerState.Idle;
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
