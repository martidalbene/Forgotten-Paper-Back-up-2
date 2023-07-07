using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColisionDetection : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player")
        {
            Player pj = collisionInfo.gameObject.GetComponent<Player>();
            if(pj.rb.velocity.y >= 0) pj.BackToSpawnPoint();
        }
    }
}
