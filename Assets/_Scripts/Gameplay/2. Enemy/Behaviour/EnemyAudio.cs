using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    AudioSource enemyAudioSource; //The enemy's audio source

    private void Start()
    {
        if ((enemyAudioSource = GetComponent<AudioSource>()) != true)
        { Utils.PrintMissingComponentMsg("AudioSource component", this); }
    }

    /// <summary>
    /// Call to play the clip loaded into enemyAudioSource
    /// </summary>
    public void PlayDeathSound()
    {
        enemyAudioSource.Play();
    }
}
