using UnityEngine;
using UnityEngine.EventSystems;


public class TriggerColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ButtonColorChange buttonColorChange;

    private void Awake()
    {
        buttonColorChange = GetComponent<ButtonColorChange>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonColorChange.SetSelected();
    }

    // This method is called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonColorChange.SetDefault();
    }
}