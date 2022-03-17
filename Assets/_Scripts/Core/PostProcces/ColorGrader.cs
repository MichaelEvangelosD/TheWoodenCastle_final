using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Variable Specifics]
 * Dynamically changed: All the variables on this class are dynamically cached and changed throughout the game.
 * 
 * [Class Flow]
 * 1. The ONLY public methods on this class is SetToBlackWhite() and SetToColored().
 * 
 * [Must Know]
 * 1. The class uses a SHARED post process profile not a dynamically created one.
 */

public class ColorGrader : MonoBehaviour
{
    public static ColorGrader S;

    PostProcessVolume mainPPVolume; //The PP volume present in the scene
    ColorGrading outSetting; //The color grading setting

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        //Cache the post process volume present in the scene
        if (mainPPVolume = GetComponent<PostProcessVolume>())
        {
            mainPPVolume.sharedProfile.TryGetSettings<ColorGrading>(out outSetting);
        }
        else { Utils.PrintMissingComponentMsg("PostProcessVolume ", this); }
    }

    /// <summary>
    /// Call to set the saturation value to -100f.
    /// </summary>
    public void SetToBlackWhite()
    {
        outSetting.saturation.value = -100f;
    }

    /// <summary>
    /// Call to set the saturation value to 0f.
    /// </summary>
    public void SetToColored()
    {
        outSetting.saturation.value = 0f;
    }

    private void OnDestroy()
    {
        S = null;
    }
}
