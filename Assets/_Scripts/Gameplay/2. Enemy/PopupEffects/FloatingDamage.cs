using UnityEngine;

public class FloatingDamage : MonoBehaviour
{
    static Animator floatAnimator; //The gameObjects' animator

    private void Start()
    {
        if (floatAnimator == null)
        {
            floatAnimator = GetComponent<Animator>();
        }

        //Play the Float animation exactly when the object spawns
        if (floatAnimator != null)
        {
            floatAnimator.Play("DamageFloat");
            floatAnimator.applyRootMotion = true;
        }

        //Destroy the gameObject after 1 second
        Destroy(gameObject, 1f);
    }
}