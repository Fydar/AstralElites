﻿using UnityEngine;
using UnityEngine.UI;

public class IntBar : MonoBehaviour
{
    public Character Target;

    [Header("Primary Bar")]

    [SerializeField]
    private Image primaryBar;

    [Header("Secondary Bar")]

    [SerializeField]
    private Image secondaryBar;

    [SerializeField]
    private float secondaryBarDelay = 1.0f;
    private float lastValue = 0.0f;

    private void Start()
    {
        Target.Health.OnAfterChanged += UpdateBars;

        UpdateBars();
    }

    private void Update()
    {
        if (secondaryBar != null)
        {
            if (secondaryBar.fillAmount > primaryBar.fillAmount)
            {
                SetImageFill(secondaryBar, Mathf.Lerp(GetFillAmount(secondaryBar), primaryBar.fillAmount, Time.deltaTime * secondaryBarDelay));
            }
            else
            {
                SetImageFill(secondaryBar, primaryBar.fillAmount);
            }
        }
    }

    private void UpdateBars()
    {
        float value = Target.Health.Value;
        float max = 100.0f;

        SetImageFill(primaryBar, value / max);

        lastValue = value;
    }

    private void SetImageFill(Image image, float fill)
    {
        if (image.type == Image.Type.Filled)
        {
            image.fillAmount = fill;
        }
        else
        {
            image.rectTransform.anchorMax = new Vector2(fill, 1.0f);
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
        }
    }

    private float GetFillAmount(Image image)
    {
        return image.type == Image.Type.Filled ? image.fillAmount : image.rectTransform.anchorMax.x;
    }
}
