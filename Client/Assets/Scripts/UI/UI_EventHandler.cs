using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
	public Action<PointerEventData> OnLeftClickHandler = null;
	public Action<PointerEventData> OnRightClickHandler = null;
	public Action<PointerEventData> OnDragHandler = null;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (OnLeftClickHandler != null)
				OnLeftClickHandler.Invoke(eventData);
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (OnRightClickHandler != null)
				OnRightClickHandler.Invoke(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
	}
}
