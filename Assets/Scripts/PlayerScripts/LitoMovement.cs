using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitoMovement : MonoBehaviour
{
    public Lito pj;

    public Rigidbody2D rb;

    public float movX;

    public float litoSpeed;
    public float litoJump = 12;

    public float JumpForce;

    public bool canJump;
    
    public bool jumpOutOfTheWater = false;

    private int lookingAt;
    
    public SpriteRenderer mySpriteRenderer; // Renderer de Lito

    
    private float coyoteTime = .13f;
    private float coyoteTimeCounter; 

    private float jumpBufferTime = .3f;
    private float jumpBufferCounter;
    private bool startCountDownCoyote = false;

    private float speed; // Velocidad actual de Lito
    public float BarlitoJump; // Fuerza de Lito al saltar con el barco
    public float BarlitoSpeed; // Velocidad de Lito transformado en Barco estando en el piso
    public float BarlitoWaterMaxSpeed; // Velocidad de Lito transformado en Barco estando en el agua
    private float BarlitoWaterAcceleration = 5f; // Velocidad máxima que puede alcanzar el barco en el agua
    public float AvionlitoSpeed; // Velocidad de Lito transformado en avion


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        speed = litoSpeed;
    }

    void FixedUpdate()
    {
        movX = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        InputControler();
        WaterAcceleration();
    }

    public void Jump()
    {
        canJump = false;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    public void OutOfTheWater()
    {
        jumpOutOfTheWater = true;
        rb.AddForce(Vector2.up * JumpForce * 2f, ForceMode2D.Impulse);
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
        if (startCountDownCoyote)
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
        if(coyoteTimeCounter <= 0f) canJump = false;

        if(movX != 0 && !pj.GrandpaIsTalking) CharacterMovement();

        else rb.velocity = new Vector2(0, rb.velocity.y);   

        if (jumpBufferCounter > 0f && canJump && coyoteTimeCounter > 0f && !pj.GrandpaIsTalking)
        {
            Jump();
            AudioManager.Instance.Play("jump");
            jumpBufferCounter = 0f;
        }

        if(!canJump)
        {
            AudioManager.Instance.Stop("walk");
        }

        lookingAt = (int)movX;
        flip();
    }



    void CharacterMovement()
    {
        rb.velocity = new Vector2(movX * speed, rb.velocity.y);

        if (pj.IsAvionlito) // Si el personaje es Avionlito, su velocidad cambia
        {
            rb.velocity = new Vector2(lookingAt * AvionlitoSpeed, -1f); //velocidad constante cuando te toca el avion
        }
    }

    public void StatChange()
    {
        // Si Lito está en su estado de Lito, se le asignan las variables predeterminadas
        if (!pj.IsBarlito && !pj.IsAvionlito)
        {
            speed = litoSpeed;
            JumpForce = litoJump;
            rb.gravityScale = 1.6f;
        }

        // Si Lito es un barco, verifico si está en el agua o no, y le asigno su velocidad y gravedad
        if (pj.IsBarlito)
        {
            if (pj.water && speed < BarlitoWaterMaxSpeed)
            {
                speed += BarlitoWaterAcceleration * Time.deltaTime;
            }
            else
            {
                speed = BarlitoSpeed;
            }
            JumpForce = BarlitoJump;
            rb.gravityScale = 2.5f;
        }
    }

    public void WaterAcceleration()
    {
        if (speed < BarlitoWaterMaxSpeed && pj.water)
        {
            speed += BarlitoWaterAcceleration * Time.deltaTime;
        }
        else if(!pj.water && pj.IsBarlito)
        {
            speed = BarlitoSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "floor" || collisionInfo.gameObject.tag == "OneWayPlatform")
        {
            canJump = true;
        }
    }

    /*void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if ((collisionInfo.gameObject.tag == "floor" || collisionInfo.gameObject.tag == "OneWayPlatform"))
        {
            startCountDownCoyote = true;
        }
    }*/

    private void OnTriggerExit2D(Collider2D other)
    {

        if((other.gameObject.tag == "Water" || other.gameObject.tag == "DirtyWater") && pj.IsBarlito && jumpOutOfTheWater)
        {
            pj.TransformTo = 0;
            pj.water = false;
            pj.IsBarlito = false;
            pj.IsAvionlito = false;
            rb.velocity = new Vector2(0, rb.velocity.y); //reseteo velocidades en X y no en Y
            StatChange();
            pj.animLito.TransformingLito();
        }
        if(other.gameObject.tag == "DirtyWater")
        {
            pj.Dirty = false;
            pj.animLito.animator.SetBool("DirtyWater", false);
        }
        if ((other.gameObject.tag == "floor" || other.gameObject.tag == "OneWayPlatform"))
        {
            startCountDownCoyote = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "floor")
        {
            rb.velocity = new Vector2(movX * litoSpeed, 0);
            coyoteTimeCounter = coyoteTime;
            startCountDownCoyote = false;
        }
        if (other.gameObject.tag == "OneWayPlatform")
        {
            rb.velocity = new Vector2(movX * litoSpeed, rb.velocity.y);
            coyoteTimeCounter = coyoteTime;
            startCountDownCoyote = false;
        }
    }
}
