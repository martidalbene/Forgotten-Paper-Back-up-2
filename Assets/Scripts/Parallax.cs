using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float multiplierEffect; // Variable que controla cuánto se mueve la imagen de fondo

    Transform cameraTransform;
    Vector3 lastCameraPosition;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Controlo la nueva posición de la cámara
        Vector3 newPos = cameraTransform.position - lastCameraPosition;

        // Modifico la actual posición de la cámara
        transform.position += new Vector3(newPos.x * multiplierEffect, newPos.y * .5f, 0f);

        lastCameraPosition = cameraTransform.position;

        // Calculo la distancia a la cámara
        float distanceWithTheCamera = cameraTransform.position.x - transform.position.x;

        if (Mathf.Abs(distanceWithTheCamera) >= width)
        {
            var movement = distanceWithTheCamera > 0 ? width * 2f : width * -2f;
            transform.position = new Vector3(transform.position.x + movement, transform.position.y + movement, 0f);
        }
    }
}
