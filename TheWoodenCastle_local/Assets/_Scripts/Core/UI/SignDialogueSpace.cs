using UnityEngine;
using TMPro;

/// <summary>
/// The available sign reading states
/// </summary>
public enum SignState
{
    Inactive,
    Reading,
}

[DefaultExecutionOrder(850)]
public class SignDialogueSpace : MonoBehaviour
{
    public static TextMeshProUGUI InfoTextSpace; //The text space to display the sign info to
    static Animator infoSpaceAnimator; //The animator attached to the GameObject

    //The current reading state of the sign
    private static SignState _currentState;
    public static SignState CurrentSignState
    {
        get { return _currentState; }
        private set { _currentState = value; }
    }

    private void Start()
    {
        //Cache the component references
        InfoTextSpace = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        infoSpaceAnimator = GetComponent<Animator>();

        //Set default reading state
        CurrentSignState = SignState.Inactive;
    }

    /// <summary>
    /// Play the PullPanelUp animation and set CurrentSignState to Reading
    /// </summary>
    public static void PullPanelUp()
    {
        infoSpaceAnimator.Play("PullPanelUp");

        CurrentSignState = SignState.Reading;
    }

    /// <summary>
    /// Play the PushPanelDown animation and set CurrentSignState to Inactive
    /// </summary>
    public static void PushPanelDown()
    {
        infoSpaceAnimator.Play("PushPanelDown");

        CurrentSignState = SignState.Inactive;
    }
}
