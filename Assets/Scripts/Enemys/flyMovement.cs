using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyMovement : MonoBehaviour
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
}
