using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * DERIVES FROM 'PP_Modifier'
 * 
 * [Variable Specifics]
 * Inspector values: intensity and lowerTimeMultiplier MUST be set from the inspector
 * Dynamically changed: All the variables on this class are dynamically cached and changed throughout the game.
 * 
 * [Class Flow]
 * 1. The ONLY entry point of this class is the DistortLens() method which is called everytime the player attack gets blocked.
 * 
 * [Must Know]
 * 1. The class uses a SHARED post process profile not a dynamically created one.
 */

public class LensDistorter : PP_Modifier
{
    public static LensDistorter S;

    [Header("Set in inspector")]
    [SerializeField] float intensity = 0f; //The intensity of the distortion effect
    [SerializeField] float lowerTimeMultiplier = 10f; //How fast should we set the distortion back to normal

    LensDistortion outSetting; //The LensDistortion setting

    bool isDistrorting = false;

    protected override void Awake()
    {
        S = this;
    }

    protected override void Start()
    {
        //Cache the post process volume present in the scene
        if (mainPPVolume = GetComponent<PostProcessVolume>())
        {
            mainPPVolume.sharedProfile.TryGetSettings<LensDistortion>(out outSetting);
        }
        else { Utils.PrintMissingComponentMsg("PostProcessVolume ", this); }
    }

    protected override void Update()
    {
        //If we are still distorting the camera view
        if (isDistrorting)
        {
            LowerValue();
        }
    }

    /// <summary>
    /// Call to lower the intensity value by Time.deltaTime
    /// </summary>
    protected override void LowerValue()
    {
        outSetting.intensity.value -= Time.deltaTime * lowerTimeMultiplier;

        if (outSetting.intensity.value <= 0)
        {
            outSetting.intensity.value = 0f;
            isDistrorting = false;
        }
    }

    /// <summary>
    /// Call to set the lens distort intensity to inspector given value
    /// <para>Sets isDistrorting to true</para>
    /// </summary>
    public void DistortLens()
    {
        outSetting.intensity.value = intensity;

        isDistrorting = true;
    }

    protected override void OnDestroy()
    {
        S = null;
    }
}
