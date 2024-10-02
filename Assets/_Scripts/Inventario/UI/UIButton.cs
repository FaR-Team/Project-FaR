using UnityEngine.EventSystems;
using UnityEngine.UI;

// <summary>
// Esto es una clase de botón que hereda de la que hay por defecto. Permite tener el click derecho y el izquierdo por separado.
// </summary>
public class UIButton : Button
{
    public UnityEngine.Events.UnityEvent onRightClick = new UnityEngine.Events.UnityEvent();
    public UnityEngine.Events.UnityEvent onMouseOver = new UnityEngine.Events.UnityEvent();
    public UnityEngine.Events.UnityEvent onMouseExit = new UnityEngine.Events.UnityEvent();
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Invocar el click izquierdo
            base.OnPointerClick(eventData);
        } else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Invocar el click derecho
            onRightClick.Invoke();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onMouseOver.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onMouseExit.Invoke();
    }
}
