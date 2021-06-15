using System;
using UnityEngine;

public class GlobalValue<T> : ScriptableObject
{
	public Action OnChanged;

	[SerializeField]
	protected T currentValue;

	public virtual T Value
	{
		get
		{
			return currentValue;
		}
		set
		{
			currentValue = value;

			if (OnChanged != null)
			{
				OnChanged();
			}
		}
	}
}