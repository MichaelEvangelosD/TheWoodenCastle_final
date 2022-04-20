using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Variable Specifics]
 * Dynamically changed: All the variables in this class are dynamically cached and changed throughout the game.
 * 
 * [Class Flow]
 * 1. The ONLY entry point of this class is the LoadLevel(scene index) method which is called from the either UI_PanelFading scipt
 *      OR the Usable doors at the end of each level, to load the next desired scene.
 * 
 * [Must Know]
 * 1. The levels are loaded asynchronously and not instantly.
 * 2. This class controls the Loading UI Panel state to be active or not
 */

[DefaultExecutionOrder(350)]
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader S;

    GameObject loadingPanel;
    SceneFading sceneFader;
    SoundFading soundFader;

    int nextSceneIdx;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        GetReferences();
        SetLoadingPanelState(false);
    }

    /// <summary>
    /// Cache the needed component references
    /// </summary>
    void GetReferences()
    {
        loadingPanel = GameObject.Find("LoadingPanel");
        sceneFader = GameObject.Find("SceneFadeImage").GetComponent<SceneFading>();
        soundFader = GameObject.Find("AudioManager").GetComponent<SoundFading>();
    }

    /// <summary>
    /// Use this method from any script to Load the next desired scene ASYNCHRONOUSLY.
    /// </summary>
    /// <param name="sceneToLoad">Scene to load next</param>
    public void LoadLevel(GameScenes sceneToLoad)
    {
        if (!(loadingPanel || sceneFader || soundFader))
        {
            Utils.PrintMissingComponentMsg("One panel", this, lineRef: 60);
            return;
        }

        //Cast the given GameScenes parameter to an int
        nextSceneIdx = (int)sceneToLoad;

        //Fade in the black image and fade out the music
        sceneFader.FadeIn();
        soundFader.FadeOut();

        //Start loading the given scene
        StartCoroutine(LoadAsyncScene(LoadingCoroutineCallBack));
    }

    /// <summary>
    /// Load the next desired scene asynchronously so we can display the Loading panel
    /// <para>Activates the Loading Panel</para>
    /// </summary>
    IEnumerator LoadAsyncScene(Action actionTrigger)
    {
        yield return new WaitForSeconds(1f);

        //Activate the loading UI panel
        SetLoadingPanelState(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneIdx);

        //This runs ONLY while the next scene is loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Notify the subscribed methods that the coroutine has finished
        actionTrigger();
    }

    /// <summary>
    /// Called when the LoadAsyncScene coroutine finishes to deactivate the LoadingPanel
    /// </summary>
    void LoadingCoroutineCallBack()
    {
        SetLoadingPanelState(false);
    }

    /// <summary>
    /// Toggles loadingPanel activeSelf state to given bool
    /// </summary>
    void SetLoadingPanelState(bool state)
    {
        loadingPanel.SetActive(state);
    }

    private void OnDestroy()
    {
        S = null;
    }
}
