using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class FootstepsPlayer : MonoBehaviour
{

    [SerializeField] private AudioClip _footstepsSound;
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    public void PlayFootstep()
    {
        _audioSource.pitch = Random.Range(0.4f, 1.2f);
        _audioSource.PlayOneShot(_footstepsSound);
    }
}
