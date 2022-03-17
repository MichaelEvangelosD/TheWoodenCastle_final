using UnityEngine;

public class FallThroughPlatform : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] LayerMask defaultLayers; //TIn which layers to detect collisions on
    [SerializeField] float holdThreshold = 0.5f; //How long should the player hold the down button to fall

    PlatformEffector2D platformEffector; //The gameObjects' platform effector

    float timer = 0.2f;
    bool layersSet = true;

    private void Start()
    {
        if ((platformEffector = GetComponent<PlatformEffector2D>()) != true)
        { Utils.PrintMissingComponentMsg("PlatformEffector2D component", this); }
    }

    private void Update()
    {
        //If the player presses the down key longer than the holdThreshold
        if (Input.GetAxis("Vertical") <= -holdThreshold && layersSet)
        {
            //Collide with nothing
            platformEffector.colliderMask = 0;

            layersSet = false;
        }

        //Reset the platform layerMask after the specified amount of time
        if (!layersSet)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                platformEffector.colliderMask = defaultLayers;
                timer = 0.2f;

                layersSet = true;
            }
        }
    }
}
