using UnityEngine;

public class Trampolin : MonoBehaviour
{
    [SerializeField] private float pushForce = 15;
    [SerializeField] private AudioClip _bounceAudioClip;
    private AudioSource _audio;
    private Animator _animation;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _animation = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (!collisionInfo.gameObject.CompareTag("Player")) return;

        collisionInfo.gameObject.TryGetComponent(out Rigidbody2D rBody);

        if (rBody != null)
        {
            _audio?.PlayOneShot(_bounceAudioClip);
            rBody.AddForce(Vector2.up * pushForce, ForceMode2D.Impulse);
            _animation?.SetTrigger("Push");
        }

        /*
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
        */
    }
}
