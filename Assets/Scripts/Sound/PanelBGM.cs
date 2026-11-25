using UnityEngine;

public class PanelBGM : MonoBehaviour
{
    public AudioClip panelMusic;
    public AudioClip returnMusic;

    private void OnEnable()
    {
        if (AudioManager.Instance != null && panelMusic != null)
        {
            AudioManager.Instance.PlayBGM(panelMusic);
        }
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null && returnMusic != null)
        {
            AudioManager.Instance.PlayBGM(returnMusic);
        }
    }
}