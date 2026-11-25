using UnityEngine;

public class CustomSceneBGM : MonoBehaviour
{
    public AudioClip sceneMusic;
    public bool stopMusicOnEnter = false;

    private void Start()
    {
        if (AudioManager.Instance == null) return;

        if (stopMusicOnEnter)
        {
            AudioManager.Instance.StopBGM();
        }
        else if (sceneMusic != null)
        {
            AudioManager.Instance.PlayBGM(sceneMusic);
        }
    }
}