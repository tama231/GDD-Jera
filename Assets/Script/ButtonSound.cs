using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [Header("Audio Sources")]
    public AudioSource audioSource;
    
    [Header("Sound Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;



    // Suara muncul saat mouse ngehover tombol
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // Suara muncul saat tombol dipencet
    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
