using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

enum PlayerState
{
    Idle,
    Attack,
    CrouchIdle,
    CrouchDown,
    CrouchUp,
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
    public Sprite[] jump_frames;
    public Sprite[] sneak_frames;
    public Sprite[] crouch_idle_frames;

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
    float jump_frame_time = 1f / 11f;
    float sneak_frame_time = 1f / 11f;
    float crouch_idle_frame_time = 1f / 11f;
    bool flipped = false;
    bool crouched = false;
    PlayerState state = PlayerState.Idle;
    PlayerState last_state = PlayerState.Idle;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (state != last_state)
        {
            cur_frame = 0;
            last_frame_time = 0;
        }

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
        else if (state == PlayerState.Jump && Time.fixedTime - last_frame_time >= jump_frame_time)
        {
            cur_frame = (cur_frame + 1) % jump_frames.Length;
            last_frame_time = Time.fixedTime;
            sprite_renderer.sprite = jump_frames[cur_frame];
        }
        else if (state == PlayerState.Sneak && Time.fixedTime - last_frame_time >= sneak_frame_time)
        {
            cur_frame = (cur_frame + 1) % sneak_frames.Length;
            last_frame_time = Time.fixedTime;
            sprite_renderer.sprite = sneak_frames[cur_frame];
        }
        else if (state == PlayerState.CrouchIdle && Time.fixedTime - last_frame_time >= crouch_idle_frame_time)
        {
            cur_frame = (cur_frame + 1) % crouch_idle_frames.Length;
            last_frame_time = Time.fixedTime;
            sprite_renderer.sprite = crouch_idle_frames[cur_frame];
        }

        last_state = state;
        sprite_renderer.flipX = flipped;
    }

    void FixedUpdate()
    {
        rigid_body.AddForceX(-Mathf.Sign(rigid_body.linearVelocityX) * Mathf.Pow(rigid_body.linearVelocityX, 2) * drag);
        rigid_body.AddForceX(cur_move_vec.x * move_force);
        bool touching_ground = false;
        crouched = cur_move_vec.y < 0;
        ContactPoint2D[] contacts = new ContactPoint2D[rigid_body.attachedColliderCount];
        rigid_body.GetContacts(contacts);
        foreach (var contact in contacts)
        {
            if (contact.normal.y == 1)
            {
                touching_ground = true;
                break;
            }
        }

        if (cur_move_vec.y > 0)
        {
            if (!jumping && touching_ground)
            {
                jumping = true;
                jump_start = Time.fixedTime;
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

        if (jumping)
        {
            state = PlayerState.Jump;
        }
        else if (state == PlayerState.Jump && touching_ground)
        {
            if (!crouched)
                state = PlayerState.Idle;
            else
                state = PlayerState.CrouchIdle;
        }

        if (rigid_body.linearVelocityX != 0)
        {
            if (state != PlayerState.Jump)
            {
                run_frame_time = 1f / (1.7f * Mathf.Abs(rigid_body.linearVelocityX));
                if (!crouched)
                    state = PlayerState.Run;
                else
                    state = PlayerState.Sneak;
            }
            flipped = rigid_body.linearVelocityX < 0;
        }
        else if (state != PlayerState.Jump)
        {
            if (!crouched)
                state = PlayerState.Idle;
            else
                state = PlayerState.CrouchIdle;
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
