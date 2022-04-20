using UnityEngine;

public class CorpseHeartRip : MonoBehaviour,
    IPlayerInteractable, IAudioActivator
{
    [Header("Set in inspector")]
    [SerializeField] GameObject worstHeartPrefab; //The worst heart prefab variant
    [SerializeField] Vector2 triggerOffset, triggerSize;

    AudioSource squishAudioSource; //The attached audio source
    BoxCollider2D triggerArea; //The area in which the player can interact with the chest

    bool isActivated = false;
    bool prompting = false;

    private void Awake()
    {
        CacheReferences();
        SetTriggerDefaults();

        isActivated = false;
        prompting = false;
    }

    /// <summary>
    /// Call to cache the needed component references
    /// </summary>
    public void CacheReferences()
    {
        squishAudioSource = GetComponent<AudioSource>();
        triggerArea = GetComponent<BoxCollider2D>();

        if (squishAudioSource == null) print($"Squish sound is empty at {this}");
        if (triggerArea == null) print($"Box Colldier trigger is empty at {this}");
    }

    /// <summary>
    /// Call to set the trigger default position and state
    /// </summary>
    public void SetTriggerDefaults()
    {
        triggerArea.isTrigger = true;
        triggerArea.offset = triggerOffset;
        triggerArea.size = triggerSize;
    }

    private void Update()
    {
        //If we are prompting and press the interact button...
        if (prompting && !isActivated)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isActivated = true;
                DropItem();
                PlayAudio();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            prompting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isActivated)
        {
            prompting = false;
        }
    }

    /// <summary>
    /// Call to instantiate the worst heart prefab variant
    /// </summary>
    void DropItem()
    {
        GameObject tempDrop = Instantiate(worstHeartPrefab);
        tempDrop.transform.SetParent(transform, false);
        tempDrop.transform.position = transform.position;
    }

    /// <summary>
    /// Call to player the audio sources' clip as one shot
    /// </summary>
    public void PlayAudio()
    {
        squishAudioSource.PlayOneShot(squishAudioSource.clip);
    }
}
