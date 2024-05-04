using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Util.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click, bool isLeftClick = true)
	{
		UI_Base.BindEvent(go, action, type,isLeftClick);
	}

	public static bool IsValid(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

	// 일반 로그
	public static void Log
		(
			this System.Object obj,
			string message,
			[CallerMemberName] string methodName = "",
			[CallerLineNumber] int line = 0
		)
	{
		//            [CallerFilePath] string file = "",

#if UNITY_EDITOR
		string className = obj.GetType().Name;
		// 로그 포맷을 설정합니다. 클래스,메소드,줄 번호,메시지를 포함하도록 합니다.
		Debug.Log($"{className}::{methodName}() line : {line} | {message}");
#endif
	}

}
