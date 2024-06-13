using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerSound
{
    Jump
}
public class Lito : MonoBehaviour
{
    [SerializeField] private BasePlayerTransformation _baseForm;
    [SerializeField] private List<BasePlayerTransformation> _ownedTransformations = new List<BasePlayerTransformation>();
    [SerializeField] private List<BasePlayerTransformation> _availableTransformations = new List<BasePlayerTransformation>();
    [SerializeField, Range(0.1f, 2)] private float _transformCooldown = 1;

    private Dictionary<LitoTransformationType, BasePlayerTransformation> _ownedTransformationsList = new Dictionary<LitoTransformationType, BasePlayerTransformation>();
    private Dictionary<LitoTransformationType, BasePlayerTransformation> _availableTransformationsList = new Dictionary<LitoTransformationType, BasePlayerTransformation>();

    // References
    private Animator _animator;
    private Rigidbody2D _rBody;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;

    // Current status values
    private ILitoTransformation _currentTransformation;
    private float _currentTransformCooldown;
    private int _currentTransformIndex = 0;
    private bool _canJump = true;
    private bool _isOnFloor = true;
    public bool _isOnWater = false;
    public bool _isOnDirtyWater = false;
    private bool _forceOutOfWater;

    // Public values
    public bool InputTransformation => Input.GetButtonDown("TransfApply");
    public float InputHorizontalAxis => Input.GetAxisRaw("Horizontal");
    public bool CanJump => _canJump && _isOnFloor;
    public bool CanTransform => _currentTransformCooldown <= 0;
    public bool IsLookingRight => _spriteRenderer.flipX;
    public LitoTransformationType CurrentTransformType => _currentTransformation.TransformationType;

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
        SubscribeToEvents();

        _animator = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _currentTransformation = _baseForm;
    }

    private void Start()
    {
        _currentTransformation.InitializeTransformation(_rBody, _audioSource);

        foreach (BasePlayerTransformation ownedtransformation in _ownedTransformations)
        {
            ownedtransformation.InitializeTransformation(_rBody, _audioSource);
            UIEvents.OnTransformationAdd(ownedtransformation.TransformationType, ownedtransformation.TransformationSprite, true);
            _ownedTransformationsList.Add(ownedtransformation.TransformationType, ownedtransformation);
        }

        foreach (BasePlayerTransformation transformation in _availableTransformations)
        {
            transformation.InitializeTransformation(_rBody, _audioSource);
            UIEvents.OnTransformationAdd(transformation.TransformationType, transformation.TransformationSprite, false);
            _availableTransformationsList.Add(transformation.TransformationType, transformation);
        }
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        if (_currentTransformCooldown > 0) _currentTransformCooldown -= delta;

        AnimationManagment();
        _currentTransformation?.UpdateTransformation(delta, InputHorizontalAxis, _isOnWater, _isOnFloor, IsLookingRight);

        // Input
        if (InputTransformation)
        {
            Transform();
            PlayerEvents.UpdatePlayerTransformation(CurrentTransformType);
        }
        if (Input.GetButtonDown("TransfPrev")) ChangeTransform(true);
        if (Input.GetButtonDown("TransfNext")) ChangeTransform(false);
        if (Input.GetButtonDown("Jump")) Jump();


        // Sprite flip
        if (InputHorizontalAxis > 0) _spriteRenderer.flipX = false;
        else if (InputHorizontalAxis < 0) _spriteRenderer.flipX = true;

        //dirtyWater();
    }

    private void FixedUpdate()
    {
        _currentTransformation?.FixedUpdateTransformation();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        PlayerEvents.OnItemGrab += ItemGrab;
        PlayerEvents.OnWaterTouch += OnWaterTouch;
    }

    private void UnsubscribeToEvents()
    {
        PlayerEvents.OnItemGrab -= ItemGrab;
        PlayerEvents.OnWaterTouch -= OnWaterTouch;

        foreach (BasePlayerTransformation transformation in _ownedTransformationsList.Values)
            transformation.OnForceTransformToBase -= Transform;
    }

    void AnimationManagment()
    {
        // Falling anim
        if (!_isOnFloor)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isFalling", true);
            return;
        }

        if (_isOnFloor) _animator.SetBool("isFalling", false);

        // Walking anim
        if (InputHorizontalAxis != 0) _animator.SetBool("isWalking", true);
        else _animator.SetBool("isWalking", false);
    }

    void Jump(bool playSound = true)
    {
        if (!CanJump) return;

        _animator.SetTrigger("isJumping");
        _canJump = false;        
        _rBody.AddForce(Vector2.up * _currentTransformation.TransformationJumpForce, ForceMode2D.Impulse);

        if (playSound) PlaySound(PlayerSound.Jump);
    }

    void Transform()
    {
        if (_ownedTransformationsList.Count == 0 || !CanTransform) return;

        _currentTransformCooldown = _transformCooldown;
        _audioSource.PlayOneShot(_currentTransformation.TransformationSound);

        // Disable previous transform animation and get selected one
        string previousTransform = $"{CurrentTransformType}";
        _animator.SetBool($"transformation{previousTransform}", false);

        // If coming from a transformation then return to normal
        if (CurrentTransformType != _baseForm.TransformationType)
        {
            TransformToNormal(_currentTransformation.TransformationCanStandOnWater);
            return;
        }

        _currentTransformation = _ownedTransformationsList.ElementAt(_currentTransformIndex).Value;
        _currentTransformation?.ExecTransformation(_isOnWater, _isOnFloor, IsLookingRight);

        // Get new transform data and apply animation
        _rBody.gravityScale = _currentTransformation.TransformationGravityScale;
        Jump(false);

        _animator.SetBool($"transformation{CurrentTransformType}", true);
    }

    void TransformToNormal(bool canStandOnWater = false)
    {
        _forceOutOfWater = canStandOnWater;
        _rBody.gravityScale = _baseForm.TransformationGravityScale;
        _currentTransformation = _baseForm;
        Jump(false);
        _animator.SetBool($"transformation{CurrentTransformType}", true);
    }

    void ChangeTransform(bool isPrevious)
    {
        if (_ownedTransformationsList.Count == 0) return;

        if (isPrevious)
        {
            if (_currentTransformIndex + 1 > _ownedTransformationsList.Count - 1)
                _currentTransformIndex = 0;
            else
                _currentTransformIndex++;
        }

        else
        {
            if (_currentTransformIndex > 0)
                _currentTransformIndex--;
            else 
                _currentTransformIndex = _ownedTransformationsList.Count - 1;
        }

        UIEvents.OnTransformationSwap(_ownedTransformationsList.ElementAtOrDefault(_currentTransformIndex).Key);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("OneWayPlatform"))
            _canJump = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("floor"))
        {
            _rBody.velocity = new Vector2(InputHorizontalAxis * _currentTransformation.TransformationSpeed, 0);
            _isOnFloor = true;
        }

        if (other.CompareTag("OneWayPlatform"))
        {
            _rBody.velocity = new Vector2(InputHorizontalAxis * _currentTransformation.TransformationSpeed, _rBody.velocity.y);
            _isOnFloor = true;
        }

        if (other.CompareTag("Water"))
            _isOnWater = true;

        if (other.CompareTag("DirtyWater"))
        {
            _isOnWater = true;
            _isOnDirtyWater = true;
        }

        if (other.CompareTag("Pencil"))
        {
            GameManager.Instance.recolectados++;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("floor") || collision.CompareTag("OneWayPlatform"))
            _isOnFloor = false;

        if (collision.CompareTag("Water"))
            _isOnWater = false;

        if (collision.CompareTag("DirtyWater"))
        {
            _isOnWater = false;
            _isOnDirtyWater = false;
        }
    }

    private void PlaySound(PlayerSound type)
    {
        switch (type)
        {
            case PlayerSound.Jump:
                _audioSource.PlayOneShot(_currentTransformation.TransformationJumpSound);
                break;
            default:
                break;
        }
    }

    private void ItemGrab(ItemType type)
    {
        switch (type)
        {
            case ItemType.Barlito: GetTransformation(LitoTransformationType.Barlito); return;
            case ItemType.Avionlito: GetTransformation(LitoTransformationType.Avionlito); return;
        }
    }

    private void GetTransformation(LitoTransformationType transformationType)
    {
        if (_ownedTransformationsList.ContainsKey(transformationType) || !_availableTransformationsList.ContainsKey(transformationType)) return;

        _availableTransformationsList.TryGetValue(transformationType, out BasePlayerTransformation transformation);
        _ownedTransformationsList.Add(transformation.TransformationType, transformation);
        transformation.OnForceTransformToBase += Transform;

        UIEvents.OnTransformationObtained(transformationType);
        if (_ownedTransformationsList.Count == 1) UIEvents.OnTransformationSwap(_ownedTransformationsList.ElementAtOrDefault(_currentTransformIndex).Key);
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

    private void OnWaterTouch()
    {
        if (!_forceOutOfWater && _isOnWater)
            BackToSpawnPoint();

        else if (_isOnWater)
        {
            if (IsLookingRight) _rBody.AddForce(Vector2.up + -Vector2.right, ForceMode2D.Impulse);
            else _rBody.AddForce((Vector2.up + Vector2.right) * 10, ForceMode2D.Impulse);
            _forceOutOfWater = false;
            _isOnWater = false;
        }
    }

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
}
