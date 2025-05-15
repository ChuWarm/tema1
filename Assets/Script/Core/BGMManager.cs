using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum BGMType { MainMenu, InGame, EliteRoom, BossRoom }

public class BGMManager : Singleton<BGMManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 0.5f;
    
    [Header("배경 음악 리스트")]
    public AudioClip mainMenuBGM;
    public AudioClip inGameBGM;
    public AudioClip eliteRoomBGM;
    public AudioClip bossRoomBGM;

    private Dictionary<BGMType, AudioClip> _bgmClips;
    private Coroutine _fadeCoroutine;
    private BGMType? _currentBGM;

    private void Start()
    {
        _bgmClips = new Dictionary<BGMType, AudioClip>
        {
            { BGMType.MainMenu, mainMenuBGM },
            { BGMType.InGame, inGameBGM },
            { BGMType.EliteRoom, eliteRoomBGM },
            { BGMType.BossRoom, bossRoomBGM },
        };
    }

    public void PlayBGM(BGMType type)
    {
        if (_currentBGM ==  type) return;

        if (_bgmClips.TryGetValue(type, out var clip))
        {
            _currentBGM = type;
            
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeToNewClip(clip));
        }
        else
        {
            Debug.LogWarning($"[BGMManager] 클립이 등록되지 않은 타입: {type}");
        }
    }

    private IEnumerator FadeToNewClip(AudioClip newclip)
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = 1f - (t / fadeDuration);
            yield return null;
        }
        
        audioSource.clip = newclip;
        audioSource.loop = true;
        audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = t / fadeDuration;
            yield return null;
        }
        
        audioSource.volume = 1f;
    }
}
