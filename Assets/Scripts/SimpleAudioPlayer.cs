using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource =default;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private bool is3D = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.loop = true;
        audioSource.spatialBlend = is3D ? 1f : 0f;

        audioSource.volume = 1.0f;
        audioSource.clip = audioClip;

        //StartCoroutine(audioSource.PlayWithCompCallback(audioClip));
        //StartCoroutine(audioSource.PlayWithFadeIn(audioClip));
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
