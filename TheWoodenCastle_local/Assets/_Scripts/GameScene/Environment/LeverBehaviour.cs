using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LeverBehaviour : MonoBehaviour,
    IPlayerInteractable, ISpriteChanger, IAudioActivator
{
    [Header("Set in inspector")]
    [SerializeField] Sprite usedSprite; //The used lever sprite
    [SerializeField] AudioSource leverAudioSource; //The audio source attached to the lever gameObject

    SpriteRenderer leverSR; //The Sprite renderer attached to the lever gameObject
    BoxCollider2D leverTrigger; //In which area to detect player interactions in

    GameObject doorToActivate; //The door to activate in the scene

    //Default values for the trigger
    Vector2 triggerSize = new Vector2(1.5f, 0.8f);
    bool isTrigger = true;
    bool isUsed, prompting;

    private void Start()
    {
        CacheReferences();
        SetTriggerDefaults();

        isUsed = false;
        prompting = false;
    }

    private void Update()
    {
        if (isUsed)
            return;

        //If we are prompting the user to interact and we press the interact button
        if (prompting)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isUsed = true;
                ChangeToSprite(usedSprite);

                PlayAudio();

                doorToActivate.GetComponent<DoorBehaviour>().OpenDoor();
            }
        }
    }

    /// <summary>
    /// Call to cache the component references
    /// </summary>
    public void CacheReferences()
    {
        leverTrigger = GetComponent<BoxCollider2D>();
        leverSR = GetComponent<SpriteRenderer>();
        doorToActivate = GameObject.FindGameObjectWithTag("DoorLocked");
    }

    /// <summary>
    /// Set the default state of the Lever trigger
    /// </summary>
    public void SetTriggerDefaults()
    {
        leverTrigger.size = triggerSize;
        leverTrigger.isTrigger = isTrigger;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isUsed)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = false;
        }
    }

    public void ChangeToSprite(Sprite newSprite)
    {
        //OffSet the current lever position to the Y axis to correct the texture overlapping
        transform.position = new Vector3(transform.position.x - 0.08f, transform.position.y - 0.08f, transform.position.z);
        leverSR.sprite = newSprite;
    }

    /// <summary>
    /// Call to play the audio sources main audio clip
    /// </summary>
    public void PlayAudio()
    {
        leverAudioSource.Play();
    }
}
