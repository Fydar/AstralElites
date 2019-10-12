using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T>
	where T : Component
{
	public T Template;
	public bool Reuse = true;
	public int PrewarmAmount = 20;

	private List<T> pool = new List<T> ();
	private int currentGrabIndex = 0;

	public void Initialise (Transform parent)
	{
		for (int i = 0; i < PrewarmAmount; i++)
		{
			ExpandPool (parent);
		}
	}

	public T Grab ()
	{
		return Grab (null);
	}

	public T Grab (Transform parent)
	{
		if (pool.Count == 0)
		{
			if (Reuse)
			{
				Template.gameObject.SetActive (false);
#if UNITY_EDITOR
				Template.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
				pool.Add (Template);
			}
			else
			{
				Template.gameObject.SetActive (false);
#if UNITY_EDITOR
				Template.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
			}
		}

		if (pool.Count == currentGrabIndex)
			ExpandPool (parent);

		T item = pool[currentGrabIndex];
		item.gameObject.SetActive (true);

#if UNITY_EDITOR
		item.gameObject.hideFlags = HideFlags.None;
#endif

		if (item.transform.parent != parent)
		{
			item.transform.SetParent (parent);
		}
		else
		{
			item.transform.SetAsLastSibling ();
		}
		currentGrabIndex++;

		return item;
	}

	public void Flush ()
	{
		if (pool.Count == 0)
		{
			if (Reuse)
				pool.Add (Template);
			else
			{
				Template.gameObject.SetActive (false);
#if UNITY_EDITOR
				Template.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
			}
		}

		foreach (T item in pool)
		{
			item.gameObject.SetActive (false);
#if UNITY_EDITOR
			item.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
		}

		currentGrabIndex = 0;
	}

	public void Return (T item)
	{
		int itemIndex = pool.IndexOf (item);

		if (itemIndex == -1)
			Debug.LogError ("Item being returned to the pool doesn't belong in it.");

		if (item.gameObject.activeInHierarchy == false)
			Debug.LogError ("Item already cached in the pool");

		pool.RemoveAt (itemIndex);
		pool.Add (item);
		item.gameObject.SetActive (false);
#if UNITY_EDITOR
		item.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
		currentGrabIndex--;
	}

	private void ExpandPool (Transform parent)
	{
		GameObject clone = GameObject.Instantiate (Template.gameObject, parent) as GameObject;

		T button = clone.GetComponent<T> ();
		pool.Add (button);
	}
}