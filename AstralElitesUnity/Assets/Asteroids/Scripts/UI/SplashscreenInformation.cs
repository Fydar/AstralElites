﻿using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SplashscreenInformation : MonoBehaviour
{
	public Text Header;
	public Text Info;

	private void Start ()
	{
		UpdateInformation ();
	}

#if UNITY_EDITOR
	private void Update ()
	{
		UpdateInformation ();
	}
#endif

	private void UpdateInformation ()
	{
		Header.text = Application.productName;

		if (Debug.isDebugBuild)
		{
			Info.text = string.Format ("Version {0}\nby {1}", Application.version, Application.companyName);
		}
		else
		{
			Info.text = string.Format ("Version {0}\nby {1}\nDevelopment Build", Application.version, Application.companyName);
		}

	}
}