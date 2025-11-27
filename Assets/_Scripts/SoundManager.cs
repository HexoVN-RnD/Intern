using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;
    [SerializeField] private AudioSource soundEffect;
    [SerializeField] private AudioSource helpSound;
    [SerializeField] private AudioSource bgMusic;
    
    [SerializeField] private AudioClip throwSnowball;
    [SerializeField] private AudioClip breakIce;
    [SerializeField] private AudioClip happySound;
    [SerializeField] private AudioClip snowBallHit;

    private bool hasPlayEffectSound = false;
    private void Awake()
    {
        SoundManager.instance = this;
    }
    public bool HasPlayEffectSound()
    {
        return hasPlayEffectSound;
    }
    public void SetHasPlayEffectSound(bool value)
    {
        hasPlayEffectSound = value;
    }
    private void Start()
    {
        soundEffect.Stop();
        hasPlayEffectSound = true;
    }
    public void PlayThrowSound()
    {
        soundEffect.PlayOneShot(throwSnowball);
        soundEffect.volume = 0.2f;
    }
    public void PlayBreakIce() 
    {
        soundEffect.PlayOneShot(breakIce);
    }
    public void PlayHappySound() 
    {
        soundEffect.PlayOneShot(happySound);
        soundEffect.volume = 0.8f;
    }
    public void PlaySnowBallHitSound() 
    {
        soundEffect.PlayOneShot(snowBallHit);
        soundEffect.volume = 0.5f;
    }
    public void StopHelpSound() 
    {
        helpSound.Stop();
    }
    public void StopBGMusic() 
    {
        bgMusic.Stop();
    }
    
}