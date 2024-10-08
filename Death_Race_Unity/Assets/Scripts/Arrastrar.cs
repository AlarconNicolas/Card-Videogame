using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startPosition;
    private Transform originalParent;
    public Transform placeholderParent = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        placeholderParent = originalParent; // Asume que el parent original es válido

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        transform.SetParent(transform.parent.parent); // Mueve al objeto un nivel arriba en la jerarquía para evitar conflictos visuales
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == originalParent.parent)
        {
            transform.position = startPosition; // Si el objeto no fue soltado sobre un Droppable válido, vuelve al inicio
        }
        transform.SetParent(placeholderParent ? placeholderParent : originalParent);
    }
}

public class Droppable : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null)
        {
            draggable.placeholderParent = this.transform; // Asigna el nuevo parent al objeto arrastrado
        }
    }
}
