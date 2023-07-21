using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroToStory : MonoBehaviour
{
    public GameObject dialoguePanel; // El fondo donde se muestra el texto
    public Text dialogueText; // El espacio de texto donde se muestra el díalogo
    public string[] dialogue; // Las distintas líneas de díalogo que se mostrarán
    private int index; // Índice para sabér qué linea mostrar
    public float wordSpeed; // Tiempo en el que se va a mostrar el texto
    private bool nextDialogue = false; // Variable que controla si ya se terminó de escribir la línea de díalogo para pasar a la siguiente

    private float timeToWait = 10f;

    private float timer = 0;

    private bool firstTime = true;

    void Update()
    {
        
        timer +=  Time.deltaTime;

        if(firstTime)
        {
            dialoguePanel.SetActive(true); // Activo el panel
            StartCoroutine(Typing()); // Empiezo a escribir
            firstTime = false;
        }

        if(nextDialogue)
        {
            NextLine();
        }

        if(dialogueText.text == dialogue[index] && timer > timeToWait) 
        {
            nextDialogue = true; // Si ya se escribió toda la línea de texto, activo la variable que me permita pasar a la siguiente linea
            timer = 0;
        }
    }

    // Corrutina para mostrar la línea de diálogo actual

    IEnumerator Typing()
    {
        foreach(char letter in dialogue[index].ToCharArray()) // Por cada Letra en la frase a mostrar...
        {
            dialogueText.text += letter; // Sumo la letra al texto que muestro
            yield return new WaitForSeconds(wordSpeed); // Espero un tiempo a sumar la siguiente
        }
    }

    // Cambio de linea de diálogo
    public void NextLine()
    {
        nextDialogue = false; // Evito que se pueda pasar a la siguiente

        // Pregunto si el índice está al final de la lista de lineas de texto
        if(index < dialogue.Length - 1)
        {
            index++; // Sumo uno al índice
            dialogueText.text = ""; // Vacio el texto mostrado
            StartCoroutine(Typing()); // Escribo la nueva linea
        }
    }

    public void GoToGame()
    {
        SceneLoader.Instance.nextScene(); 
    }
}
