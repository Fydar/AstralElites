using System;
using UnityEngine;

[Serializable]
public class PopupPool : GameObjectPool<Popup> { }

public class PopupManager : MonoBehaviour
{
	public static PopupManager instance;

	public PopupPool[] Popups;

	private void Awake()
	{
		instance = this;

		for (int i = 0; i < Popups.Length; i++)
		{
			Popups[i].Flush();
		}
	}

	public T GetPopup<T>()
		where T : Popup
	{
		for (int i = 0; i < Popups.Length; i++)
		{
			var popup = Popups[i];

			if (popup.Template.GetType() == typeof(T))
			{
				return (T)popup.Grab(transform);
			}
		}
		return null;
	}

	public PopupPool GetPopupPool(Type type)
	{
		for (int i = 0; i < Popups.Length; i++)
		{
			var popup = Popups[i];

			if (popup.Template.GetType() == type)
			{
				return popup;
			}
		}
		return null;
	}
}