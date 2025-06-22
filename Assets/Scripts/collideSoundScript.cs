using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideSoundScript : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] AudioSource audioSource;


    private void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (audioManager != null && (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)) {
            audioManager.playAudio(audioSource);
        }
    }
}
