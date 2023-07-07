using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Objetivo a seguir por la cámara
    public Vector3 offset; // Lo desfazada que estará la cámara siguiendo al objetivo
    public float smoothFactor; // La velocidad en la que la cámara seguirá al objetivo
    public bool onPoint = false;

    void FixedUpdate()
    {
        following();
    }

    void following()
    {
        if(target.position.y > 3 && target.position.y < 6 && !onPoint) offset.y = -.5f; // Si el jugador alcanza cierta altura, muevo la cámara hacia abajo
        else if(target.position.y > 6 && target.position.y < 10 && !onPoint) offset.y = -2f;
        else if(onPoint) offset.y = -8.7f;
        else offset.y = 5.4f; // Si no, la fijo en una altura específica
        
        // Calculo la posición en la que debería estar la cámara
        Vector3 targetPosition = target.position + offset; 
        Vector3 smoothCamera = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime); 

        transform.position = smoothCamera;
    }

    // Usar colliders para calcular las áreas donde cambiar el offset
}
