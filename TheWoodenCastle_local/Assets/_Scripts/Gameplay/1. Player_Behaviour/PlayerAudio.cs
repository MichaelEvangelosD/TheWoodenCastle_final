using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the available Audio clips for the player
/// </summary>
public enum PlayerAudioClips
{
    Attack = 0,
    Hurt = 1,
    Death = 2,
    SwordBlock = 3,
    Suicide = 4,
}

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips; //Populated list with all the player audio clips

    AudioSource playerAudioSource; //The player sound source

    private void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play the given clip as one shot 
    /// <param name="audioClip">Which audio clip to play</param>
    /// <param name="changePitch">If given value is true, randomize audio source pitch</paramref>
    public void PlayAudio(PlayerAudioClips audioClip, bool changePitch = false)
    {
        playerAudioSource.pitch = 1f;

        if (changePitch && audioClip != PlayerAudioClips.Death)
        {
            playerAudioSource.pitch = Random.Range(0.9f, 1.1f);
        }

        playerAudioSource.PlayOneShot(audioClips[(int)audioClip]);
    }
}
