using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Barlito, Avionlito
}
public class BaseItem : MonoBehaviour, IItem
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private AudioClip _grabSound;
    private SpriteRenderer _spr;

    protected private virtual void Start()
    {
        _spr = GetComponent<SpriteRenderer>();
        _spr.sprite = _itemSprite;
    }

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
