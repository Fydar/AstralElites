using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
	[SerializeField] private Text TimeText;

	[Space]
	[SerializeField] private RectTransform HourHand;
	[SerializeField] private RectTransform MinuteHand;

	private void Update ()
	{
		if (Time.renderedFrameCount % 60 == 0)
		{
			UpdateClock ();
		}
	}

	private void UpdateClock ()
	{
		var now = DateTime.Now;

		if (TimeText != null)
		{
			int hours = now.Hour;
			int minutes = now.Minute;
			TimeText.text = hours.ToString ("00") + ":" + minutes.ToString ("00");
		}

		if (HourHand != null)
		{
			HourHand.eulerAngles = new Vector3 (0, 0, (float)(now.TimeOfDay.TotalHours % 12.0) * -360.0f);
		}

		if (MinuteHand != null)
		{
			MinuteHand.eulerAngles = new Vector3 (0, 0, (float)(now.TimeOfDay.Minutes / 60.0) * -360.0f);
		}
	}
}
