using System.Collections;
using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 * 
 * [Variable Specifics]
 * Inspector values: The heartFadeArray[] size MUST be set from the inspector.
 * 
 * [Class Flow]
 * 1. At the Start() method of this script the FadeInHearts(...) coroutine gets called to activate X hearts which correspond to the players' HEALTH VALUE
 * 2. The only public methods on this class is ActivateOneHeart() and DeactivateOneHeart() which are called when...
 *      1a. A chest drops a heart (ActivateOneHeart())
 *      1b. An enemy attacks the player (DeactivateOneHeart())
 */

[DefaultExecutionOrder(700)]
public class HealthVisuals : MonoBehaviour
{
    public static HealthVisuals S;

    [Header("Set Array Size ONLY")]
    public HeartFade[] heartFadeScripts;

    private void Awake()
    {
        S = this;

        FindHeartFadeReferences();
    }

    /// <summary>
    /// Call to populate heartFadeArray with all the available HeartFade scripts that are 
    /// ACTIVE in the scene
    /// </summary>
    void FindHeartFadeReferences()
    {
        for (int i = 0; i < 3; i++)
        {
            heartFadeScripts[i] = GameObject.Find($"PlayerHeart_{i + 1}").GetComponent<HeartFade>();
        }
    }

    private void Start()
    {
        StartCoroutine(FadeInHearts(PlayerBehaviour.S.PlayerHealth.Health, heartFadeScripts));
    }

    /// <summary>
    /// Call to set the alpha value of numberOfHearts elements in scriptArray to 1f
    /// starting from the Nth element in scriptArray
    /// </summary>
    /// <param name="numberOfHearts">How many hearts to set the alpha value in 1f</param>
    /// <param name="scriptArray">The array with the script references</param>
    IEnumerator FadeInHearts(int numberOfHearts, HeartFade[] scriptArray)
    {
        for (int i = numberOfHearts - 1; i >= 0; i--)
        {
            ActivateOneHeart();
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// Call to fade in the first faded heart from the RIGHT side of the screen
    /// <para>For external use</para>
    /// </summary>
    public void ActivateOneHeart()
    {
        for (int i = heartFadeScripts.Length - 1; i >= 0; i--)
        {
            if (heartFadeScripts[i].IsFaded == true)
            {
                heartFadeScripts[i].StartLerpToValue(1f);
                break;
            }
        }
    }

    /// <summary>
    /// Call to fade out the first non-faded heart from the LEFT side of the screen
    /// <para>For external use</para>
    /// </summary>
    public void DeactivateOneHeart()
    {
        for (int i = 0; i < heartFadeScripts.Length; i++)
        {
            if (heartFadeScripts[i].IsFaded == false)
            {
                heartFadeScripts[i].StartLerpToValue(0f);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        S = null;
    }
}
