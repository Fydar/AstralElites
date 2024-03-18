using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraSizeBehaviour : MonoBehaviour
{
    [SerializeField]
    private float defaultSize = 5.0f;

    void Update()
    {
        var camera = GetComponent<Camera>();

        camera.orthographicSize = Mathf.Max(5.0f, defaultSize / ((float)Screen.width / Screen.height));
    }
}
