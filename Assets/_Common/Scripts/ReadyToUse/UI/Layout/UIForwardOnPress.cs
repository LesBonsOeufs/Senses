using UnityEngine;
using UnityEngine.EventSystems;

public class UIForwardOnPress : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}