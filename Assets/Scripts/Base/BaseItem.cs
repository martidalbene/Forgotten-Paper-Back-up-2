using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Barlito, Avionlito, Pencil
}
public class BaseItem : MonoBehaviour, IItem
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private AudioClip _grabSound;

    public AudioClip GrabSound => _grabSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        OnItemGrab(_itemType);
    }

    public virtual void OnItemGrab(ItemType itemType)
    {
        PlayerEvents.OnItemGrab(itemType);
        UIEvents.OnRequestPlayUIAudio(_grabSound);
        Destroy(gameObject);
    }
}
