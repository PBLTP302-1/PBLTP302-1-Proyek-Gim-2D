using System.Collections;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float crossfadeDuration = 1f;
    private AudioClip worldMusic;

    [Header("Battle Music")]
    [SerializeField] private AudioClip battleMusic; 
    [SerializeField] private AudioClip bossMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxEncounter;
    [SerializeField] private AudioClip sfxEvolve;
    [SerializeField] private AudioClip sfxFaint;
    [SerializeField] private AudioClip sfxFootstep;
    [SerializeField] private AudioClip sfxHeal;
    [SerializeField] private AudioClip sfxHit;
    [SerializeField] private AudioClip sfxHitCritical;
    [SerializeField] private AudioClip sfxLevelUp;
    [SerializeField] private AudioClip sfxMiss;
    [SerializeField] private AudioClip sfxSave;
    [SerializeField] private AudioClip sfxVictory;

    [Header("SFX Clips - UI")]
    [SerializeField] private AudioClip sfxDialogBlip;
    [SerializeField] private AudioClip sfxUIMove;
    [SerializeField] private AudioClip sfxConfirm;
    [SerializeField] private AudioClip sfxCancel;

    public bool Muted { get; private set; }

    // MUSIC
    public void PlayWorldMusic(AudioClip clip)
    {
        worldMusic = clip;
        PlayMusic(clip);
    }

    public void PlayBattleMusic(bool isBoss)
    {
        PlayMusic(isBoss ? bossMusic : battleMusic);
    }

    public void ResumeWorldMusic()
    {
        PlayMusic(worldMusic);
    }

    private Coroutine crossfadeCoroutine;
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(clip));
    }

    IEnumerator Crossfade(AudioClip newClip)
    {
        float t = 0f;
        float startVolume = musicSource.volume;
        musicSource.clip = newClip;
        musicSource.Play();
        while (t < crossfadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0.5f, t / crossfadeDuration);
            yield return null;
        }
        musicSource.volume = 0.5f;
    }

    // SFX
    public void PlaySfx(AudioClip clip, float pitchJitter = 0.05f)
    {
        if (Muted || clip == null || sfxSource == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayEncounter() => PlaySfx(sfxEncounter);
    public void PlayEvolve() => PlaySfx(sfxEvolve);
    public void PlayFaint() => PlaySfx(sfxFaint);
    public void PlayFootstep() => PlaySfx(sfxFootstep, 0.1f);
    public void PlayHeal() => PlaySfx(sfxHeal);
    public void PlayHit(bool isCrit) => PlaySfx(isCrit ? sfxHitCritical : sfxHit);
    public void PlayLevelUp() => PlaySfx(sfxLevelUp);
    public void PlayMiss() => PlaySfx(sfxMiss);
    public void PlaySave() => PlaySfx(sfxSave);
    public void PlayVictory() => PlaySfx(sfxVictory, 0f); 
    public void PlayDialogBlip() => PlaySfx(sfxDialogBlip, 0.15f);
    public void PlayUIMove() => PlaySfx(sfxUIMove, 0f);
    public void PlayConfirm() => PlaySfx(sfxConfirm, 0f);
    public void PlayCancel() => PlaySfx(sfxCancel, 0f);

    public bool ToggleMute()
    {
        Muted = !Muted;
        AudioListener.volume = Muted ? 0f : 1f;
        return Muted;
    }
}