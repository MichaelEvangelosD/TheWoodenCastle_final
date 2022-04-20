using UnityEngine;

public class PlayerEvasion : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] float evasionCooldown; //Time between evasion triggering

    //Helper variables
    float evadeDoneTime;
    bool isEvading;
    [HideInInspector] public bool justEvaded;

    private void Start()
    {
        justEvaded = false;
    }

    /// <summary>
    /// Executes only if justEvaded is false
    /// <para>Call to move the player to evasion layer and set isEvading to true</para>
    /// </summary>
    public void StartEvade()
    {
        MoveToEvasionLayer();
        isEvading = true;

        evadeDoneTime = Time.time + evasionCooldown;
    }

    private void Update()
    {
        //Stop evading after the specified amount of time
        if (isEvading && Time.time > evadeDoneTime)
        {
            StopEvade();
        }
    }

    /// <summary>
    /// Set the player current active layer to CharacterLayers.EvasionLayer
    /// </summary>
    void MoveToEvasionLayer()
    {
        PlayerBehaviour.S.SetActiveLayer(CharacterLayers.EvasionLayer);
    }

    /// <summary>
    /// Call to set the player current active layer to CharacterLayers.Player
    /// <para>resets isEvading and justEvaded to false</para>
    /// </summary>
    void StopEvade()
    {
        MoveToPlayerLayer();

        isEvading = false;
        justEvaded = false;
    }

    /// <summary>
    /// Set the player current active layer to CharacterLayers.Player
    /// </summary>
    void MoveToPlayerLayer()
    {
        PlayerBehaviour.S.SetActiveLayer(CharacterLayers.Player);
    }
}
