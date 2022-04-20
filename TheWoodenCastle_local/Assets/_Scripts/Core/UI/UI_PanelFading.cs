using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Variable Specifics]
 * Dynamically changed: All the variables are cached and changed throughout the game.
 * 
 * [Class Flow]
 * 1. The entry point of this class is the NotifyPauseMenu() method which is called from onPlayerDeath game event.
 * 2. When called the Defeat UI images start lerping towards 1f alpha value.
 * 3. Lastly when the image lerping finishes canPressEsc is set to true thus the Player can press "Cancel" button 
 *      to load the main menu scene.
 * 
 * [Must Know]
 * 1. When canPressCancel is set to true we check inside the Update() method if we press the Cancel button (ESC/Start) 
 *      and through the SceneLoader script we Load the main menu scene.
 */

public class UI_PanelFading : MonoBehaviour,
    IColorLerpable
{
    public static UI_PanelFading S;

    Image backgroundImage; //The black background image
    TextMeshProUGUI textOnTop; //The red text on the fade panel
    TextMeshProUGUI promptText; //The white bottom text that prompts the player

    float lerpedAlpha;
    float lerpDuration = 7f; //How long it takes to complete the lerping
    float elapsedTime;

    //Helper variables
    bool startLerping = false;
    bool canPressCancel = false;

    //Ending triggers
    bool worstEndingTriggered = false;
    bool suicideEndingTriggered = false;
    bool suicideTriggered = false;

    #region STARTUP_SETUP
    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        //Subscribe the needed methods to their respective events 
        if (FindObjectOfType<GameEvents>() && GameEvents.S != null)
        {
            GameEvents.S.onPlayerDeath += LaunchPlayerDefeated;
            GameEvents.S.onBestEnding += LaunchBestScreen;
            GameEvents.S.onMixedEnding += LaunchMixedScreen;
            GameEvents.S.onWorstEnding += LaunchWorstScreen;
            GameEvents.S.onSuicideEnding += LaunchSuicideScreen;
        }
        else { Utils.PrintMissingComponentMsg("GameEvents script", this); }

        GetReferences();
        SetDefaults();
    }

    /// <summary>
    /// Call to cache the needed component refences
    /// </summary>
    void GetReferences()
    {
        backgroundImage = GameObject.Find("BlackBG").GetComponent<Image>();
        textOnTop = GameObject.Find("TextOnTop").GetComponent<TextMeshProUGUI>();
        promptText = GameObject.Find("PromptForEscText").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Call to set the default variable, component states and values
    /// </summary>
    void SetDefaults()
    {
        if ((backgroundImage || textOnTop || promptText) == true)
        {
            backgroundImage.gameObject.SetActive(false);
            textOnTop.gameObject.SetActive(false);
            promptText.gameObject.SetActive(false);
        }
        else { Utils.PrintMissingComponentMsg("A panel component ", this); }

        //Set boolean defaults
        startLerping = false;
        canPressCancel = false;
    }
    #endregion

    private void Update()
    {
        //If the prompt has been shown...
        if (canPressCancel)
        {
            //...we can press the cancel key to return to the main menu
            if (Input.GetButtonDown("Cancel"))
            {
                SceneLoader.S.LoadLevel(0);
            }
        }
    }

    private void FixedUpdate()
    {
        //If we called a lerp event (PlayerDeath, Best/Mixed/Worst endings or Suicide Sequence)
        if (!startLerping) return;

        //Start increasing the alpha value of the blackBG and textOnTop in -lerpDuration- from 0 to 1
        if (elapsedTime < lerpDuration)
        {
            LerpAlphaValue(0, 1);
            ApplyLerpedAlpha();

            elapsedTime += Time.deltaTime;
        }
        else
        {
            //Triggers when the player reaches the worst ending scene
            if (worstEndingTriggered)
            {
                StartCoroutine(WaitForSeconds(3f, LoadWorstTutorial));

                promptText.text = "Revealing the truth...";

                ShowPromptText();
                return;
            }

            //Triggers when the player reaches the suicide scene at the end of the worst ending
            if (suicideEndingTriggered)
            {
                StartCoroutine(WaitForSeconds(3f, SuicidePlayer));
                promptText.text = "Good Bye";
                return;
            }

            //When the alpha lerping finishes,
            //prompt the user to press ESC or Start to get back to the Main Menu
            promptText.text = "Return to main menu\n" +
                                "ESC or Start";

            ShowPromptText();
            canPressCancel = true;
        }
    }

    /// <summary>
    /// Call to wait for X seconds and then call the given callback method
    /// </summary>
    IEnumerator WaitForSeconds(float seconds, Action callbackMethod)
    {
        yield return new WaitForSeconds(seconds);

        callbackMethod();
    }

    /// <summary>
    /// Call to load the worst tutorial scene
    /// </summary>
    void LoadWorstTutorial()
    {
        SceneLoader.S.LoadLevel(GameScenes.InWorstTutorial);
    }

    /// <summary>
    /// Launch the player suicide sequence
    /// </summary>
    void SuicidePlayer()
    {
        if (suicideTriggered) return; //acts as a single use flag

        if (PlayerBehaviour.S != null)
        {
            //Play the suicide audio clip
            PlayerBehaviour.S.PlayerAudio.PlayAudio(PlayerAudioClips.Suicide);

            //Show the prompting text
            ShowPromptText();

            suicideTriggered = true;

            //Load the main menu after 5 secs
            StartCoroutine(WaitForSeconds(5f, LoadMenu));
        }
        else { Utils.PrintMissingComponentMsg("PlayerBehaviour script", this); }
    }

    /// <summary>
    /// Call to load the main menu scene
    /// </summary>
    void LoadMenu()
    {
        SceneLoader.S.LoadLevel(0);
    }

    #region IMAGE_LERPING
    /// <summary>
    /// Slowly lerp the lerpedAlpha value from 0 to 1
    /// <para>Internal use ONLY</para>
    /// </summary>
    public void LerpAlphaValue(float startValue, float endValue)
    {
        lerpedAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / lerpDuration);
    }

    /// <summary>
    /// Apply the new alpha to background and textOnTop alphas
    /// <para>Internal use ONLY</para>
    /// </summary>
    public void ApplyLerpedAlpha()
    {
        backgroundImage.color = new Color(0, 0, 0, lerpedAlpha);
        textOnTop.color = new Color(255f, 0, 0, lerpedAlpha);
    }
    #endregion

    #region DEFEATED_SEQUENCE
    /// <summary>
    /// Called from OnPlayerDeath event to fade the panel as Defeated 
    /// </summary>
    void LaunchPlayerDefeated()
    {
        SetTextOnTopValue("DEFEATED");

        //Disable the ingame pause menu
        NotifyPauseMenu();

        startLerping = true;

        //Stop the music playback
        ClearMainAudioSource();

        ActivateUI_Panel();
    }

    /// <summary>
    /// Call when you want to stop the main audio soruce from playing
    /// </summary>
    void ClearMainAudioSource()
    {
        AudioMaster.S.ClearAudioSource();
    }
    #endregion

    #region BEST_ENDING_SEQUENCE
    void LaunchBestScreen()
    {
        SetTextOnTopValue($"OUTCOME\n" +
                    $"------------\n" +
                    $"Subject #{PlayerPrefs.GetInt("playerDeaths")} brainwashed");

        NotifyPauseMenu();

        startLerping = true;

        ActivateUI_Panel();
    }
    #endregion

    #region MIXED_ENDING_SEQUENCE
    void LaunchMixedScreen()
    {
        SetTextOnTopValue($"OUTCOME\n" +
            $"------------\n" +
            $"Subject #{PlayerPrefs.GetInt("playerDeaths")} under re-evaluation");

        NotifyPauseMenu();

        startLerping = true;
        ActivateUI_Panel();
    }
    #endregion

    #region WORST_ENDING_SEQUENCE
    void LaunchWorstScreen()
    {
        SetTextOnTopValue($"OUTCOME\n" +
            $"------------\n" +
            $"Subject #{PlayerPrefs.GetInt("playerDeaths")} damaged");

        NotifyPauseMenu();

        startLerping = true;

        worstEndingTriggered = true;

        ActivateUI_Panel();
    }

    void LaunchSuicideScreen()
    {
        SetTextOnTopValue("You know what to do...\n" +
            $"Subject #{PlayerPrefs.GetInt("playerDeaths")}");

        NotifyPauseMenu();

        startLerping = true;

        suicideEndingTriggered = true;

        ActivateUI_Panel();
    }
    #endregion

    /// <summary>
    /// Call to change the textOnTop text to the given value
    /// </summary>
    /// <param name="value">Change text to display ...?</param>
    void SetTextOnTopValue(string value)
    {
        textOnTop.text = value;
    }

    /// <summary>
    /// Call to trigger the DisablePauseMenu game event
    /// </summary>
    void NotifyPauseMenu()
    {
        GameEvents.S.OnPauseMenuStateChange(false);
    }

    /// <summary>
    /// Call to set the background and textOnTop states to true
    /// </summary>
    void ActivateUI_Panel()
    {
        backgroundImage.gameObject.SetActive(true);
        textOnTop.gameObject.SetActive(true);
    }

    /// <summary>
    /// Set the promptText gameObject to true
    /// </summary>
    void ShowPromptText()
    {
        promptText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        S = null;

        //Unsub from the events to prevent NullReferences
        GameEvents.S.onPlayerDeath -= LaunchPlayerDefeated;
        GameEvents.S.onBestEnding -= LaunchBestScreen;
        GameEvents.S.onMixedEnding -= LaunchMixedScreen;
        GameEvents.S.onWorstEnding -= LaunchWorstScreen;
        GameEvents.S.onSuicideEnding -= LaunchSuicideScreen;
    }
}
