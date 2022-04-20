using System.Collections.Generic;
using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Variable Specifics]
 * Inspector values: The clips and the startingAudioValue must be set from the inspector
 * Dynamically changed: Reference variables change dynamically through the managers and the scripts
 * 
 * [Class Flow]
 * 1. The entry point of the class is inside the Start() method. The manager automatically gets the audio 
 *      clip to assign in THIS level every time the level loads from the LevelData Script.
 * 2. Secondly when we enter the scene, the SetStartingVolume(...) is checking if we are in the main menu 
 *  through the GameState change event which triggers from the GameManager every time the scene changes. 
 *  2a. If we are in the main menu scene we set the main menu options panel volume slider to the startingAudioValue
 *      from the inspector.
 * 
 * [Must Know]
 * 1. When the MasterVolume property is set it automatically sets the AudioListener.volume to the given value
 * 2. SetMasterVolume() is called from the main menu AND in-game volume sliders.
 * 3. ClearAudioSource() is called from the UI_PanelFading so we nullify the audio source and the defeat sound can play
 *      with not obstructions.
 */

/// <summary>
/// All the available audio clips to assign in the main audio source
/// <para>The enum ints must be on a par with the clipList element indexes</para>
/// </summary>
public enum MainAudioClips
{
    MainMenuTheme = 0,
    TutorialTheme = 1,
    LevelTheme = 2,
    BestEndingTheme = 3,
    MixedEndingTheme = 4,
    WorstEndingTheme = 5,
}

[DefaultExecutionOrder(200)]
public class AudioMaster : MonoBehaviour
{
    public static AudioMaster S;

    [Header("Set in Inspector")]
    [SerializeField] List<AudioClip> clipList; //All the available theme audio clips
    [Range(0, 1)] public float defaultAudioVolume; //The volume in which the MainMenu scene starts with

    AudioSource mainAudioSource; //The audio source atatched to the AudioManager GameObject

    //Holds the AudioListener volume as a variable
    private static float _masterVolume = 0f;
    public float MasterVolume
    {
        get { return _masterVolume; }
        private set
        {
            _masterVolume = value;

            AudioListener.volume = MasterVolume;
        }
    }

    #region STARTUP_SETUP
    private void Awake()
    {
        S = this;

        //Cache the AudioSource component to the appropriate variable
        if ((mainAudioSource = GetComponent<AudioSource>()) == null)
        { Utils.PrintMissingComponentMsg("AudioSouce component", this); }
    }

    private void Start()
    {
        //Subsribe the SetStartingVolume() method to onGameSceneChanged event
        if (FindObjectOfType<GameEvents>() && GameEvents.S != null)
        { GameEvents.S.onGameSceneChanged += SetStartingVolume; }
        else
        { Utils.PrintMissingComponentMsg("GameEvents script", this); }

        //Cache the corresponding scene audio clip
        if (FindObjectOfType<LevelData>() && LevelData.S != null)
        { AssignAudioClip(LevelData.S.GetSceneAudioClipInfo()); }
        else
        { Utils.PrintMissingComponentMsg("LevelData script", this); }
    }
    #endregion

    /// <summary>
    /// Call to set the MasterVolume to the inspector given value.
    /// <para>Useable only for the MainMenu game state</para>
    /// </summary>
    void SetStartingVolume(GameScenes scene)
    {
        if (scene == GameScenes.InMainMenu)
        {
            SetMasterVolume(defaultAudioVolume);
        }
    }

    /// <summary>
    /// Set the MasterVolume property to the given value
    /// </summary>
    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
    }

    /// <summary>
    /// Call to set the main audio source clip to null
    /// </summary>
    public void ClearAudioSource()
    {
        mainAudioSource.clip = null;
    }

    /// <summary>
    /// Call to set the main audio source clip and then play it
    /// <para> Sets .loop to true</para>
    /// </summary>
    /// <param name="clipIdx">The clip to play</param>
    void AssignAudioClip(MainAudioClips clipIdx)
    {
        //If the provided clipID is greater than the clipList length
        //Throw a warning and stop the editor playback
        if ((int)clipIdx > clipList.Count - 1)
        {
            print("The given sound clip value exceeded the clipList length." +
                $"\nValue passed in AssignAudioClip() {(int)clipIdx}");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            return;
        }

        //Set the main audio soruce to the provided clip
        //Play it and sets its looping to true
        mainAudioSource.clip = clipList[(int)clipIdx];
        mainAudioSource.Play();
        mainAudioSource.loop = true;
    }

    private void OnDestroy()
    {
        S = null;

        //Unsub the methods to not get NullRef errors
        GameEvents.S.onGameSceneChanged -= SetStartingVolume;
    }
}