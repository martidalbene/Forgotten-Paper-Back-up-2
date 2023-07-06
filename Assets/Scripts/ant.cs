using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ant : MonoBehaviour
{

    [SerializeField] float vel; // Velocidad del Enemigo
    [SerializeField] float rayCastDist; // Distancia de Detección del rayo hacia abajo
    [SerializeField] float rayCastWallDist;

    public LayerMask contactsPiso; // Variable que me facilita el reconocimiento de las layers que el raycast debe detectar
    public LayerMask contactWall;
    public Transform rayPoint; // Variable para controlar el lugar donde está el raycast para detectar el piso
    public Transform rayView;
    //private Animator animator; // Variable que le da "vida" al enemigo

    AudioSource audioSource;

    Rigidbody2D rb;

    [SerializeField] private bool orientation;

    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();

        //audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // Detecto cuando el enemigo llega al borde de una plataforma y lo invierto para que no caiga
        if (!onGround() || canSeeWall())
        {
            transform.eulerAngles += new Vector3(0, 180, 0);
            orientation = !orientation;
            Debug.Log("No estoy sobre el piso");
        }
        // Patrullo
        else
        {
            walk();
            //animator.SetBool("isWalking", true);
        }
        
    }

    private void OnDrawGizmos()
    {
        // Dibujo el Raycast para patrullar
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rayPoint.position, rayPoint.position - transform.up * rayCastDist);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rayView.position, rayView.position - transform.right * rayCastWallDist);
    }

    // Función con la que controlo el patrullaje del enemigo
    private void walk()
    {
        if (orientation)
        {
            rb.velocity = new Vector2(vel, 0);
        }
        else
        {
            rb.velocity = new Vector2(-vel, 0);
        }
    }

    private bool onGround()
    {
        bool val = false;

        float distance = rayCastDist;
        
        Vector2 end = rayPoint.position - Vector3.up * distance;

        // Creo el raycast para detectar cuando el enemigo está caminando sobre una plataforma y cuando llegó al borde de la misma
        RaycastHit2D sobrePiso = Physics2D.Raycast(rayPoint.position, -transform.up, rayCastDist, contactsPiso);

        if(sobrePiso.collider != null) val = true;
        else val = false;

        return val;

    }

    private bool canSeeWall()
    {
        bool val = false;

        float distance = rayCastWallDist;

        if(orientation)
        {
            distance = -rayCastWallDist;
        }

        Vector2 end = rayView.position + Vector3.right * distance;
        
        RaycastHit2D viendoPJ = Physics2D.Linecast(rayView.position, end, contactWall);

        if(viendoPJ.collider != null) val = true;
        else
        {
            val = false;
        }

        return val;
    }
}
