using UnityEngine;
using UnityEngine.Rendering;

public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect instance;

    public Spring Spring;
    public float PulseVelocity = 10;

    private Volume volume;

    private void Awake()
    {
        volume = GetComponent<Volume>();
        instance = this;
    }

    private void Update()
    {
        Spring.Update(Time.deltaTime);

        volume.weight = Mathf.Abs(Spring.Value);
    }

    public void Pulse(float strength)
    {
        Spring.Velocity += strength * PulseVelocity;
    }
}