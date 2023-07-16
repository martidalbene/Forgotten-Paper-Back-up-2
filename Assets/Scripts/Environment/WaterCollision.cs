using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    private Lito lito;
    private LitoMovement litoMovement;

    private void Start()
    {
        lito = FindObjectOfType<Lito>();
        litoMovement = FindObjectOfType<LitoMovement>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !lito.IsBarlito)
        {
            lito.BackToSpawnPoint();
        }
        else if(collision.gameObject.tag == "Player" && lito.IsBarlito)
        {
            lito.water = true;
        }

        if (gameObject.tag == "DirtyWater" && collision.gameObject.tag == "Player")
        {
            lito.Dirty = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        lito.water = false;
        lito.Dirty = false;
    }
}
