using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundSetup
{
    public string id;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    private AudioSource[] audioSources;
    private Dictionary<string, AudioClip> idToClip;
    public List<SoundSetup> setup;

    private void Awake()
    {
        idToClip = new Dictionary<string, AudioClip>();
        audioSources = GetComponentsInChildren<AudioSource>();
        foreach (var item in setup)
        {
            idToClip.Add(item.id, item.clip);
        }
    }

    public void PlaySound(string id)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                if (idToClip.ContainsKey(id))
                {
                    audioSources[i].clip = idToClip[id];
                    audioSources[i].Play();
                    return;
                }
            }
        }
    }
}
