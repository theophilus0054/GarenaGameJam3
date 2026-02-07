using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public SoundType hoverSound = SoundType.Hover;
    public SoundType clickSound = SoundType.Click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.PlaySound(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.PlaySound(clickSound);
    }
}
