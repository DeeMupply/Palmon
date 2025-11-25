using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonHoveredEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<GameObject> hoverImage;
    private void Start()
    {
        foreach (var image in hoverImage)
        {
            image.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var image in hoverImage)
        {
            image.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var image in hoverImage)
        {
            image.SetActive(false);
        }
    }
}