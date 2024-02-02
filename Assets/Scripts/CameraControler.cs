using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public float cameraMoveSpeed;
    public float cameraScaleSpeed;
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal") * cameraMoveSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxis("Vertical") * cameraMoveSpeed * Time.deltaTime;
        gameObject.transform.position += new Vector3(horizontalInput, verticalInput, 0);
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * cameraScaleSpeed;
        camera.orthographicSize -= scrollInput;

        if (camera.orthographicSize < 5)
        {
            camera.orthographicSize = 5;
        }
    }
}
