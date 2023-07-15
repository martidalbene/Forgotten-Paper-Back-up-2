using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLito : MonoBehaviour
{
    public Lito pj; // Referencia del jugador
    public LitoMovement pjMovement;
    public Animator animator; // Referencia del animator
    private Rigidbody2D rb; // Referencia del RigidBody

    private bool Mov;
    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        rb = pjMovement.rb;
    }

    // Update is called once per frame
    void Update()
    {
        // Si soy Lito, y me muevo, activo la caminata
        if (!pj.IsAvionlito && !pj.IsBarlito)
        {
            animator.SetBool("LitoIsWalking", Mov);
            PasiveAnimations();
        }

        WalkingAnim();
    }

    //Controlo las animaciones según corresponda
    void PasiveAnimations()
    {
        if (pjMovement.rb.velocity.y > 0.1f && !pjMovement.canJump)
        {
            animator.SetBool("LitoIsJumping", true);
            animator.SetBool("LitoIsFalling", false);
        }
        else if (pjMovement.rb.velocity.y < -.5f && !pjMovement.canJump)
        {
            animator.SetBool("LitoIsJumping", false);
            animator.SetBool("LitoIsFalling", true);
        }
        else if (pjMovement.canJump)
        {
            animator.SetBool("LitoIsJumping", false);
            animator.SetBool("LitoIsFalling", false);
        }
    }

    // Reviso si debo ejecutar la animación de caminata
    private void WalkingAnim()
    {
        if(pjMovement.movX != 0)
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
        if(pj.IsBarlito || pj.IsAvionlito)
        {
            animator.SetTrigger("LitoTransforming");
        }
        

        switch (pj.TransformTo)
        {
            case 1: // Activo la animación para transformarme en Avion
                animator.SetBool("TransformAvionlito", true);
                animator.SetBool("TransformBarlito", false);
                animator.SetBool("TransformLito", false);
                break;
            case 0: // Activo la animacion para volver a ser Lito
                if (!pj.IsAvionlito)
                {
                    animator.SetBool("TransformLito", true);
                }
                else if (!pj.IsBarlito)
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
