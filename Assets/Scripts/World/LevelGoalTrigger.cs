using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTrigger : MonoBehaviour
{
    public Action OnGoalReached;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        OnGoalReached?.Invoke();
    }
}
