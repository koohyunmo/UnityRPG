using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerUpHandler
{
	private float lastClickTime = 0f;
	private int clickCount = 0;
	private const float doubleClickThreshold = 0.2f; // 더블 클릭 감지 시간 간격

	public Action<PointerEventData> OnLeftClickHandler = null;
	public Action<PointerEventData> OnRightClickHandler = null;
	public Action<PointerEventData> OnDragHandler = null;
	public event Action<PointerEventData> OnDoubleClickHandler = null;  // 더블 클릭 핸들러 추가
	public Action<PointerEventData> OnPointerUpHandler = null;  // 마우스 포인트 업 핸들러 추가

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			clickCount++;
			if (clickCount == 1)
			{
				lastClickTime = Time.time;
			}
			else if (clickCount == 2 && Time.time - lastClickTime < doubleClickThreshold)
			{
				// 더블 클릭 이벤트 발생
				OnDoubleClickHandler?.Invoke(eventData);
				clickCount = 0;  // 클릭 카운트 초기화
			}

			// 시간 간격이 너무 길면 클릭 카운트를 리셋
			if (Time.time - lastClickTime > doubleClickThreshold)
			{
				clickCount = 1;
				lastClickTime = Time.time;
			}

			// 첫 클릭 이벤트 발생
			if (OnLeftClickHandler != null && clickCount == 1)
			{
				OnLeftClickHandler.Invoke(eventData);
			}
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

	public void OnPointerUp(PointerEventData eventData)
	{
		// 마우스 버튼이 떼어질 때 이벤트 처리
		if (eventData.dragging)
		{
			if (OnPointerUpHandler != null)
				OnPointerUpHandler.Invoke(eventData);
			clickCount = 0;
		}
	}

}
