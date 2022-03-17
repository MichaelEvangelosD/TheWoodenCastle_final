using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class DoorBehaviour : MonoBehaviour,
    IPlayerInteractable, ISpriteChanger
{
    [Header("Set in inspector")]
    [SerializeField] Sprite openSprite; //The door open sprite
    [SerializeField] GameScenes sceneToLoad; //Which scene to load next

    SpriteRenderer doorSR; //The sprite renderer attached to the gameObject
    CircleCollider2D doorTrigger; //In what area to detect player interactions
    bool isOpen = false;
    bool prompting;

    //Default values for the trigger
    float triggerRadius = 0.5f;
    bool isTrigger = true;

    private void Start()
    {
        //Cache the component references
        CacheReferences();
        SetTriggerDefaults();

        isOpen = false;
        prompting = false;
    }

    private void Update()
    {
        if (!isOpen)
            return;

        if (prompting && Input.GetButtonDown("Fire2"))
        {
            isOpen = false;

            if (GameManager.S != null & GameManager.S.CurrentGameScene == GameScenes.InLevel2)
            {
                if (SceneLoader.S != null) SceneLoader.S.LoadLevel((GameScenes)GameManager.S.EligibleEnding);
            }
            else
            {
                if (SceneLoader.S != null) SceneLoader.S.LoadLevel(sceneToLoad);
            }
        }

    }

    /// <summary>
    /// Call to cache the needed component references
    /// </summary>
    public void CacheReferences()
    {
        doorTrigger = GetComponent<CircleCollider2D>();
        doorSR = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Set the default state of the door trigger
    /// </summary>
    public void SetTriggerDefaults()
    {
        doorTrigger.radius = triggerRadius;
        doorTrigger.isTrigger = isTrigger;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOpen)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!isOpen)
            return;

        if (collision.CompareTag("Player"))
        {
            prompting = false;
        }
    }

    /// <summary>
    /// Acts as a method accessor for ChangeToSprite(...)
    /// </summary>
    public void OpenDoor()
    {
        ChangeToSprite(openSprite);
    }

    /// <summary>
    /// Call to set the door sprite to the given parameter.
    /// <para>Sets isOpen to true</para>
    /// </summary>
    public void ChangeToSprite(Sprite newSprite)
    {
        doorSR.sprite = newSprite;
        isOpen = true;
    }
}