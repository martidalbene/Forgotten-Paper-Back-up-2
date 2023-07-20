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

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
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
        
        currentTime += Time.deltaTime;

        if(unlockbarlito.hasBarlito) // Si tengo la transformación del barco, se lo indico al jugador
        {
            player.HasBarlito = true;
            UIBarlito.sprite = Barlito;
            spawnMarinerito.SetActive(true);
        }

        if (unlockavionlito.hasAvionlito) // Si tengo la transformación del avión, se lo indico al jugador
        {
            player.HasAvionlito = true;
            UIAvionlito.sprite = Avionlito;
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

        showTime.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
    }

    private void UpdatePencilCounter()
    {
        cantColeccionables.text = recolectados.ToString();
    }

    private void UpdateDeathsCounter()
    {
        deathsCounter.text = "Deaths: " + litoDeathsCounter.ToString();
    }

    private void FinishPointsCalculator()
    {
        
    }

}
