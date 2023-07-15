using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player") 
        {
            LitoMovement lito = collisionInfo.gameObject.GetComponent<LitoMovement>();

            lito.BackToSpawnPoint();
        }
    }
}
