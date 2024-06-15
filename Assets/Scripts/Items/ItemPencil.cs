using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPencil : BaseItem
{
    public override void OnItemGrab(ItemType itemType)
    {
        GameManager.Instance.OnPencilGrab?.Invoke();
        UIEvents.OnRequestPlayUIAudio(GrabSound);
        Destroy(gameObject);
    }
}
