using System;
using System.Collections;
using UnityEngine;
using TMPro;

/* CLASS DOCUMENTATION *\
 * 
 * [Variable Specifics]
 * Inspector values:  typingSpeed, fastForwardSpeed, sentenceToType MUST be set from the inspector.
 * Dynamically changed:  The Reference Variables are dynamically cached and changed when the engine calls the Start method.
 * 
 * [Class Flow]
 * 1. The Main entry point of the class is the OnTriggerEnter2D method when the player enters its trigger space.
 * 2. If the Player chooses to interact with the trigger, the Coroutine gets called 
 *      and enables the FastForward ability of the player in Update().
 * 3.At the end of the coroutine the exit prompt gets enabled and the player 
 *      can press the Fire3 button (E on K/B, Y on XBOX One, Delta on PS4) to exit the interaction sequence.
 * 
 * [Must Know]
 * 1. When the interaction sequence starts the PlayerState.PlayerActive is set to false to deactivate the Player behaviour.
 *  1a. When the interaction sequence ends the PlayerState.PlayerActive is set to true to activate the Player behaviour.
 */

[DefaultExecutionOrder(900)]
public class TutorialSign : MonoBehaviour,
    IPlayerInteractable
{
    [Header("Set in inspector")]
    [SerializeField, Range(0, 0.1f)] float typingSpeed; //How fast should the text be displayed
    [SerializeField, Range(0, 0.1f)] float fastForwardSpeed; //The speed the text gets displayed if we press the FF key
    [Tooltip("Everything that's inside this Text Box will be displayed " +
        "letter by letter in the canvas infoBox")]
    [SerializeField, TextArea] string sentenceToType;

    //Reference Variables
    GameObject signCanvas;
    AudioSource signAudioSource;
    BoxCollider2D triggerBox;
    TextMeshProUGUI signTextBox;
    bool prompting = false;
    bool activated = false;
    bool canExit = false;

    bool canFastForward = false;
    float cachedTypeSpeed;

    private void Start()
    {
        CacheReferences();
        SetTriggerDefaults();

        //Cache the typing speed
        cachedTypeSpeed = typingSpeed;

        //Deactivate the sign canvas
        signCanvas.SetActive(false);
    }

    /// <summary>
    /// Call to cache the needed component references
    /// </summary>
    public void CacheReferences()
    {
        signCanvas = transform.GetChild(1).gameObject;
        signAudioSource = GetComponent<AudioSource>();
        triggerBox = GetComponent<BoxCollider2D>();
        signTextBox = SignDialogueSpace.InfoTextSpace;
    }

    /// <summary>
    /// Call to set the default trigger values
    /// </summary>
    public void SetTriggerDefaults()
    {
        triggerBox.isTrigger = true;
        triggerBox.offset = new Vector2(-0.05478668f, 0.19f);
        triggerBox.size = new Vector2(0.8904266f, 1f);
    }

    private void Update()
    {
        //Enabled from OnTriggerEnter to enable the interact UI  panel
        if (prompting)
        {
            if (Input.GetButtonDown("Fire2") && !activated)
            {
                activated = true;

                //Deactivate the player when we enter interaction state
                PlayerBehaviour.S.PlayerActive = false;

                SignDialogueSpace.PullPanelUp();

                StartCoroutine(TypeSentence(signTextBox, sentenceToType, RoutineCallback));
            }
        }

        //Can be triggered while the coroutine gets executed
        if (canFastForward)
        {
            if (Input.GetButtonDown("Fire3"))
            {
                TriggerFastForward();
            }
        }

        /*Enabled ONLY after the coroutine finishes printing
        all the letters in the textBox*/
        if (canExit)
        {
            if (Input.GetButtonDown("Fire3"))
            {
                SignDialogueSpace.PushPanelDown();

                PlayerBehaviour.S.PlayerActive = true;

                SignDialogueSpace.InfoTextSpace.text = "";

                activated = false;
                canExit = false;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Activate only if the trgger detects the player prefab
        if (!signCanvas.activeSelf && collision.CompareTag("Player"))
        {
            signCanvas.SetActive(true);

            prompting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //Activate only if the trgger detects the player prefab
        if (signCanvas.activeSelf && collision.CompareTag("Player"))
        {
            signCanvas.SetActive(false);

            prompting = false;
            activated = false;
        }
    }

    /// <summary>
    /// Display the given sentence letter by letter in the given text box with a small interval.
    /// </summary>
    IEnumerator TypeSentence(TextMeshProUGUI textBox, string sentenceToType, Action callbackMethod)
    {
        //reset the typing speed
        typingSpeed = cachedTypeSpeed;

        //Wait until the ui panel is up
        yield return new WaitForSeconds(1f);

        //Enable fast forwarding the text
        canFastForward = true;

        //Clear the text
        textBox.text = "";

        //Start displaying each letter one by one and play the Tick Sound 
        foreach (char letter in sentenceToType.ToCharArray())
        {
            textBox.text += letter;
            PlayTickSound();

            yield return new WaitForSeconds(typingSpeed);
        }

        //... and finally call the couroutine callback method
        callbackMethod();
    }

    /// <summary>
    /// Call to enable the players ability to exit the UI interaction
    /// and stop the fast forwarding ability.
    /// <para>Coroutine use ONLY, passed as an action callback</para>
    /// </summary>
    void RoutineCallback()
    {
        canExit = true;
        canFastForward = false;
    }

    /// <summary>
    /// Call to randomize the signs audio source pitch and play it's attached clip as an OneShot
    /// </summary>
    void PlayTickSound()
    {
        signAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        signAudioSource.PlayOneShot(signAudioSource.clip);
    }

    /// <summary>
    /// Call to enable the players' ability to fast forward and change the typing speed
    /// to the inspector value of fastForwardSpeed.
    /// </summary>
    void TriggerFastForward()
    {
        canFastForward = false;
        typingSpeed = fastForwardSpeed;
    }
}
