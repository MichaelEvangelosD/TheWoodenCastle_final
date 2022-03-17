using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class ChestBehaviour : MonoBehaviour,
    IPlayerInteractable, ISpriteChanger, IAudioActivator
{
    //Open chest sprite
    [Header("Set in inspector")]
    [SerializeField] Sprite openSprite; //The chests open sprite
    [SerializeField] AudioSource chestAudioSource; //The audio soruce attached to the chest gameObject

    SpriteRenderer chestSR; // The SpirteRenderer of the chest 
    Transform dropPoint; //The point to instatiate the prefabs
    BoxCollider2D chestTrigger; //In what area can the player interact with the chest

    //Default values for the trigger
    Vector2 triggerSize = new Vector2(1.5f, 0.8f);
    bool isTrigger = true;
    bool isOpen, prompting;

    private void Start()
    {
        CacheReferences();
        SetTriggerDefaults();

        isOpen = false;
        prompting = false;
    }

    private void Update()
    {
        if (isOpen)
            return;

        //If we are in the trigger area AND press the interact button 
        if (prompting && Input.GetButtonDown("Fire2"))
        {
            isOpen = true;

            DropItem();
            ChangeToSprite(openSprite);
            PlayAudio();
        }
    }

    #region INTERFACE_IMPLEMENTATIONS
    /// <summary>
    /// Cache the script references 
    /// </summary>
    public void CacheReferences()
    {
        chestTrigger = GetComponent<BoxCollider2D>();
        chestSR = GetComponent<SpriteRenderer>();
        dropPoint = transform.GetChild(0);
    }

    /// <summary>
    /// Set the default state of the chest trigger
    /// </summary>
    public void SetTriggerDefaults()
    {
        chestTrigger.size = triggerSize;
        chestTrigger.isTrigger = isTrigger;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isOpen)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = false;
        }
    }

    /// <summary>
    /// Play the audio source clip
    /// </summary>
    public void PlayAudio()
    {
        chestAudioSource.Play();
    }

    public void ChangeToSprite(Sprite newSprite)
    {
        //OffSet the current chest position to the Y axis to correct the texture overlapping
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);

        chestSR.sprite = newSprite;
    }
    #endregion

    #region CHEST_SPECIFICS
    /// <summary>
    /// Instantiate the randomized item at the dropPoint position
    /// and change the chest sprite to the open one
    /// </summary>
    void DropItem()
    {
        if (dropPoint != null)
        {
            //Get the random drop GameObject from ChestDrops and Instantiate it
            int tempInt = RandomizeDrop();

            GameObject tempDrop = Instantiate(ChestDropsManager.S.chestDrops[tempInt]);
            tempDrop.SetActive(false);
            tempDrop.transform.SetParent(dropPoint.transform, false);
            tempDrop.transform.position = dropPoint.position;
            tempDrop.SetActive(true);
        }
    }

    /// <summary>
    /// Call to get a random number between 0 and ChestDropsManager.S.listCount
    /// </summary>
    int RandomizeDrop()
    {
        System.Random randomizer = new System.Random((int)DateTime.Now.Ticks);

        return randomizer.Next(0, ChestDropsManager.S.listCount);
    }
    #endregion
}
