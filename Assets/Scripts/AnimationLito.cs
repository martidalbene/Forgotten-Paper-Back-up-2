using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLito : MonoBehaviour
{
    public Player player; // Referencia del jugador
    public Animator animator; // Referencia del animator
    private Rigidbody2D rb; // Referencia del RigidBody

    private bool Mov;
    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        rb = player.rb;
    }

    // Update is called once per frame
    void Update()
    {
        // Si soy Lito, y me muevo, activo la caminata
        if (!player.IsAvionlito && !player.IsBarlito)
        {
            animator.SetBool("LitoIsWalking", Mov == true);
            animator.SetFloat("LitoVelocityY", rb.velocity.y);
            animator.SetFloat("LitoVelocityX", rb.velocity.x);
            PasiveAnimations();
        }

        WalkingAnim();
    }

    //Controlo las animaciones según corresponda
    void PasiveAnimations()
    {
        if (player.rb.velocity.y > 0)
        {
            animator.SetBool("LitoIsJumping", true);
            animator.SetBool("LitoIsFalling", false);
        }
        else if (player.rb.velocity.y < 0)
        {
            animator.SetBool("LitoIsJumping", false);
            animator.SetBool("LitoIsFalling", true);
        }
        else if (player.rb.velocity.y == 0)
        {
            animator.SetBool("LitoIsJumping", false);
            animator.SetBool("LitoIsFalling", false);
        }
    }

    // Reviso si debo ejecutar la animación de caminata
    private void WalkingAnim()
    {
        if(rb.velocity.x != 0)
        {
            Mov = true;
        }
        else
        {
            Mov = false;
        }
    }

    // Controlo en qué debe transformarse lito
    public void TransformingLito()
    {
        // Activo la animación de transición
        if(player.IsBarlito || player.IsAvionlito)
        {
            animator.SetTrigger("LitoTransforming");
        }
        

        switch (player.TransformTo)
        {
            case 1: // Activo la animación para transformarme en Avion
                animator.SetBool("TransformAvionlito", true);
                animator.SetBool("TransformBarlito", false);
                animator.SetBool("TransformLito", false);
                break;
            case 0: // Activo la animacion para volver a ser Lito
                if (!player.IsAvionlito)
                {
                    animator.SetBool("TransformLito", true);
                }
                else if (!player.IsBarlito)
                {
                    animator.SetBool("TransformLito", true);
                }
                break;
            case -1: // Activo la animación para transformarme en Barco
                animator.SetBool("TransformAvionlito", false);
                animator.SetBool("TransformBarlito", true);
                animator.SetBool("TransformLito", false);
                break;
            default:
                break;
        }
    }
}
