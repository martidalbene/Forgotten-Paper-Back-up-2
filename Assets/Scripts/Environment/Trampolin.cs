using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolin : MonoBehaviour
{
    public float pushForce;

    // Controlo si el personaje tocó la parte superior del trampolin
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Player")
        {
            LitoMovement pj = collisionInfo.gameObject.GetComponent<LitoMovement>();
            if (pj.rb.velocity.y <= 0) pj.rb.AddForce(Vector2.up * pushForce, ForceMode2D.Impulse); // Ejerzo una fuerza sobre Lito, empujándolo hacia arriba
            AudioManager.Instance.Play("leaf");
            if(gameObject.tag == "Trampolin")
            {
                Animator anim = GetComponentInParent<Animator>();
                anim.SetTrigger("Push");
            }
            
        }
    }
}
