#if UNITY_EDITOR
using System.Collections;
using UnityEditor;

public class EditorCoroutine
{
	public static EditorCoroutine Start (IEnumerator _routine)
	{
		EditorCoroutine coroutine = new EditorCoroutine (_routine);
		coroutine.Start ();
		return coroutine;
	}

	readonly IEnumerator routine;
	EditorCoroutine (IEnumerator _routine)
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

	void update ()
	{
		if (!routine.MoveNext ())
		{
			Stop ();
		}
	}
}
#endif