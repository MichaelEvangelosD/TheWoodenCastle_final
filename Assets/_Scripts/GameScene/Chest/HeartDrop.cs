using UnityEngine;

public class HeartDrop : MonoBehaviour
{
    void Start()
    {
        UpdateHealthGUI();

        //Update the players health
        PlayerBehaviour.S.PlayerHealth.Health++;
    }

    /// <summary>
    /// Call to activate one heart at the HealthVisuals script
    /// </summary>
    void UpdateHealthGUI()
    {
        HealthVisuals.S.ActivateOneHeart();
    }
}
