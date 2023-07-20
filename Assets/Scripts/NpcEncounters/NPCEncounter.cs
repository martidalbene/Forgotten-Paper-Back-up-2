using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCEncounter : MonoBehaviour
{
    public GameObject dialoguePanel; // El fondo donde se muestra el texto
    public Text dialogueText; // El espacio de texto donde se muestra el díalogo
    public string[] dialogue; // Las distintas líneas de díalogo que se mostrarán
    private int index; // Índice para sabér qué linea mostrar

    public float wordSpeed; // Tiempo en el que se va a mostrar el texto
    public bool litoIsClose; // Variable que controla si Lito está o no dentro del área para tener la interacción

    private bool nextDialogue = false; // Variable que controla si ya se terminó de escribir la línea de díalogo para pasar a la siguiente
    private bool firstTimeWeMeet = true; // Para saber si es la primera vez que el jugador ingresa al área para interactuar
    public Lito pj;
    public GameObject NextDialogLetter;

    // Update is called once per frame
    void Update()
    {
        // Si Lito está en el área de interacción y es la primera vez que ingresa
        if(litoIsClose && firstTimeWeMeet)
        {
            pj.GrandpaIsTalking = true;
            dialoguePanel.SetActive(true); // Activo el panel
            StartCoroutine(Typing()); // Empiezo a escribir
            firstTimeWeMeet = false; // Modifico la variable para saber que ya pasó esa primera vez
        }
        // Si lito está en el área de interacción, pero no es la primera vez, es necesario precionar Enter para comenzar la interacción
        else if(litoIsClose && Input.GetKeyDown(KeyCode.Return) && !nextDialogue)
        {
            if(dialoguePanel.activeInHierarchy) {
                pj.GrandpaIsTalking = false;
                zeroText(); // Si el panel está activo, llamo una función para cerrar y vaciar todo
            }

            else // Si no está activo, lo activo y empiezo a escribir
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
                pj.GrandpaIsTalking = true;
            }
        }
        // Si estamos en mitad de una interacción, presionamos F, y la frase anterior se terminó de escribir, pasaremos a la siguiente linea de texto
        else if(Input.GetKeyDown(KeyCode.F) && nextDialogue && NextDialogLetter.activeInHierarchy)
        {
            NextLine();
        }

        if(dialogueText.text == dialogue[index]) 
        {
            NextDialogLetter.SetActive(true);
            nextDialogue = true; // Si ya se escribió toda la línea de texto, activo la variable que me permita pasar a la siguiente linea
        }
    }

    // Dejo el texto vacío, el índice en 0 y desactivo el panel donde se muestra
    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        pj.GrandpaIsTalking = false;
        nextDialogue = false;
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
        NextDialogLetter.SetActive(false);
        nextDialogue = false; // Evito que se pueda pasar a la siguiente

        // Pregunto si el índice está al final de la lista de lineas de texto
        if(index < dialogue.Length - 1)
        {
            index++; // Sumo uno al índice
            dialogueText.text = ""; // Vacio el texto mostrado
            StartCoroutine(Typing()); // Escribo la nueva linea
        }
        // Si lo está, entonces borro todo, porque ya no hay más que mostrar
        else
        {
            zeroText();
        }
    }


    // Si el jugador está dentro del área de interacción, activo la posibilidad de interactuar
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            litoIsClose = true;
        }
    }

    // Si el jugador sale, borro el texto, donde lo muestro y desactivo la posibilidad de interactuar
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            litoIsClose = false;
            zeroText();
        }
    }
}
