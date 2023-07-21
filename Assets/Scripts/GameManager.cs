using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private UnlockBarlito unlockbarlito; // Controlo si tengo la tranformación de Barco
    private UnlockAvionlito unlockavionlito; // Controlo si tengo la transformación de Avion
    private Lito player; // Referencia al jugador

    public GameObject pauseMenu; // Referencia al menu de pausa

    public Text showTime;
    private float currentTime = 0;
    public Sprite Barlito;
    public Sprite Avionlito;
    public Image UIBarlito;
    public Image UIAvionlito;

    public Text cantColeccionables;
    public int recolectados = 0;

    public int litoDeathsCounter = 0;

    public Text deathsCounter;

    public GameObject spawnMarinerito;

    private int gamePlayTime;

    private bool firstTimeTouchingBoatTransformation = true;
    private bool firstTimeTouchingPlaneTransformation = true;

    public bool stillPlaying = true;

    private bool enteringNewScene = true;

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
        unlockbarlito = FindObjectOfType<UnlockBarlito>();
        player = FindObjectOfType<Lito>();
        unlockavionlito = FindObjectOfType<UnlockAvionlito>();
    }

    // Update is called once per frame
    void Update()
    {
        if(stillPlaying)
        {
            currentTime += Time.deltaTime;

            if(unlockbarlito.hasBarlito) // Si tengo la transformación del barco, se lo indico al jugador
            {
                player.HasBarlito = true;
                UIBarlito.sprite = Barlito;
                spawnMarinerito.SetActive(true);
                if(firstTimeTouchingBoatTransformation) 
                {
                    AudioManager.Instance.Play("newTransformation");
                    firstTimeTouchingBoatTransformation = false;
                }
            }

            if (unlockavionlito.hasAvionlito) // Si tengo la transformación del avión, se lo indico al jugador
            {
                player.HasAvionlito = true;
                UIAvionlito.sprite = Avionlito;
                if(firstTimeTouchingPlaneTransformation)
                {
                    AudioManager.Instance.Play("newTransformation");
                    firstTimeTouchingPlaneTransformation = false;
                } 
            }

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

        UpdateTimer(currentTime);
        UpdatePencilCounter();
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

        gamePlayTime = (int)minutes;

        showTime.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }

    private void UpdatePencilCounter()
    {
        cantColeccionables.text = recolectados.ToString();
    }

    private void UpdateDeathsCounter()
    {
        if(stillPlaying) deathsCounter.text = "Deaths: " + litoDeathsCounter.ToString();
        else deathsCounter.text = litoDeathsCounter.ToString();
    }

    private bool FinishPointsCalculator()
    {
        bool canBeGood = false;

        if(litoDeathsCounter < 10 && gamePlayTime < 15) canBeGood = true;

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
        cantColeccionables = GameObject.Find("Pencils").GetComponent<Text>();
        showTime = GameObject.Find("Time").GetComponent<Text>();
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

}
