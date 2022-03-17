using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 *
 * [Variable Specifics]
 * Inspector values: The values must be set from within the editor inspector for the script to work correctly
 * 
 * [Class Flow]
 * 1. This script works only as a DATA CONTAINER for each scene.
 * 
 * [Must Know]
 * 1. LevelData script is loaded FIRST every time a new scene loads, and that's because GameManager and AudioMaster
 *  grab the Audio clip and GameState info from this script to determine which methods to execute.
 */

[DefaultExecutionOrder(50)]
public class LevelData : MonoBehaviour
{
    public static LevelData S;

    [Header("Set in inspector")]
    [SerializeField] MainAudioClips sceneAudioTheme; // The audio theme clip of THIS scene
    [SerializeField] GameScenes thisScenesInfo; // The game scene info

    private void Awake()
    {
        S = this;
    }

    public MainAudioClips GetSceneAudioClipInfo()
    {
        return sceneAudioTheme;
    }

    public GameScenes GetSceneGameStateInfo()
    {
        return thisScenesInfo;
    }

    private void OnDestroy()
    {
        S = null;
    }

}
