using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform Platform;

    [SerializeField]
    private Transform start;

    [SerializeField]
    private Transform end;

    public float Speed;

    int Direction = 1;
    private void Update()
    {
        Vector2 target = CurrentMovementTarget();

        Platform.position = Vector2.Lerp(Platform.position, target, Speed * Time.deltaTime);

        float distance = (target - (Vector2)Platform.position).magnitude;

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
        if (Platform != null && start != null && end != null)
        {
            Gizmos.DrawLine(Platform.transform.position, start.position);
            Gizmos.DrawLine(Platform.transform.position, end.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
        Debug.Log("Hola");
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }
}
