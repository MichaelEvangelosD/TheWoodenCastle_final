using System;
using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Class Flow]
 * This class works just as a central hub to gather all the custom made game events.
 */
[DefaultExecutionOrder(100)]
public class GameEvents : MonoBehaviour
{
    public static GameEvents S;

    private void Awake()
    {
        S = this;
    }

    /// <summary>
    /// Called every time a new scene loads and passes in the new loaded scene 
    /// </summary>
    public event Action<GameScenes> onGameSceneChanged;
    public void OnGameSceneChanged(GameScenes newScene)
    {
        onGameSceneChanged?.Invoke(newScene);
    }

    /// <summary>
    /// Called from PlayerBehaviour when the player health falls below 0
    /// </summary>
    public event Action onPlayerDeath;
    public void OnPlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }

    /// <summary>
    /// Called from UI_PanelFading script when the player dies
    /// </summary>
    public event Action<bool> onPauseMenuStateChange;
    public void OnPauseMenuStateChange(bool state)
    {
        onPauseMenuStateChange?.Invoke(state);
    }

    /// <summary>
    /// Called from the TriggerBestEnding script
    /// </summary>
    public event Action onBestEnding;
    public void OnBestEnding()
    {
        onBestEnding?.Invoke();
    }

    /// <summary>
    /// Called from the TriggerMixedEnding script
    /// </summary>
    public event Action onMixedEnding;
    public void OnMixedEnding()
    {
        onMixedEnding?.Invoke();
    }

    /// <summary>
    /// Called from the TriggerWorstEnding script
    /// </summary>
    public event Action onWorstEnding;
    public void OnWorstEnding()
    {
        onWorstEnding?.Invoke();
    }

    /// <summary>
    /// Called from the TriggerSuicide script (Suicide Scene)
    /// </summary>
    public event Action onSuicideEnding;
    public void OnSuicideEnding()
    {
        onSuicideEnding?.Invoke();
    }

    /// <summary>
    /// Called from CoinDrop script when the chest drop is a coin
    /// </summary>
    public event Action onCoinPickup;
    public void OnCoinPickup()
    {
        onCoinPickup?.Invoke();
    }

    /// <summary>
    /// Called from an Enemy Behaviour script to increase player damage when killed
    /// </summary>
    public event Action onEnemyDeath;
    public void OnEnemyDeath()
    {
        onEnemyDeath?.Invoke();
    }

    /// <summary>
    /// Called from PlayerAttack when the hit is a block
    /// </summary>
    public event Action onVibrateCall;
    public void OnVibrateCall()
    {
        onVibrateCall?.Invoke();
    }
}
