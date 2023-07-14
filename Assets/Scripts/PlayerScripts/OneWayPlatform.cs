using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatfrom;
    public BoxCollider2D playerCollider;

    // Update is called once per frame
    void Update()
    {
        // Si el jugador presionó S, Lito va a atravesar la plataforma
        if (Input.GetKey("s"))
        {
            if (currentOneWayPlatfrom != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reviso cuando el personaje tocó una plataforma
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatfrom = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reviso cuando el personaje dejó de tocar una plataforma
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatfrom = null;
        }
    }

    // Corrutina para saber cuándo tengo que deshabilitar la plataforma
    private IEnumerator DisableCollision()
    {
        Debug.Log(playerCollider);
        BoxCollider2D platformCollider = currentOneWayPlatfrom.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);

        yield return new WaitForSeconds(0.75f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
