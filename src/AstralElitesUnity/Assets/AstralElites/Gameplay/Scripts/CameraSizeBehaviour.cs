using UnityEngine;

[ExecuteAlways]
public class CameraSizeBehaviour : MonoBehaviour
{
    [SerializeField]
    private float defaultSize = 5.0f;

    private void Update()
    {
        var camera = GetComponent<Camera>();

        camera.orthographicSize = Mathf.Max(5.0f, defaultSize / ((float)Screen.width / Screen.height));
    }
}
