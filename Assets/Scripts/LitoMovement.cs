using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitoMovement : MonoBehaviour
{
    public Lito pj;

    public Rigidbody2D rb;

    public float movX;

    public float litoSpeed;

    public float JumpForce;

    public bool canJump;
    
    public bool jumpOutOfTheWater = false;

    private int lookingAt;
    
    public SpriteRenderer mySpriteRenderer; // Renderer de Lito

    
    private float coyoteTime = .13f;
    private float coyoteTimeCounter; 

    private float jumpBufferTime = .15f;
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

    public void Jump()
    {
        canJump = false;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    public void OutOfTheWater()
    {
        jumpOutOfTheWater = true;
        rb.AddForce(Vector2.up * JumpForce * 2.5f, ForceMode2D.Impulse);
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

        if(movX != 0) CharacterMovement();

        else rb.velocity = new Vector2(0, rb.velocity.y);   

        if (jumpBufferCounter > 0f && canJump && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
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
            coyoteTimeCounter = coyoteTime;
            startCountDownCoyote = false;
        }
        if(collisionInfo.gameObject.tag == "OneWayPlatform")
        {   
            canJump = true;
            rb.velocity = new Vector2(movX * litoSpeed, rb.velocity.y);
            coyoteTimeCounter = coyoteTime;
            startCountDownCoyote = false;
        }
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if((collisionInfo.gameObject.tag == "floor" || collisionInfo.gameObject.tag == "OneWayPlatform")) {
            Debug.Log("Deje de tocar piso");
            startCountDownCoyote = true;
        }
    }

    void CharacterMovement()
    {
        rb.velocity = new Vector2(movX * litoSpeed, rb.velocity.y);

        if (pj.IsAvionlito) // Si el personaje es Avionlito, su velocidad cambia
        {
            rb.velocity = new Vector2(lookingAt * AvionlitoSpeed, -1f); //velocidad constante cuando te toca el avion
        }
    }

    /*void OutOfTheWater()
    {
        jumpOutOfTheWater = true;
        rb.AddForce(Vector2.up * JumpForce * 2.5f, ForceMode2D.Impulse);
    }*/

    public void StatChange()
    {
        // Si Lito está en su estado de Lito, se le asignan las variables predeterminadas
        if (!pj.IsBarlito && !pj.IsAvionlito)
        {
            speed = litoSpeed;
            JumpForce = litoSpeed;
            rb.gravityScale = 2;
        }

        // Si Lito es un barco, verifico si está en el agua o no, y le asigno su velocidad y gravedad
        if (pj.IsBarlito)
        {
            if(pj.water && speed < BarlitoWaterMaxSpeed) 
            {
                speed += BarlitoWaterAcceleration * Time.deltaTime;
            }
            else{
                speed = BarlitoSpeed;
            }
            JumpForce = BarlitoJump;
            rb.gravityScale = 2.5f;
        }
    }
}
