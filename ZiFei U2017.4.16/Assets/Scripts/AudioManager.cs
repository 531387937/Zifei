using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance = null;


    public AudioSource BGMAudioSource;

    private AudioSource heroSoundAudioSource;
    private AudioSource thingSoundAudioSource;
    private AudioSource tipsSoundAudioSource;
    private AudioSource soundAudioSource;

    private AudioSource heroAttackAudioSource;

    private Dictionary<string, AudioClip> audioClipsDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Instance = this;

        if (BGMAudioSource == null)
            BGMAudioSource = GetComponentInChildren<AudioSource>();

        thingSoundAudioSource = gameObject.AddComponent<AudioSource>();
        soundAudioSource = gameObject.AddComponent<AudioSource>();
        tipsSoundAudioSource = gameObject.AddComponent<AudioSource>();
        heroSoundAudioSource = gameObject.AddComponent<AudioSource>();
        heroAttackAudioSource = gameObject.AddComponent<AudioSource>();


        soundAudioSource.clip = LoadAudioClip(Global.GetInstance().audioName_BtnClick3);
        tipsSoundAudioSource.clip = LoadAudioClip(Global.GetInstance().audioName_BtnClick3);
        thingSoundAudioSource.clip = LoadAudioClip(Global.GetInstance().audioName_BtnClick3);
        heroSoundAudioSource.clip = LoadAudioClip(Global.GetInstance().audioName_BtnClick3);
        heroAttackAudioSource.clip = LoadAudioClip(Global.GetInstance().audioName_BtnClick3);
        StopSound();
    }


    public void SetBGMVolume(float volume)
    {

        BGMAudioSource.volume = volume;

    }

    public void SetSoundVolume(float volume)
    {
        soundAudioSource.volume = volume;
        thingSoundAudioSource.volume = volume;
        heroSoundAudioSource.volume = volume;
        heroAttackAudioSource.volume = volume;
        tipsSoundAudioSource.volume = volume;
    }


    public void SoundPlay(string clipName)
    {
        if (soundAudioSource.clip != null && soundAudioSource.clip.name != clipName)
            soundAudioSource.clip = LoadAudioClip(clipName);

        if (soundAudioSource.clip == null) return;

        soundAudioSource.loop = false;
        soundAudioSource.Play();
    }

    public void TipsSoundPlay(string clipName)
    {
        if (tipsSoundAudioSource.clip != null && tipsSoundAudioSource.clip.name != clipName)
            tipsSoundAudioSource.clip = LoadAudioClip(clipName);

        if (tipsSoundAudioSource.clip == null) return;

        tipsSoundAudioSource.loop = false;
        tipsSoundAudioSource.Play();
    }

    public void HeroAttackSoundPlay(string clipName)
    {
        if (heroAttackAudioSource.clip != null && heroAttackAudioSource.clip.name != clipName)
            heroAttackAudioSource.clip = LoadAudioClip(clipName);

        if (heroAttackAudioSource.clip == null) return;

        heroAttackAudioSource.loop = false;
        heroAttackAudioSource.Play();
    }

    public void ThingSoundPlay(string clipName)
    {
        if (thingSoundAudioSource.clip != null && thingSoundAudioSource.clip.name != clipName)
            thingSoundAudioSource.clip = LoadAudioClip(clipName);

        if (thingSoundAudioSource.clip == null) return;

        thingSoundAudioSource.loop = false;
        thingSoundAudioSource.Play();
    }

    public void HeroSoundPlay(string clipName, bool isLoop = false)
    {
        if (heroSoundAudioSource.clip != null && heroSoundAudioSource.clip.name != clipName)
            heroSoundAudioSource.clip = LoadAudioClip(clipName);

        if (heroSoundAudioSource.clip == null) return;
        
        heroSoundAudioSource.loop = false;
        heroSoundAudioSource.Play();
    }

    public void HeroSoundStop() {
        if(heroSoundAudioSource.clip != null)
            heroSoundAudioSource.Stop();
    }

    public void StopSound() {
        heroSoundAudioSource.Stop();
        thingSoundAudioSource.Stop();
        soundAudioSource.Stop();
        heroAttackAudioSource.Stop();
        tipsSoundAudioSource.Stop();
    }

    AudioClip LoadAudioClip(string clipName) {

        if (audioClipsDictionary.ContainsKey(clipName))
        {
            return audioClipsDictionary[clipName];
        }
        else {

            AudioClip clip = (AudioClip)Resources.Load(clipName);
            audioClipsDictionary.Add(clipName, clip);
            return clip;
        }

    }
   
}
 