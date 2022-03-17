using UnityEngine;

/* CLASS DOCUMENTATION *\
 *
 * [Class Flow]
 * 1. The FadeInOnSceneLoad() acts as an entry point for this script because it gets called 
 *  everytime a new scene loads.
 * 
 * [Must Know]
 * 1. Due to a Unity bug we have to ReApply the animator root motion every time the animations plays.
 *  Called from an animation event
 */

[DefaultExecutionOrder(750)]
public class SoundFading : MonoBehaviour,
    IFadeable
{
    static Animator fadeSoundAnimator; //The animator attached to THIS GameObject

    private void Awake()
    {
        //Cache the animator component reference
        if ((fadeSoundAnimator = gameObject.GetComponent<Animator>()) != true)
        { Utils.PrintMissingComponentMsg("Animator component", this); }

        //Subsribe the method that gets called at scene LOAD
        GameEvents.S.onGameSceneChanged += OnSceneLoadFading;
    }

    /// <summary>
    /// Call to fade in the sound when the scene loads
    /// </summary>
    public void OnSceneLoadFading(GameScenes state)
    {
        FadeIn();
    }

    /// <summary>
    /// Call to play the fade out animation from the sound source animator
    /// </summary>
    public void FadeOut()
    {
        fadeSoundAnimator.Play("SoundFadeOut");
    }

    /// <summary>
    /// Call to play the fade in animation from the sound source animator
    /// </summary>
    public void FadeIn()
    {
        fadeSoundAnimator.Play("SoundFadeIn");
    }

    /// <summary>
    /// Call to reset and apply the animator root motion
    /// <para>Unity bug</para>
    /// </summary>
    public void ReApplyRootMotion()
    {
        fadeSoundAnimator.applyRootMotion = true;
        fadeSoundAnimator.applyRootMotion = false;
    }

    private void OnDestroy()
    {
        GameEvents.S.onGameSceneChanged -= OnSceneLoadFading;
    }
}
