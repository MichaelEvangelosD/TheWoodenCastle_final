using UnityEngine;

public class TriggerWorstEnding : TriggerEnding
{
    bool isActivated = false;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            if (GameEvents.S != null)
            {
                GameEvents.S.OnWorstEnding();

                PlayerBehaviour.S.PlayerActive = false;

                isActivated = true;
            }
            else { Utils.PrintMissingComponentMsg("GameEvents script", this); }
        }
    }
}
