using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (PlayerEvents.PlayerCurrentTransformation != LitoTransformationType.Barlito)
            PlayerEvents.OnWaterTouch?.Invoke();

        /*
        else if(collision.gameObject.tag == "Player" && lito.IsBarlito)
        {
            lito.water = true;
            AudioManager.Instance.Play("water");
        }

        if (gameObject.tag == "DirtyWater" && collision.gameObject.tag == "Player")
        {
            lito.Dirty = true;
        }*/
    }

    /*
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerEvents.PlayerCurrentTransformation != LitoTransformationType.Barlito)
            PlayerEvents.OnWaterTouch?.Invoke();

    }*/
}
