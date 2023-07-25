using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UiClickHandler : MonoBehaviour
{
public UnityEvent onLeftClick;
public UnityEvent onRightClick;
public UnityEvent onMiddleClick;
public bool isRightClick = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeftClick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightClick = true;
            onRightClick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            onMiddleClick.Invoke();
        }
    }
}
