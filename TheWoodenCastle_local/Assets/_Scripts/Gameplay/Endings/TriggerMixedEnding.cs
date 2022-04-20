using UnityEngine;

public class TriggerMixedEnding : TriggerEnding
{
    //Works as a flag inside the onCollisionEnter2D method
    bool isActivated = false;

    /// <summary>
    /// Call to activate the mixed ending sequence
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            if (GameEvents.S != null)
            {
                GameEvents.S.OnMixedEnding();

                PlayerBehaviour.S.PlayerActive = false;

                isActivated = true;
            }
            else { Utils.PrintMissingComponentMsg("GameEvents script", this); }
        }
    }
}
