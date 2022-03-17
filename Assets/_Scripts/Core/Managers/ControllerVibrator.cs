using UnityEngine;

#if UNITY_STANDALONE_WIN
using XInputDotNetPure;
#endif

/* CLASS DOCUMENTATION *\
 * 
 * [Variable Specifics]
 * Inspector values: vibrationTime MUST be set from the inspector
 * Dynamically changed: These variables dynamically change throughout the game
 * 
 * [Class Flow]
 * 1. The only entry point of the class is the Vibrate() method subsribed to the onVibrateCall game event.
 *  1a. This event is triggered only when the player attack is blocked to vibrate the controller.
 * 
 * [Must Know]
 * 1. The controller gets automatically detected
 */

public class ControllerVibrator : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    [Header("Set in inspector")]
    [SerializeField] float vibrationTime = 0.3f; //For how much time we want the vibration to last
    float vibrateRate = 0; //How strong should the vibration be

    #region Reference Variables
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    #endregion

    private void Start()
    {
        GameEvents.S.onVibrateCall += Vibrate;
    }

    private void FixedUpdate()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        GamePad.SetVibration(playerIndex, vibrateRate, vibrateRate);
    }

    private void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (vibrateRate >= 0)
        {
            vibrateRate -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Called from onVibrateCall action event and vibrates ONLY if there is a controller connected from the start
    /// </summary>
    public void Vibrate()
    {
        if (!playerIndexSet) return;

        vibrateRate = vibrationTime;
    }

    private void OnDestroy()
    {
        GameEvents.S.onVibrateCall -= Vibrate;
    }
#endif

}