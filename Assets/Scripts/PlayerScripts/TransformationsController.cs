using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationsController : MonoBehaviour
{
    public Lito pj;
    public LitoMovement pjMovement;

    public AnimationLito animLito; // Clase de Animación de Lito

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CharacterChange();
    }

    void CharacterChange()
    {
        if (Input.GetButtonDown("Transform Lito") && !pj.water && !pj.GrandpaIsTalking && (pj.IsBarlito || pj.IsAvionlito))
        {
            if(pjMovement.canJump) pjMovement.Jump();
            pj.TransformTo = 0;
            pj.IsBarlito = false;
            pj.IsAvionlito = false;
            pjMovement.rb.velocity = new Vector2(0, pjMovement.rb.velocity.y); //reseteo velocidades en X y no en Y
            pjMovement.StatChange();
            animLito.TransformingLito();
            AudioManager.Instance.Play("transform");
            AudioManager.Instance.Stop("wind");
        }

        if (Input.GetButtonDown("Transform Lito") && pj.water && !pj.GrandpaIsTalking && !pjMovement.jumpOutOfTheWater)
        {
            pjMovement.OutOfTheWater();
            AudioManager.Instance.Play("transform");
            AudioManager.Instance.Stop("wind");
        }

        if(Input.GetButtonDown("Transform Bar") && pj.HasBarlito == true && !pj.IsBarlito && !pj.GrandpaIsTalking) //&& inGround
        {
            pj.TransformTo = -1;
            pj.IsBarlito = true;
            pj.IsAvionlito = false;
            pjMovement.rb.velocity = new Vector2(0, pjMovement.rb.velocity.y);
            pjMovement.jumpOutOfTheWater = false;
            if(pjMovement.canJump) pjMovement.Jump();
            pjMovement.StatChange();
            animLito.TransformingLito();
            AudioManager.Instance.Play("transform");
            AudioManager.Instance.Stop("wind");
        }

        if(Input.GetButtonDown("Transform Avio") && pj.HasAvionlito == true && !pj.IsAvionlito && !pj.GrandpaIsTalking)
        {
            pj.TransformTo = 1;
            pj.IsBarlito = false;
            pj.IsAvionlito = true;
            if(pjMovement.canJump) pjMovement.Jump();
            pjMovement.StatChange();
            animLito.TransformingLito();
            AudioManager.Instance.Play("transform");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(pj.IsAvionlito && (collision.gameObject.tag == "floor" || collision.gameObject.tag == "OneWayPlatform"))
        {
            pjMovement.Jump();
            pj.TransformTo = 0;
            pj.IsBarlito = false;
            pj.IsAvionlito = false;
            pjMovement.rb.velocity = new Vector2(0, pjMovement.rb.velocity.y); //reseteo velocidades en X y no en Y
            pjMovement.StatChange();
            animLito.TransformingLito();
            AudioManager.Instance.Play("transform");
            AudioManager.Instance.Stop("wind");
        }
    }
}
