using System.Collections;
using UnityEditor;

public class EditorCoroutine
{
	public static EditorCoroutine Start(IEnumerator _routine)
	{
		var coroutine = new EditorCoroutine(_routine);
		coroutine.StartInternal();
		return coroutine;
	}

	private readonly IEnumerator routine;

	private EditorCoroutine(IEnumerator _routine)
	{
		routine = _routine;
	}

	private void StartInternal()
	{
		EditorApplication.update += Update;
	}

	private void StopInternal()
	{
		EditorApplication.update -= Update;
	}

	private void Update()
	{
		if (!routine.MoveNext())
		{
			StopInternal();
		}
	}
}