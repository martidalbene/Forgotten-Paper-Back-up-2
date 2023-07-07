using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Manager para controlar el cambio de escenas cuando sea necesario
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public Animator transition;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Debug.Log("Mas de un manager");
    }

    // Cargo la siguiente escena
    public void nextScene() 
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // Cargo la escena del Menú Principal
    public void mainMenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    // Cierro el juego
    public void exitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelIndex);
    }

}