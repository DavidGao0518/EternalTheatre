using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour {
    private readonly (float min, float max) _lowPassMinMax = (500f, 22000f);
    private const string LowPassCutoff = "LowPassCutoff";
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixer audioMixer;

    public void PlayMusic(AudioClip musicClip, bool loop, float pitch, float volume, bool overrideAlways) {
        if (!overrideAlways && musicSource.clip != null && musicSource.clip == musicClip) return;

        StopCoroutine(nameof(PlayMusicRoutine));
        StartCoroutine(PlayMusicRoutine(musicClip, loop, pitch, volume));
    }

    private IEnumerator PlayMusicRoutine(AudioClip musicClip, bool loop, float pitch, float volume) {
        if (musicSource.isPlaying) {
            StartCoroutine(FadeVolume(0.3f, 0));
            yield return new WaitForSecondsRealtime(0.35f);
        }

        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        musicSource.Play();
        musicSource.pitch = pitch;
    }

    public void StopMusic() {
        StartCoroutine(StopMusicRoutine(0.5f));
    }

    public void StopMusic(float FadeTime)
	{
        StartCoroutine(StopMusicRoutine(FadeTime));
    }


    private IEnumerator StopMusicRoutine(float fadeDuration) {
        StartCoroutine(FadeVolume(fadeDuration, 0));

        yield return new WaitForSecondsRealtime(fadeDuration);

        musicSource.Stop();
        musicSource.clip = null;
    }

    private IEnumerator FadeVolume(float fadeDuration, float targetVolume) {
        float start = musicSource.volume;
        float currentTime = 0;

        while (currentTime < fadeDuration) {
            musicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / fadeDuration);

            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // Methods below are not used but can be implemented.
    public void UpdateVolume(float fadeDuration, float targetVolume) {
        StartCoroutine(FadeVolume(fadeDuration, targetVolume));
    }

    public void UpdatePitch(float fadeDuration, float targetPitch) {
        StartCoroutine(FadePitch(fadeDuration, targetPitch));
    }

    private IEnumerator FadePitch(float fadeDuration, float targetPitch) {
        float start = musicSource.pitch;
        float currentTime = 0;

        while (currentTime < fadeDuration) {
            musicSource.pitch = Mathf.Lerp(start, targetPitch, currentTime / fadeDuration);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    #region LowPassCutOffFilter

    public void SetLowPassCutOff(float duration, float f) {
        StartCoroutine(FadeLowPassFilter(duration, f));
    }

    private void SetLowPassFilter(float lowPassTransitionDirection) {
        float lowPassValue = Mathf.Lerp(_lowPassMinMax.min, _lowPassMinMax.max, lowPassTransitionDirection);
        audioMixer.SetFloat(LowPassCutoff, lowPassValue);
    }

    private IEnumerator FadeLowPassFilter(float fadeDuration, float targetPass) {
        float currentTime = 0;

        while (currentTime < fadeDuration) {
            SetLowPassFilter(Mathf.Lerp(0, targetPass, currentTime / fadeDuration));

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}