using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Set Dynamically")]
    [SerializeField] RectTransform pausePanel; //The pause panel GameObject
    [SerializeField] Slider musicSlider; //The musicvolume slider at the in-game pause menu
    [SerializeField] Button returnButton; //Cached only for the single use of dynamically selecting it

    bool canPause = true; //Can we pause?
    bool isPaused = false; //Is the game paused right now?


    private void Start()
    {
        GameEvents.S.onGameSceneChanged += CheckCurrentScene;

        //When the player dies, this method gets called
        GameEvents.S.onPauseMenuStateChange += SetCanPauseState;

        GetReferences();

        SetDefaults();

        if (returnButton == null)
        {
            Utils.PrintMissingComponentMsg("ReturnButton in pausePanel", this);
        }
    }

    void CheckCurrentScene(GameScenes scene)
    {
        if (scene == GameScenes.InTutorialScene || scene == GameScenes.InWorstTutorial)
        {
            canPause = false;
        }
        else
        {
            canPause = true;
        }
    }

    /// <summary>
    /// Cache the needed component references
    /// </summary>
    void GetReferences()
    {
        pausePanel = GameObject.Find("PausePanel").GetComponent<RectTransform>();

        musicSlider = pausePanel.GetComponentInChildren<Slider>();
        returnButton = pausePanel.GetComponentInChildren<Button>();
    }

    /// <summary>
    /// Set the startup behaviour of the pause menu
    /// </summary>
    void SetDefaults()
    {
        musicSlider.value = AudioListener.volume;

        pausePanel.gameObject.SetActive(false);

        isPaused = false;
    }

    /// <summary>
    /// Set canPause to false so the Pause menu can't get enabled
    /// </summary>
    void SetCanPauseState(bool state)
    {
        canPause = state;
    }

    private void Update()
    {
        //If ESC or Start controller button gets pressed...
        if (canPause && Input.GetButtonDown("Cancel"))
        {
            //Check now to see if any controller is connected for cursor visibility
            GameManager.S.ControllerConnected = CheckForConnectedController();

            //Start the pausing sequence
            PauseSequence();
        }
    }

    /// <summary>
    /// Call to check if any controller is connected
    /// </summary>
    /// <returns>True if a controller is connected
    /// <para>False otherwise</para></returns>
    bool CheckForConnectedController()
    {
        bool tempBool = false;

        foreach (string element in Input.GetJoystickNames())
        {
            if (element.Length > 1)
            {
                tempBool = true;
                break;
            }
            else
            {
                tempBool = false;
                break;
            }
        }

        return tempBool;
    }

    /// <summary>
    /// Call to Toggle the pause panel ON/OFF and slow down/resume the time scale based on the isPaused state.
    /// <para>isPaused = true -> SlowDownTime</para>
    /// <para>isPaused = false -> ResumeTime</para>
    /// </summary>
    public void PauseSequence()
    {
        //Show/Unshow the pause panel
        TogglePausePanel();

        if (!isPaused) //If we are not in pause state
        {
            SlowDownTimeScale();

            if (GameManager.S.ControllerConnected)
            {
                returnButton.Select();
            }
        }
        else //If we are in pause state
        {
            ResumeTimeScale();
        }
    }

    /// <summary>
    /// Activate or deactivate the pause panel
    /// <para>Also controls the cursor visible and lockState</para>
    /// </summary>
    void TogglePausePanel()
    {
        pausePanel.gameObject.SetActive(!pausePanel.gameObject.activeSelf);

        if (pausePanel.gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;

            //If there is a controller connected do not display the cursor
            if (!GameManager.S.ControllerConnected)
            {
                Cursor.visible = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Call when you want to pause the game.
    /// <para>Also sets PlayerState.PlayerActive to false</para>
    /// </summary>
    void SlowDownTimeScale()
    {
        //Deactivate the player behaviour so we don't get any input
        PlayerBehaviour.S.PlayerActive = false;

        Time.timeScale = .1f;
        isPaused = true;
    }

    /// <summary>
    /// Call when you want to un-pause the game.
    /// <para>Also sets PlayerState.PlayerActive to true</para>
    /// </summary>
    void ResumeTimeScale()
    {
        Time.timeScale = 1;
        isPaused = false;

        if (SignDialogueSpace.CurrentSignState == SignState.Reading)
        {
            return;
        }
        else
        {
            PlayerBehaviour.S.PlayerActive = true;
        }
    }

    /// <summary>
    /// <para>UI use ONLY</para>
    /// Sets the master volume from the slider in the in-game menu
    /// </summary>
    /// <param name="value">Value passed from the in-game pause menu slider</param>
    public void SetVolume(float value)
    {
        AudioMaster.S.SetMasterVolume(value);
    }

    /// <summary>
    /// Call to Load the MainMenu scene
    /// </summary>
    public void QuitToMenu()
    {
        //Resume the time first so the game can continue
        ResumeTimeScale();

        //Load the MainMenu
        SceneLoader.S.LoadLevel(0);
    }

    private void OnDestroy()
    {
        GameEvents.S.onPauseMenuStateChange -= SetCanPauseState;
    }
}
