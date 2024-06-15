using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject pauseMenu; // Referencia al menu de pausa
    public Text showTime;
    public int litoDeathsCounter = 0;
    public Text deathsCounter;
    public GameObject spawnMarinerito;    

    private bool firstTimeTouchingBoatTransformation = true;
    private bool firstTimeTouchingPlaneTransformation = true;

    public bool stillPlaying = true;
    private bool enteringNewScene = true;

    // new
    [Header("Pencils")]
    [SerializeField] private List<ItemPencil> _allPencilsInWorld = new List<ItemPencil>();

    // Values
    private float _currentTime = 0;
    private int _playTime;
    private int _colectedPencils = 0;

    // Events
    public Action OnPencilGrab;


    void Awake()
    {

        if (Instance == null)
            Instance = this;
        else Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }

    // Busco los objetos que necesito
    void Start()
    {
        OnPencilGrab += PlayerPencilGrab;
    }

    private void OnDestroy()
    {
        OnPencilGrab -= PlayerPencilGrab;
    }

    // Update is called once per frame
    void Update()
    {
        if(stillPlaying)
        {
            _currentTime += Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.Escape)) 
            {
                // Si se presiona Escape, y el menú de pausa está activo, llamo a la funcion Continue()
                if(pauseMenu.activeInHierarchy)
                {
                    Continue();
                }
                // Si no está activo, lo activo y paro el juego
                else
                {
                    Time.timeScale = 0f;
                    pauseMenu.SetActive(true);
                }     
            }

        }
        else
        {
            if(enteringNewScene && (SceneLoader.Instance.currentScene() == 3 || SceneLoader.Instance.currentScene() == 4))
            {
                GetNewTexts();
                enteringNewScene = false;
            }
        }

        UpdateTimer(_currentTime);
        UpdateDeathsCounter();
        
    }

    // Funcion que controla si el jugador decidió seguir jugando estando en el menu de pausa
    public void Continue()
    {
        Time.timeScale = 1f; // Vuelvo el tiempo de juego a la normalidad
        pauseMenu.SetActive(false); // Desactivo el menu de pausa
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

    private void UpdateDeathsCounter()
    {
        if(stillPlaying) deathsCounter.text = "Deaths: " + litoDeathsCounter.ToString();
        else deathsCounter.text = litoDeathsCounter.ToString();
    }

    private bool FinishPointsCalculator()
    {
        bool canBeGood = false;

        if(litoDeathsCounter < 10 && _playTime < 15) canBeGood = true;

        return canBeGood;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(FinishPointsCalculator())
            {
                SceneLoader.Instance.goToGoodEnding();
                stillPlaying = false;
            }
            else 
            {
                SceneLoader.Instance.goToBadEnding();
                stillPlaying = false;
            }
            
            
        }
    }

    private void GetNewTexts()
    {
        deathsCounter = GameObject.Find("Deaths").GetComponent<Text>();
        showTime = GameObject.Find("Time").GetComponent<Text>();
    }

    public void Reset()
    {
        Destroy(gameObject);
    }


    // new
    private void PlayerPencilGrab()
    {
        _colectedPencils += 1;
        UIEvents.OnPencilCountUpdate($"x{_colectedPencils}");
    }
}
