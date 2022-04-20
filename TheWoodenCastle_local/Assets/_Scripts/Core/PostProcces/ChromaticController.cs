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
 * 1. The ONLY entry point of this class is the ChromaticPulse() method which is called everytime the player gets damaged.
 * 
 * [Must Know]
 * 1. The class uses a SHARED post process profile not a dynamically created one.
 */

public class ChromaticController : PP_Modifier
{
    public static ChromaticController S;

    ChromaticAberration outSetting; //The ChromaticAberration settin

    bool isPulsing = false;

    protected override void Awake()
    {
        S = this;
    }

    protected override void Start()
    {
        //Cache the post process volume present in the scene
        if (mainPPVolume = GetComponent<PostProcessVolume>())
        {
            mainPPVolume.sharedProfile.TryGetSettings<ChromaticAberration>(out outSetting);
        }
        else { Utils.PrintMissingComponentMsg("PostProcessVolume ", this); }
    }

    protected override void Update()
    {
        //If we are still pusling
        if (isPulsing)
        {
            LowerValue();
        }
    }

    /// <summary>
    /// Call to lower the intensity value by Time.deltaTime
    /// </summary>
    protected override void LowerValue()
    {
        outSetting.intensity.value -= Time.deltaTime;

        if (outSetting.intensity.value <= 0)
        {
            outSetting.intensity.value = 0f;
            isPulsing = false;
        }
    }

    /// <summary>
    /// Call to set the chromatic intensity value to 1f 
    /// </summary>
    public void ChromaticPulse()
    {
        outSetting.intensity.value = 1f;

        isPulsing = true;
    }

    protected override void OnDestroy()
    {
        S = null;
    }
}
