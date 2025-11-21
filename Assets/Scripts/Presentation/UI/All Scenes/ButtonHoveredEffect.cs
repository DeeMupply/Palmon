using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoveredEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverImage;
    private void Start()
    {
        hoverImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverImage.SetActive(false);
    }
}