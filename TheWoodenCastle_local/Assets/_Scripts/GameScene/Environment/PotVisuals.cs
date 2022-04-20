using UnityEngine;

public class PotVisuals : MonoBehaviour
{
    Animation potAnimation; //The attached animation clip on the GameObject
    AudioSource potAudioSource; //The attached audio source on the GameObject
    ParticleSystem potParticleSystem; //The attached ParticleSystem on the GameObject

    private void Awake()
    {
        //Cache the needed component references
        potAnimation = GetComponent<Animation>();
        potAudioSource = GetComponent<AudioSource>();
        potParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Call to play the audiom, animation, particle effect and the Destroy the gameObject
    /// </summary>
    public void DestroyPot()
    {
        potParticleSystem.Play();
        potAnimation.Play();
        potAudioSource.Play();

        Destroy(gameObject, .3f);
    }
}
