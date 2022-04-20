using UnityEngine;

public class SwordBlockFlash : MonoBehaviour
{
    //The flashing particle system on the gameObject
    static ParticleSystem flashParticle;

    private void Start()
    {
        //Grab the particle system component
        if (flashParticle == null)
        {
            flashParticle = GetComponent<ParticleSystem>();
        }
    }

    private void Update()
    {
        //When the effect stops playing, Destroy it
        if (!flashParticle.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
