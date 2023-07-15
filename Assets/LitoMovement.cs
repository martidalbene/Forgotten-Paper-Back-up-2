using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitoMovement : MonoBehaviour
{

    public Rigidbody2D rb;

    public float movX;

    public float litoSpeed;

    public float JumpForce;

    private bool canJump;

    private int lookingAt;
    
    public SpriteRenderer mySpriteRenderer; // Renderer de Lito

    
    private float coyoteTime = .2f;
    private float coyoteTimeCounter; 

    private float jumpBufferTime = .15f;
    private float jumpBufferCounter;

    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        movX = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        InputControler();
    }

    void Jump()
    {
        canJump = false;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    void flip()
    {
       switch (lookingAt)
        {
            case 1:
                mySpriteRenderer.flipX = false;
                break;
            case -1:
                mySpriteRenderer.flipX = true;
                break;
        }
    }

    private void InputControler()
    {
        if(rb.velocity.y == 0)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if(movX != 0) CharacterMovement();
        else rb.velocity = new Vector2(0, rb.velocity.y);   

        if (jumpBufferCounter > 0f && canJump && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            coyoteTimeCounter = 0f;
        }

        lookingAt = (int)movX;
        flip();
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "floor") 
        {
            canJump = true;
            rb.velocity = new Vector2(movX * litoSpeed, 0);
        }
        if(collisionInfo.gameObject.tag == "OneWayPlatform")
        {
            canJump = true;
            rb.velocity = new Vector2(movX * litoSpeed, rb.velocity.y);
        }
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if((collisionInfo.gameObject.tag == "floor" || collisionInfo.gameObject.tag == "OneWayPlatform") && coyoteTimeCounter <= 0f) canJump = false;
    }

    void CharacterMovement()
    {
        rb.velocity = new Vector2(movX * litoSpeed, rb.velocity.y);

        /*if (IsAvionlito) // Si el personaje es Avionlito, su velocidad cambia
        {
            rb.velocity = new Vector2(LookingAt * AvionlitoSpeed, -1f); //velocidad constante cuando te toca el avion
        }*/
    }

    /*void OutOfTheWater()
    {
        jumpOutOfTheWater = true;
        rb.AddForce(Vector2.up * JumpForce * 2.5f, ForceMode2D.Impulse);
    }*/

    public void BackToSpawnPoint()
    {
        transform.position = spawnPoint.transform.position;
        /*TransformTo = 0;
        water = false;
        IsBarlito = false;
        IsAvionlito = false;
        rb.velocity = new Vector2(0, rb.velocity.y); //reseteo velocidades en X y no en Y
        StatChange();
        animLito.TransformingLito();*/
    }
}
