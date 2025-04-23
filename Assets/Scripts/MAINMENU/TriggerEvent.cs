using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ButtonAnimation buttonAnimation;

    private void Awake()
    {
        buttonAnimation = GetComponent<ButtonAnimation>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonAnimation.StartUnderlineAnimation();
    }

    // This method is called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonAnimation.ResetUnderlineAnimation();
    }
}