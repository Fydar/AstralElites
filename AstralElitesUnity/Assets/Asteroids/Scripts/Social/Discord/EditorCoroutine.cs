﻿#if UNITY_EDITOR
using System.Collections;
using UnityEditor;

public class EditorCoroutine
{
	public static EditorCoroutine Start (IEnumerator _routine)
	{
		var coroutine = new EditorCoroutine (_routine);
		coroutine.Start ();
		return coroutine;
	}

	private readonly IEnumerator routine;

	private EditorCoroutine (IEnumerator _routine)
	{
		routine = _routine;
	}

	private void Start ()
	{
		EditorApplication.update += update;
	}
	public void Stop ()
	{
		EditorApplication.update -= update;
	}

	private void update ()
	{
		if (!routine.MoveNext ())
		{
			Stop ();
		}
	}
}
#endif