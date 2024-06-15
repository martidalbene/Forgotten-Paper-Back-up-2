using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player settings")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private LevelGoalTrigger _endLevelTrigger;

    [Header("Pencils")]
    [SerializeField] private List<ItemPencil> _allPencilsInWorld = new List<ItemPencil>(); // If for some reason this is needed

    // Values
    private int _playerDeathCounter = 0;
    private float _currentTime = 0;
    private int _playTime;
    private int _colectedPencils = 0;
    private bool _stillPlaying = true;
    private bool _enteringNewScene = true;
    private bool _isTimerPaused = false;

    // Events
    public Action<bool> OnGamePause;
    public Action OnPencilGrab;
    public Action<PlayerController> OnPlayerDeath;
    public Action OnLevelEnd;
    public Action<bool> OnPauseUnpauseTimer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        SubscribeToEvents();   
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        OnGamePause += PauseGame;
        OnPencilGrab += PlayerPencilGrab;
        OnPlayerDeath += PlayerDeath;
        OnPauseUnpauseTimer += PauseTimer;
        _endLevelTrigger.OnGoalReached += GoalReached;
    }

    private void UnsubscribeFromEvents()
    {
        OnGamePause -= PauseGame;
        OnPencilGrab -= PlayerPencilGrab;
        OnPlayerDeath -= PlayerDeath;
        OnPauseUnpauseTimer -= PauseTimer;
        _endLevelTrigger.OnGoalReached -= GoalReached;
    }

    private void Update()
    {
        if(_stillPlaying)
        {
            if (!_isTimerPaused) _currentTime += Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.Escape))
                OnGamePause?.Invoke(Time.timeScale != 0);
        }
        else
        {
            if(_enteringNewScene && (SceneLoader.Instance.currentScene() == 3 || SceneLoader.Instance.currentScene() == 4))
                _enteringNewScene = false;
        }

        UpdateTimer(_currentTime);
    }

    private void PauseGame(bool isGamePaused)
    {
        UIEvents.OnGamePaused(isGamePaused);

        if (isGamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    private void PauseTimer(bool isPaused)
    {
        _isTimerPaused = isPaused;
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);
        float minutes = Mathf.FloorToInt(currentTime / 60);

        _playTime = (int)minutes;

        string theTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        string latestTime = "";

        if (latestTime != theTime)
        {
            latestTime = theTime;
            UIEvents.OnPlayTimeUpdate(latestTime);
        }
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

    private void PlayerDeath(PlayerController playerRef)
    {
        _playerDeathCounter++;
        playerRef.gameObject.transform.position = _spawnPoint.position;
        UIEvents.OnUpdatePlayerDeathCounter($"Deaths: {_playerDeathCounter}");
    }

    private void PlayerPencilGrab()
    {
        _colectedPencils += 1;
        UIEvents.OnPencilCountUpdate($"{_colectedPencils}");
    }

    private void GoalReached()
    {
        if (_playerDeathCounter < 10 && _playTime < 15)
            SceneLoader.Instance.goToGoodEnding();
        else
            SceneLoader.Instance.goToBadEnding();

        _stillPlaying = false;
    }
}
