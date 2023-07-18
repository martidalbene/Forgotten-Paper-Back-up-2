using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform start;

    [SerializeField]
    private Transform end;

    public float Speed;

    int Direction = 1;
    private void Update()
    {
        Vector2 target = CurrentMovementTarget();

        transform.position = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        float distance = (target - (Vector2)transform.position).magnitude;

        if (distance <= 0.6f)
        {
            Direction *= -1;
        }
    }

    Vector2 CurrentMovementTarget()
    {
        if (Direction == 1)
        {
            return start.position;
        }
        else
        {
            return end.position;
        }
    }

    private void OnDrawGizmos()
    {
        //Solo se ve en Debugeo
        if (transform != null && start != null && end != null)
        {
            Gizmos.DrawLine(transform.position, start.position);
            Gizmos.DrawLine(transform.position, end.position);
        }
    }

    /*void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        collisionInfo.transform.SetParent(transform);
    }*/

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Feet") other.transform.parent.SetParent(transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Feet")
        {
            other.transform.parent.SetParent(null);
        }
    }
    
    /*void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Player")
        {
            collisionInfo.transform.SetParent(null);
        }
    }*/
}
