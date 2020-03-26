using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private const int airJumpMax = 1;

    public LayerMask groundLayer;

    [SerializeField]
    private float walkSpeed = 10.0f;
    [SerializeField]
    private float accellSpeed = 50.0f;
    [SerializeField]
    private float stopSpeed = 40.0f;
    [SerializeField]
    private float airStopSpeed = 10.0f;
    [SerializeField]
    private float jumpForce = 30.0f;
    [SerializeField]
    private float isGroundedDelay = 0.05f;

    private Rigidbody2D rb;
    private Transform sprite;

    private bool isGrounded;
    private Transform groundedParent;
    private int airJumpCount = airJumpMax;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Player_Sprite");
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        if (groundedParent != transform.parent)
        {
            if (transform.parent != null && transform.parent.GetComponent<ReplayMovement>() != null)
            {
                rb.velocity += transform.parent.GetComponent<ReplayMovement>().kinematicVelocity;
            }
        }
        transform.parent = groundedParent;

        Movement();
    }

    private void Update()
    {
        JumpReset();
        Jump();
    }

    private void Movement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        float x = 0;

        float stopSpeedMod;
        if (isGrounded)
        {
            stopSpeedMod = stopSpeed;
        }
        else
        {
            stopSpeedMod = airStopSpeed;
        }

        //If movement key pressed
        if (Mathf.Abs(inputX) > 0)
        {
            if (rb.velocity.x < 0 && inputX > 0 || rb.velocity.x > 0 && inputX < 0)
            {
                x = inputX * stopSpeedMod;
            }
            //Limit x (velocity) to walkSpeed
            else if (rb.velocity.x < walkSpeed && inputX > 0 || rb.velocity.x > -walkSpeed && inputX < 0)
            {
                x = inputX * accellSpeed;
            }
        }
        //If movement key not pressed
        else
        {
            //Aim x (velocity) to 0
            if (Mathf.Abs(rb.velocity.x) >= 3)
            {
                if (rb.velocity.x > 0)
                {
                    x = -stopSpeedMod;
                }
                else if (rb.velocity.x < 0)
                {
                    x = stopSpeedMod;
                }
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        Vector2 movementVector = new Vector2(x, 0);
        rb.AddForce(movementVector);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Vector2 jumpVector = new Vector2(0, jumpForce);

            if (isGrounded)
            {
                rb.AddForce(jumpVector, ForceMode2D.Impulse);
            }
            else if (!isGrounded && airJumpCount > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(jumpVector, ForceMode2D.Impulse);
                airJumpCount -= 1;
            }
        }
    }

    private void JumpReset()
    {
        if (isGrounded && airJumpCount != airJumpMax)
        {
            airJumpCount = airJumpMax;
        }
    }

    private void CheckGrounded()
    {
        Vector2 position = rb.position;
        Vector2 size = new Vector2(sprite.lossyScale.x - 0.05f, 0.2f);

        Collider2D hit = Physics2D.OverlapBox(position, size, 0, groundLayer);
        groundedParent = null;
        if (hit != null)
        {
            isGrounded = true;
            groundedParent = hit.transform.root;
        } 
        else
        {
            StartCoroutine(IsGroundedDelay());
        }
    }

    private IEnumerator IsGroundedDelay()
    {
        yield return new WaitForSeconds(isGroundedDelay);
        isGrounded = false;
    }
}
