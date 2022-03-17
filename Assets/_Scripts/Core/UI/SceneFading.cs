using UnityEngine;

[DefaultExecutionOrder(800)]
public class SceneFading : MonoBehaviour,
    IFadeable
{
    static Animator fadeImageAnimator; //The animator attached to the GameObject

    private void Awake()
    {
        //Cache the animator component
        if ((fadeImageAnimator = GameObject.Find("SceneFadeImage").GetComponent<Animator>()) != true)
        { Utils.PrintMissingComponentMsg("SceneFadeImage animator component", this); }

        //Sub the method to the needed event
        GameEvents.S.onGameSceneChanged += OnSceneLoadFading;
    }

    /// <summary>
    /// Fade out when the scene loads
    /// </summary>
    public void OnSceneLoadFading(GameScenes state)
    {
        FadeOut();
    }

    /// <summary>
    /// Call to play the Fade out animation
    /// </summary>
    public void FadeOut()
    {
        fadeImageAnimator.Play("SceneFadeOut");
    }

    /// <summary>
    /// Call to play the Fade in animation
    /// </summary>
    public void FadeIn()
    {
        fadeImageAnimator.Play("SceneFadeIn");
    }

    private void OnDestroy()
    {
        GameEvents.S.onGameSceneChanged -= OnSceneLoadFading;
    }
}
