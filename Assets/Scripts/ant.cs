using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ant : MonoBehaviour
{

    [SerializeField] float vel; // Velocidad del Enemigo
    [SerializeField] float rayCastDist; // Distancia de Detección del rayo hacia abajo
    [SerializeField] float detectRangeX; // Rango de detección del Enemigo
    [SerializeField] float biteRange; // Rango de ataque del Enemigo
    [SerializeField] float noMoveRange; // Rango donde el enemigo se detiene por completo
    [SerializeField] Transform target; // Objeto a Detectar
    public bool gotHit = false; // Variable que detecta si el enemigo fue golpeado
    public int life = 5; // Vidas del Enemigo  

    public LayerMask contactsPiso; // Variable que me facilita el reconocimiento de las layers que el raycast debe detectar
    public LayerMask contactsPJ; // Variable que me facilita el reconomiento de las layers que el raycast debe detectar
    public Transform rayPoint; // Variable para controlar el lugar donde está el raycast para detectar el piso
    public Transform rayView; // Varieable para controlar el lugar donde empieza el raycast para detectar al jugador
    public Animator animator; // Variable que le da "vida" al enemigo


    Vector3 direction; // Variable que nos ayuda a controlar la distancia entre el personaje y el enemigo

    AudioSource audioSource;

    public AudioClip clipAtack;

    Vector3 testEnd;

    Rigidbody2D rb;

    private bool orientation = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        direction = target.position - transform.position; // Resto la posición del personaje a la posición del enemigo

        // Detecto cuando el enemigo llega al borde de una plataforma y lo invierto para que no caiga
        if (!onGround() && direction.magnitude > detectRangeX)
        {
            transform.eulerAngles += new Vector3(0, 180, 0);
            orientation = !orientation;
        }

        // Pregunto si el minero se encuentra dentro del rango de detección, pero el enemigo está en la plataforma de enfrente
        if(canSeePlayer() && !onGround())
        {
            animator.SetBool("isWalking", false);
            dontWalk();
        }
        
        // Pregunto si el minero está dentro del rango de detección, pero no está tan cerca como para morderlo
        if(canSeePlayer() && onGround())
        {
            Debug.Log("Siguiendo");
            walkingTowards();
            animator.SetBool("isWalking", true);
        }
        
        // Si el enemigo no está en rango, patrullo
        if(!canSeePlayer() && onGround())
        {
            walk();
            animator.SetBool("isWalking", true);
        }
        
        // Pregunto si el minero está lo suficientemente cerca como para ser mordido
        if(direction.magnitude < biteRange && canSeePlayer() && onGround())
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("isAttacking");
            if(direction.magnitude > noMoveRange) rb.velocity = new Vector2(vel * 0.1f, 0);
        }

        // Pregunto si el gusano sigue con vida
        if(life <= 0)
        {
            animator.SetBool("isDead", true);
        }
        
    }

    private void OnDrawGizmos()
    {
        // Dibujo el Raycast para patrullar
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rayPoint.position, rayPoint.position - transform.up * rayCastDist);

        // Dibujo Area de Detección del personaje
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRangeX);

        // Dibujo Area de ataque del enemigo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, biteRange);

        // Dibujo Area límite donde el enemigo no se moverá más
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noMoveRange);
    }


    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        // Pregunto si el enemigo se chocó contra una pared
        if(collisionInfo.gameObject.layer == 8 || collisionInfo.gameObject.layer == 13 || collisionInfo.gameObject.layer == 0)
        {
            orientation = !orientation;
            transform.eulerAngles += new Vector3(0, 180, 0);
        }
    }

    // Función con la que controlo el patrullaje del enemigo
    private void walk()
    {
        if (orientation)
        {
            rb.velocity = new Vector2(-vel, 0);
        }
        else
        {
            rb.velocity = new Vector2(vel, 0);
        }
    }
    
    private void dontWalk()
    {
        rb.velocity = new Vector2(0, 0);
    }

    // Función con la que controlo cuando el enemigo debe empezar a caminar hacia el jugador
    private void walkingTowards()
    {
        if (orientation)
        {
            rb.velocity = new Vector2(-vel * 0.5f, 0);
        }
        else
        {
            rb.velocity = new Vector2(vel * 0.5f, 0);
        }
        
    }

    // Función con la que controlo el tiempo de "Recuperación" del enemigo tras ser golpeado
    public void recoveryTime()
    {
        gotHit = false;
        animator.SetBool("isWalking", true);
    }

    // Funcion con la que desaparezco al enemigo si éste murió
    public void died()
    {
        Destroy(gameObject);
    }

    public void attackSound()
    {
        audioSource.PlayOneShot(clipAtack);
    }

    private bool canSeePlayer()
    {
        bool val = false;

        float distance = detectRangeX;

        if(orientation)
        {
            distance = -detectRangeX;
        }

        Vector2 end = rayView.position + Vector3.right * distance;
        
        RaycastHit2D viendoPJ = Physics2D.Linecast(rayView.position, end, contactsPJ);

        if(viendoPJ.collider != null) val = true;
        else
        {
            val = false;
            Debug.DrawLine(rayView.position, end, Color.blue);
        }

        return val; 

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
}
