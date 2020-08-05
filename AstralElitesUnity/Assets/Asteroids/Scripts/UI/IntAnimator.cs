using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class IntAnimator : MonoBehaviour
{
	public GlobalInt Value;
	public float InterpolationSpeed = 10;

	private float currentValue = 0.0f;
	private Text text = null;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	private void Start()
	{
		currentValue = Value.Value;
		text.text = Mathf.RoundToInt(currentValue).ToString();
	}

	private void Update()
	{
		if (currentValue > Value.Value)
		{
			currentValue = Value.Value;
		}

		if (currentValue != Value.Value)
		{
			currentValue = Mathf.Lerp(currentValue, Value.Value, Time.deltaTime * InterpolationSpeed);
		}

		text.text = Mathf.RoundToInt(currentValue).ToString();
	}
}
