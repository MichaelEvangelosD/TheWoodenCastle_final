using UnityEngine;

public class DropBehaviour : MonoBehaviour
{
    Animation animationClip; //The animation clip attached to the gameObject

    private void Start()
    {
        animationClip = GetComponent<Animation>();

        //Play the Float animation exactly when the object spawns
        animationClip.Play();
    }

    private void FixedUpdate()
    {
        if (animationClip != null)
        {
            //Destroy the GameObject after the animation has stopped playing
            if (!animationClip.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}
