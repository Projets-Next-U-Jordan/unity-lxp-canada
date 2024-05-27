using UnityEngine;
using UnityEngine.EventSystems;

public class FartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioClip hoverSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound == null)
        {
            Debug.LogWarning("No hover sound assigned to the FartButton");
            return;
        }
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // audioSource.Stop();
    }
}