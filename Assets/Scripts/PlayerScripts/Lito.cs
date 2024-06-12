using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lito : MonoBehaviour
{
    [SerializeField] private BasePlayerTransformation _baseForm;
    [SerializeField] private List<BasePlayerTransformation> _ownedTransformations = new List<BasePlayerTransformation>();

    // References
    private Animator _animator;
    private Rigidbody2D _rBody;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;

    // Current status values
    private Dictionary<LitoTransformationType, BasePlayerTransformation> _ownedTransformationsList;
    private ILitoTransformation _currentTransformation;
    private bool _canJump = true;
    private bool _forceOutOfWater;

    // Events
    public Action OnWaterTouch;
    public Action OnRequestTransformation;

    // Public values
    public float InputHorizontalAxis => Input.GetAxisRaw("Horizontal");
    public LitoTransformationType CurrentTransformType => _currentTransformation.TransformationType;

    public bool HasLito => _ownedTransformationsList.ContainsKey(LitoTransformationType.Lito);
    public bool HasBarlito => _ownedTransformationsList.ContainsKey(LitoTransformationType.Barlito);
    public bool HasAvionlito => _ownedTransformationsList.ContainsKey(LitoTransformationType.Avionlito);


    // old

    public LitoMovement pjMovement;

    public AnimationLito animLito;

    public bool water = false; // Controlador de si Lito tocó o no el agua
    public bool Dirty = false;
    private float dirtyTimer;

    public bool IsBarlito = false; // Controlo si estoy transformado en Barco

    public bool IsAvionlito = false; // Controlo si estoy transformado en Avion
    

    public int TransformTo; // Controlo en qué me voy a transformar
    
    public bool GrandpaIsTalking = false;

    public Transform spawnPoint;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _currentTransformation = _baseForm;
    }

    private void Start()
    {
        /*
        foreach (PlayerTransformation transformation in _ownedTransformations)
            _ownedTransformationsList.Add(transformation.TransformationType, transformation);*/
    }

    // Update is called once per frame
    private void Update()
    {
        float delta = Time.deltaTime;
        _currentTransformation?.UpdateTransformation(delta);

        // Input
        if (Input.GetKeyDown(KeyCode.Q)) TransformManagment();
        if (Input.GetKeyDown(KeyCode.E)) TransformToNormal();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();


        // Sprite flip & animation
        if (InputHorizontalAxis != 0)
        {
            _animator.SetBool("isWalking", true);
            if (InputHorizontalAxis > 0) _spriteRenderer.flipX = false;
            else if (InputHorizontalAxis < 0) _spriteRenderer.flipX = true;
        }
        else
            _animator.SetBool("isWalking", false);

        //dirtyWater();
    }

    private void FixedUpdate()
    {
        _rBody.velocity = new Vector2(InputHorizontalAxis * _currentTransformation.TransformationSpeed, _rBody.velocity.y);

    }

    void Jump()
    {
        if (!_canJump) return;

        _canJump = false;
        _rBody.AddForce(Vector2.up * _currentTransformation.TransformationJumpForce, ForceMode2D.Impulse);
    }

    void TransformManagment()
    {
        string previousTransform = $"{CurrentTransformType}";
        _animator.SetBool($"transformation{previousTransform}", false);

        switch (CurrentTransformType)
        {
            case LitoTransformationType.Lito:
                _currentTransformation = _ownedTransformations[1];
                break;
            case LitoTransformationType.Avionlito: TransformToNormal();
                break;
            case LitoTransformationType.Barlito: TransformToNormal();
                break;
        }

        _currentTransformation.ApplyTransformation();
        _animator.SetBool($"transformation{CurrentTransformType}", true);
        print($"Transform: {previousTransform} => {CurrentTransformType}");
    }

    void TransformToNormal()
    {
        _animator.SetBool($"transformation{CurrentTransformType}", false);
        _currentTransformation = _baseForm;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("OneWayPlatform"))
            _canJump = true;
    }




    // old

    /*
    private void dirtyWater()
    {
        if (Dirty)
        {
            dirtyTimer += Time.deltaTime;
            animLito.animator.SetBool("DirtyWater", true);
        }
        else
        {
            dirtyTimer = 0;
            animLito.animator.SetBool("DirtyWater", false);
        }

        if(dirtyTimer >= 5)
        {
            BackToSpawnPoint();
            dirtyTimer = 0;
        }
        else if(dirtyTimer < 0)
        {
            dirtyTimer = 0;
        }
    }*/

    public void BackToSpawnPoint()
    {
        GameManager.Instance.litoDeathsCounter++;
        transform.position = spawnPoint.transform.position;
        TransformTo = 0;
        water = false;
        IsBarlito = false;
        IsAvionlito = false;
        pjMovement.rb.velocity = new Vector2(0, pjMovement.rb.velocity.y); //reseteo velocidades en X y no en Y
        pjMovement.StatChange();
        animLito.TransformingLito();
    }

    public void PlayWind()
    {    
        AudioManager.Instance.Play("wind");
    }
    public void PlayWalk()
    {
        _audioSource.PlayOneShot(_currentTransformation.TransformationFootstepSound);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Pencil")
        {
            GameManager.Instance.recolectados++;
            Destroy(other.gameObject);
        }
    }
}
