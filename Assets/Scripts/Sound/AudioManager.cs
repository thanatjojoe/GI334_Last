using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSFX;
    public AudioClip buttonHighlightSFX;
    public AudioClip jumpSFX;
    public AudioClip coinSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBGM(backgroundMusic);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (Button btn in allButtons)
        {
            if (btn.GetComponent<CustomButtonSound>() == null)
            {
                btn.onClick.RemoveListener(PlayButtonClick);
                btn.onClick.AddListener(PlayButtonClick);
            }

            AddHighlightEvent(btn.gameObject);
        }

        Toggle[] allToggles = FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Toggle tgl in allToggles)
        {
            tgl.onValueChanged.RemoveListener(OnToggleValueChanged);
            tgl.onValueChanged.AddListener(OnToggleValueChanged);

            AddHighlightEvent(tgl.gameObject);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PlayButtonClick();
        }
    }

    private void AddHighlightEvent(GameObject uiObj)
    {
        EventTrigger trigger = uiObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = uiObj.AddComponent<EventTrigger>();
        }

        trigger.triggers.RemoveAll(e => e.eventID == EventTriggerType.PointerEnter);

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayButtonHighlight(); });
        trigger.triggers.Add(entry);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayButtonHighlight()
    {
        PlaySFX(buttonHighlightSFX);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
