using UnityEngine;
using UnityEngine.UI;

public class CustomButtonSound : MonoBehaviour
{
    public AudioClip customSFX;
    [Range(0f, 1f)] public float volume = 1f;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null && customSFX != null)
            {
                AudioManager.Instance.PlaySFX(customSFX, volume);
            }
        });
    }
}