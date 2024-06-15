using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private BasePlayerTransformation _baseForm;
    [SerializeField] private List<BasePlayerTransformation> _ownedTransformations = new List<BasePlayerTransformation>();
    [SerializeField] private List<BasePlayerTransformation> _availableTransformations = new List<BasePlayerTransformation>();
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _windSound;
    [SerializeField, Range(0.1f, 2)] private float _transformCooldown = 1;

    // References
    private Animator _animator;
    private Rigidbody2D _rBody;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private PlatformEffector2D _currentPlatform;
    private BoxCollider2D _selfCollider;

    // Current status values
    private Dictionary<LitoTransformationType, BasePlayerTransformation> _ownedTransformationsList = new Dictionary<LitoTransformationType, BasePlayerTransformation>();
    private Dictionary<LitoTransformationType, BasePlayerTransformation> _availableTransformationsList = new Dictionary<LitoTransformationType, BasePlayerTransformation>();
    private ILitoTransformation _currentTransformation;
    private float _currentTransformCooldown;
    private int _currentTransformIndex = 0;
    private bool _canJump = true;
    private bool _isOnFloor = true;
    private bool _isOnWater = false;
    private bool _isOnDirtyWater = false;
    private bool _forceOutOfWater;
    private float _dirtyTimer;
    private bool _isControlEnabled = true;

    // Public values
    public bool InputPreviousTransformation => Input.GetButtonDown("TransfPrev");
    public bool InputNextTransformation => Input.GetButtonDown("TransfNext");
    public bool InputTransformation => Input.GetButtonDown("TransfApply");
    public float InputHorizontalAxis => Input.GetAxisRaw("Horizontal");
    public bool InputJump => Input.GetButtonDown("Jump");
    public bool InputFallOfPlatform => Input.GetKeyDown("s");
    public bool InputNextNPCDialogue => Input.GetKeyDown(KeyCode.F);
    public bool CanJump => _canJump && _isOnFloor;
    public bool CanTransform => _currentTransformCooldown <= 0;
    public bool IsLookingRight => _spriteRenderer.flipX;
    public LitoTransformationType CurrentTransformType => _currentTransformation.TransformationType;

    private void Awake()
    {
        SubscribeToEvents();

        _animator = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _selfCollider = GetComponent<BoxCollider2D>();

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
        if (InputNextNPCDialogue) NPCEncounterEvents.OnNPCNextDialogue?.Invoke();
        DirtyWater(delta);
        AnimationManagment();

        if (!_isControlEnabled)
        {
            _rBody.velocity = new Vector2(0, _rBody.velocity.y);
            return;
        }

        // Input
        if (InputTransformation)
        {
            Transform();
            PlayerEvents.UpdatePlayerTransformation(CurrentTransformType);
        }

        if (InputPreviousTransformation) ChangeTransform(true);
        if (InputNextTransformation) ChangeTransform(false);
        if (InputJump) Jump();
        if (InputFallOfPlatform && _currentPlatform != null) StartCoroutine(DisableCollision());

        if (_currentTransformCooldown > 0) _currentTransformCooldown -= delta;
        _currentTransformation?.UpdateTransformation(delta, InputHorizontalAxis, _isOnWater, _isOnFloor, IsLookingRight, _forceOutOfWater);


        // Sprite flip
        if (InputHorizontalAxis > 0) _spriteRenderer.flipX = false;
        else if (InputHorizontalAxis < 0) _spriteRenderer.flipX = true;
    }

    private void FixedUpdate()
    {
        if (!_isControlEnabled) return;

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
        PlayerEvents.OnForceOutOfWater += OnForceOutOfWater;
        PlayerEvents.OnEnemyDamage += BackToSpawnPoint;
        PlayerEvents.OnEnableDisableControls += EnableDisableControls;
    }

    private void UnsubscribeToEvents()
    {
        PlayerEvents.OnItemGrab -= ItemGrab;
        PlayerEvents.OnWaterTouch -= OnWaterTouch;
        PlayerEvents.OnForceOutOfWater -= OnForceOutOfWater;
        PlayerEvents.OnEnemyDamage -= BackToSpawnPoint;
        PlayerEvents.OnEnableDisableControls -= EnableDisableControls;

        foreach (BasePlayerTransformation transformation in _ownedTransformationsList.Values)
            transformation.OnForceTransformToBase -= TransformForceToNormal;
    }

    private void EnableDisableControls(bool enabled)
    {
        _isControlEnabled = enabled;
    }

    private void AnimationManagment()
    {
        // Falling anim
        if (!_isOnFloor)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isFalling", true);
            return;
        }

        if (_isOnFloor || _isOnWater) _animator.SetBool("isFalling", false);

        // Walking anim
        if (_isControlEnabled)
        {
            if (InputHorizontalAxis != 0) _animator.SetBool("isWalking", true);
            else _animator.SetBool("isWalking", false);
        }
        else
            _animator.SetBool("isWalking", false);
    }

    private void Jump(bool playSound = true)
    {
        if (!CanJump) return;

        _animator.SetTrigger("isJumping");
        _canJump = false;        
        _rBody.AddForce(Vector2.up * _currentTransformation.TransformationJumpForce, ForceMode2D.Impulse);

        if (playSound) _audioSource.PlayOneShot(_currentTransformation.TransformationJumpSound);
    }

    private void Transform()
    {
        if (_ownedTransformationsList.Count == 0 || !CanTransform) return;

        _currentTransformCooldown = _transformCooldown;
        _audioSource.PlayOneShot(_currentTransformation.TransformationSound);

        // Disable previous transform animation and get selected one
        _animator.SetBool($"transformation{CurrentTransformType}", false);
        _currentTransformation.EndTransformation();

        // If already transformed then go back to base
        if (CurrentTransformType != _baseForm.TransformationType)
        {
            TransformToNormal();
            return;
        }

        
        _currentTransformation = _ownedTransformationsList.ElementAt(_currentTransformIndex).Value;
        _currentTransformation?.ExecTransformation(_isOnWater, _isOnFloor, IsLookingRight);

        // Get new transform data and apply animation
        _rBody.gravityScale = _currentTransformation.TransformationGravityScale;
        _animator.SetBool($"transformation{CurrentTransformType}", true);
    }

    private void TransformForceToNormal()
    {
        if (_currentTransformation.TransformationType == _baseForm.TransformationType) return;
        _currentTransformCooldown = _transformCooldown;
        _audioSource.PlayOneShot(_currentTransformation.TransformationSound);

        _animator.SetBool($"transformation{CurrentTransformType}", false);
        TransformToNormal();
    }

    private void TransformToNormal()
    {
        _currentTransformation = _baseForm;        
        _rBody.gravityScale = _baseForm.TransformationGravityScale;        
        _currentTransformation?.ExecTransformation(_isOnWater, _isOnFloor, IsLookingRight);

        _animator.SetBool($"transformation{CurrentTransformType}", true);
    }

    private void ChangeTransform(bool isPrevious)
    {
        if (_ownedTransformationsList.Count == 0 || _currentTransformation.TransformationType != _baseForm.TransformationType) return;

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

        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            collision.gameObject.TryGetComponent(out PlatformEffector2D pEffector);
            if (pEffector != null) _currentPlatform = pEffector;
        }

        _currentTransformation?.PlayerColliderHit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("floor"))
        {
            _rBody.velocity = new Vector2(InputHorizontalAxis * _currentTransformation.TransformationSpeed, _rBody.velocity.y);
            _isOnFloor = true;
        }

        if (other.CompareTag("OneWayPlatform"))
        {
            _rBody.velocity = new Vector2(InputHorizontalAxis * _currentTransformation.TransformationSpeed, _rBody.velocity.y);
            _isOnFloor = true;
        }

        if (other.CompareTag("Water"))
        {
            _isOnWater = true;
            OnWaterTouch();
        }
        if (other.CompareTag("DirtyWater"))
        {
            _isOnWater = true;
            _isOnDirtyWater = true;
            OnWaterTouch();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("floor") || collision.CompareTag("OneWayPlatform"))
            _isOnFloor = false;

        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            collision.gameObject.TryGetComponent(out PlatformEffector2D pEffector);
            if (pEffector == _currentPlatform) _currentPlatform = null;
        }

        if (collision.CompareTag("Water") || collision.CompareTag("DirtyWater"))
        {
            _isOnWater = false;
            _isOnDirtyWater = false;
            _forceOutOfWater = false;
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
        transformation.OnForceTransformToBase += TransformForceToNormal;

        UIEvents.OnTransformationObtained(transformationType);
        if (_ownedTransformationsList.Count == 1) UIEvents.OnTransformationSwap(_ownedTransformationsList.ElementAtOrDefault(_currentTransformIndex).Key);
    }

    private void DirtyWater(float deltaTime)
    {
        if (_isOnDirtyWater)
        {
            _dirtyTimer += deltaTime;
            _animator.SetBool("DirtyWater", true);
        }
        else
        {
            _dirtyTimer = 0;
            _animator.SetBool("DirtyWater", false);
        }

        if(_dirtyTimer >= 5)
        {
            BackToSpawnPoint();
            _dirtyTimer = 0;
        }

        else if(_dirtyTimer < 0)
            _dirtyTimer = 0;
    }

    private void OnWaterTouch()
    {
        if (!_forceOutOfWater && _isOnWater && !_currentTransformation.TransformationCanStandOnWater)
            BackToSpawnPoint();
    }

    private void OnForceOutOfWater(bool force)
    {
        _forceOutOfWater = force;
    }

    private void BackToSpawnPoint()
    {
        TransformForceToNormal();
        GameManager.Instance.OnPlayerDeath?.Invoke(this);
    }

    public void PlayWind()
    {
        _audioSource.PlayOneShot(_windSound);
    }

    public void PlayWalk()
    {
        _audioSource.PlayOneShot(_currentTransformation.TransformationFootstepSound);
        
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = _currentPlatform.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(_selfCollider, platformCollider);
        yield return new WaitForSeconds(0.75f);

        Physics2D.IgnoreCollision(_selfCollider, platformCollider, false);
    }
}
