using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * THIS CLASS USES A SINGLETON SO THERE IS ONLY INSTANCE OF IT IN EVERY SCENE
 *
 * [Variable Specifics]
 * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
 * Dynamically changed: These variables are changed throughout the game
 * 
 * [Class Flow]
 * 1. When the scene loads the StartFollow() methods gets called UNLESS we are in the MainMenu scene
 * 2. In LateUpdate() the script checks if the player is dead.
 *      2a. If he is DEAD then Zoom BOTH cameras on him while still moving the so he remains in the center
 *      2b. If he is NOT then just move the camera with him.
 * 
 * [Must Know]
 * 1. StartFollow() executes in every scene BUT the MainMenu
 * 2. The Player GameObject is found dynamically when StartFollow() gets called.
 */

[DefaultExecutionOrder(450)]
public class CameraFollower : MonoBehaviour
{
    public static CameraFollower S;

    [Header("Set in Inspector")]
    [SerializeField] Vector3 cameraOffset; //Used so the camera gets offseted from the player
    [SerializeField, Range(0.5f, 0.8f)] float camSizeOnDeath;

    Camera uiCamera; //The camera that's rendering the UI
    Transform objFollow; //The object to follow (player)

    float cameraSizeValue; //used for lerping both cameras' orthographic size
    bool canFollow = false;

    private void Awake()
    {
        S = this;

        GameEvents.S.onGameSceneChanged += StartFollow;
    }

    /// <summary>
    /// Call to find the Player gameObject and enable camera following mechanics
    /// </summary>
    /// <param name="state">Current scene of the game</param>
    public void StartFollow(GameScenes state)
    {
        if (state == GameScenes.InMainMenu) return;

        objFollow = GameObject.FindWithTag("Player").transform;
        canFollow = true;
    }

    private void Start()
    {
        //Cache the UI camera (child of the main camera)
        uiCamera = transform.GetChild(0).GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        //Early exit
        if (!canFollow) return;

        //If the player is dead
        if (PlayerBehaviour.S.PlayerHealth.IsDead())
        {
            MoveCamera();
            ZoomOnDeath();
        }
        else //If he is still alive
        {
            MoveCamera();
        }
    }

    /// <summary>
    /// Call to set the main cameras' transform to the objects' position + the offset value from the inspector
    /// </summary>
    void MoveCamera()
    {
        transform.position = objFollow.transform.position + cameraOffset;
    }

    /// <summary>
    /// Call to zoom in BOTH cameras' on the followed object
    /// </summary>
    void ZoomOnDeath()
    {
        //Start zooming in the following object
        cameraSizeValue = Camera.main.orthographicSize;
        cameraSizeValue = Interpolate(0.001f, cameraSizeValue, camSizeOnDeath);

        //Apply cameraSizeValue to BOTH cameras
        uiCamera.orthographicSize = cameraSizeValue;
        Camera.main.orthographicSize = cameraSizeValue;
    }

    /// <summary>
    /// Smoothly lerp to the new value in each frame
    /// </summary>
    /// <param name="posA">The starting value</param>
    /// <param name="posB">The final value</param>
    /// <returns>A float closer to endValue each time Interpolate() is called based on lerpTime</returns>
    float Interpolate(float lerpTime, float startValue, float endValue)
    {
        return (1 - lerpTime) * startValue + lerpTime * endValue;
    }

    private void OnDestroy()
    {
        S = null;

        //Unsub the methods so we don't get NullRef errors
        GameEvents.S.onGameSceneChanged -= StartFollow;
    }
}
