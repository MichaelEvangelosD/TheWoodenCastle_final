using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSuicideEnding : TriggerEnding
{
    bool isActivated = false;

    /// <summary>
    /// Call to activate the suicide ending sequence
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            if (GameEvents.S != null && PlayerBehaviour.S != null)
            {
                GameEvents.S.OnSuicideEnding();

                PlayerBehaviour.S.PlayerActive = false;

                isActivated = true;
            }
            else { Utils.PrintMissingComponentMsg("GameEvents script", this); }
        }
    }
}
