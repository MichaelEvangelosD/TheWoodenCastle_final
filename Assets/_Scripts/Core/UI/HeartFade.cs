using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(430)]
public class HeartFade : MonoBehaviour,
    IColorLerpable
{
    Image heartImage; //The heart image attached to the GameObject

    private bool _isFaded;
    public bool IsFaded
    {
        get { return _isFaded; }
        private set { _isFaded = value; }
    }

    //Lerping Variables
    bool startLerping = false;

    float endValue;
    float lerpedAlpha = 0f;
    float lerpDuration = 0.5f;
    float elapsedTime;

    private void Awake()
    {
        //Cache the image component reference
        if ((heartImage = GetComponent<Image>()) != true)
        { Utils.PrintMissingComponentMsg("Image component", this); }

        //Check if the heart is faded at the start of the scene
        //to update its isFaded boolean
        CheckFadeState();
    }

    private void FixedUpdate()
    {
        //Early exit
        if (!startLerping) return;

        if (elapsedTime < lerpDuration)
        {
            LerpAlphaValue(heartImage.color.a, endValue);
            ApplyLerpedAlpha();

            elapsedTime += Time.deltaTime;
        }
        else
        {
            CheckFadeState();

            elapsedTime = 0f;
            lerpedAlpha = 0f;

            startLerping = false;
        }
    }

    /// <summary>
    /// Call to set the heart's image color Alpha to endValue and Start lerping to it
    /// <para>Validates if the heart is faded or not by calling CheckFadeState()</para>
    /// </summary>
    /// <param name="endValue">Ranges from 0-1</param>
    public void StartLerpToValue(float endValue)
    {
        this.endValue = endValue;

        CheckFadeState();

        startLerping = true;
    }

    /// <summary>
    /// Call to check if the heart is faded or not based on its endValue
    /// </summary>
    void CheckFadeState()
    {
        if (endValue == 0f)
        { IsFaded = true; }
        else { IsFaded = false; }
    }

    #region INTERFACE_IMPLEMENTATION
    /// <summary>
    /// <para>Internal use ONLY</para>
    /// Slowly lerp the lerpedAlpha value from heartImage.color.a to endValue
    /// </summary>
    public void LerpAlphaValue(float startValue, float endValue)
    {
        lerpedAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / lerpDuration);
    }

    /// <summary>
    /// <para>Internal use ONLY</para>
    /// Call to set the heartImage's value to the lerpedAlpha value
    /// </summary>
    public void ApplyLerpedAlpha()
    {
        heartImage.color = new Color(1f, 1f, 1f, lerpedAlpha);
    }
    #endregion
}