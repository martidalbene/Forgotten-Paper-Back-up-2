using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeColor : MonoBehaviour
{
    // Start is called before the first frame update
    private Player player;

    public Color startcolor, endcolor;

    public float speed;

    private float startTime;

    void Start()
    {
        player = GetComponent<Player>();
        startTime = Time.time;
    }

    public void FadeToGreen()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(startcolor, endcolor, (Time.time - startTime) * speed);
    }

    public void FadeToWhite()
    {
        gameObject.GetComponent<SpriteRenderer>().color = startcolor;
    }

}
