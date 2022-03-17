using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(600)]
public class MainMenuActions : MonoBehaviour
{
    [Header("Set in Inpsector")]
    [SerializeField] RectTransform mainMenuPanel; //Main menu panel in main menu scene
    [SerializeField] RectTransform optionsPanel; //Options panel in main menu scene
    [SerializeField] Button startButton;//Start button in the main menu scene

    [SerializeField] AudioSource clickAudioSource; //Button click audio source
    [SerializeField] Slider musicSlider; //The options music slider

    private void Start()
    {
        SetMenuDefaults();
    }

    /// <summary>
    /// Call to set the Main Menu scene defaults
    /// </summary>
    void SetMenuDefaults()
    {
        //Set the main menu music slider to the starting value from the audio master
        musicSlider.value = AudioMaster.S.defaultAudioVolume;

        //Set Panel default states
        mainMenuPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// When the Start Button gets pressed in the main menu
    /// load the 1st level
    /// </summary>
    public void StartGame()
    {
        SceneLoader.S.LoadLevel(GameScenes.InTutorialScene);
    }

    /// <summary>
    /// Deactivate the MainMenu panel and activate the Options panel
    /// </summary>
    public void GoToSettings()
    {
        mainMenuPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(true);

        //Select the music slider when on options panel
        musicSlider.Select();
    }

    /// <summary>
    /// Deactivate the Options panel and activate the MainMenu panel
    /// </summary>
    public void GoToMainMenu()
    {
        mainMenuPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);

        //Select the start button on the main menu panel
        startButton.Select();
    }

    /// <summary>
    /// When Quit gets pressed terminate the application
    /// </summary>
    public void QuitGame()
    {
        //Exit the application
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Call to play the button click sound
    /// </summary>
    public void PlayClickSound()
    {
        clickAudioSource.Play();
    }

    /// <summary>
    /// Call to set the main audio source volume to given value
    /// </summary>
    public void SetVolume(float value)
    {
        AudioMaster.S.SetMasterVolume(value);
    }
}