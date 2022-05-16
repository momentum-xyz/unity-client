using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetAudioSourceMixer : MonoBehaviour
{
    public string mixerPath = "Audio/AudioMixer";
    [Tooltip("Available Groups: Music,FX")]
    public string mixerGroup = "Music";



    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null) return;

        AudioMixer mixer = Resources.Load(mixerPath) as AudioMixer;

        if (mixer == null)
        {
            Debug.Log("Could not find AudioMixer with path: " + mixerPath);
            return;
        }

        AudioMixerGroup[] audioGroups = mixer.FindMatchingGroups(mixerGroup);

        if (audioGroups.Length == 0) return;

        audioSource.outputAudioMixerGroup = audioGroups[0];

    }
}
