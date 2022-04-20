using UnityEngine;

[DefaultExecutionOrder(410)]
public class PlayerHealth : MonoBehaviour
{
    const int MAX_PLAYER_HP = 3; //The max constant value of the players HP

    private static int playerHealth;
    /// <summary>
    /// When Set, if value > MAX_PLAYER_HP set the playerHealth to MAX_PLAYER_HP
    /// <para>If not then set playerHealth to given value</para>
    /// </summary>
    public int Health
    {
        get { return playerHealth; }
        set
        {
            if (value > MAX_PLAYER_HP)
            {
                playerHealth = MAX_PLAYER_HP;
            }
            else
            {
                playerHealth = value;
            }
        }
    }

    //Damage visualization timer variables
    float showDamageDuration = 0.1f; //For how long to turn the player red when attacked
    bool showingDamage;
    float damageDoneTime;

    private void Awake()
    {
        GameEvents.S.onGameSceneChanged += SetStartingHealth;
    }

    /// <summary>
    /// Call to set the player starting health.
    /// <para>If we in the tutorial set the health to max health</para>
    /// <para>Else set it to the past scenes value</para>
    /// </summary>
    void SetStartingHealth(GameScenes state)
    {
        if (state == GameScenes.InTutorialScene)
        {
            Health = MAX_PLAYER_HP;
        }
        else
        {
            Health = playerHealth;
        }
    }

    private void Update()
    {
        //Stop showing damage if the showDamageDuration has been passed
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnshowDamage();
        }
    }

    /// <summary>
    /// Decrease player Health by 1 and play the hurt player audio clip
    /// <para>Calls HealthVisuals -> DeactivateOneHeart() to fade out one heart</para>
    /// </summary>
    public void DecreaseHealth()
    {
        Health--; //Decerase health

        //Fade out one heart
        HealthVisuals.S.DeactivateOneHeart();

        //Play the hurt audio clip
        PlayerBehaviour.S.PlayerAudio.PlayAudio(PlayerAudioClips.Hurt, true);

        //Pulse once when hit
        ChromaticController.S.ChromaticPulse();
    }

    /// <summary>
    /// Check the status of the player
    /// </summary>
    /// <returns>True if health <= 0, false if not</returns>
    public bool IsDead()
    {
        if (Health <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Turn the player sprite red to visualize damage for showDamageDuration number of seconds
    /// </summary>
    public void ShowDamage()
    {
        //Turn the player sprite red
        PlayerBehaviour.S.PlayerSprite.SpriteRenderer.color = new Color(255f, 0f, 0f, 200f);

        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    /// <summary>
    /// Turn the player sprite back to normal (white to show spirte colours)
    /// </summary>
    void UnshowDamage()
    {
        PlayerBehaviour.S.PlayerSprite.SpriteRenderer.color = Color.white;
        showingDamage = false;
    }

    private void OnDestroy()
    {
        GameEvents.S.onGameSceneChanged -= SetStartingHealth;
    }
}
