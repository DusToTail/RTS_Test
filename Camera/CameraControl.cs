using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for controlling the movement and rotation of a orthographic camera based on the mouse input of the user
/// </summary>
public class CameraControl : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector2 offset;

    [Header("Ground")]
    [SerializeField]
    private Transform ground;

    [Header("Others")]
    [SerializeField]
    private float maxSize;
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float mouseScrollSensitivity;
    [SerializeField]
    private float mouseDragSensitivity;

    private Transform bottomLeft;
    //private Transform bottomRight;
    //private Transform topLeft;
    private Transform topRight;

    private void Awake()
    {
        
    }

    private void Start()
    {
        bottomLeft = ground.GetChild(0);
        //bottomRight = ground.GetChild(1);
        //topLeft = ground.GetChild(2);
        topRight = ground.GetChild(3);

        ApplyOffset(offset);
    }

    private void LateUpdate()
    {
        //LookAtTarget();
        float x = 0;
        float y = 0;
        if (Input.mousePosition.x > Screen.width - 10)
            x = 1;
        else if (Input.mousePosition.x < 1)
            x = -1;
        else
            x = 0;

        if(Input.mousePosition.y > Screen.height - 10)
            y = 1;
        else if(Input.mousePosition.y <= 1)
            y = -1;
        else
            y = 0;

        AdjustPosition(new Vector2(x, y), mouseDragSensitivity);
        AdjustZoom(Input.mouseScrollDelta.y * mouseScrollSensitivity);
    }

    /// <summary>
    /// Move Camera to the specified position through Translation instantly, disregarding y-axis
    /// </summary>
    /// <param name="_mapPosition"></param>
    public void MoveCameraTo(Vector3 _mapPosition)
    {
        Vector3 dir = _mapPosition - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        transform.Translate(dir, Space.World);
        target.transform.Translate(dir, Space.World);

        AdjustForCameraBorder();
    }

    /// <summary>
    /// Move Camera on xz plane, disregarding y axis
    /// </summary>
    /// <param name="_vector"></param>
    /// <param name="_amount"></param>
    private void AdjustPosition(Vector2 _vector, float _amount)
    {
        Vector3 dir = new Vector3(_vector.x, 0, _vector.y);
        transform.Translate(dir.normalized * _amount, Space.World);
        target.transform.Translate(dir.normalized * _amount, Space.World);

        AdjustForCameraBorder();
    }

    /// <summary>
    /// Minus the camera orthographicSize property by an amount
    /// </summary>
    /// <param name="_amount"></param>
    private void AdjustZoom(float _amount)
    {
        if(GetComponent<Camera>() == null) {  return; }
        GetComponent<Camera>().orthographicSize -= _amount;
        if (GetComponent<Camera>().orthographicSize < minSize)
            GetComponent<Camera>().orthographicSize = minSize;
        if(GetComponent<Camera>().orthographicSize > maxSize)
            GetComponent<Camera>().orthographicSize = maxSize; ;
    }

    /// <summary>
    /// Block the camera going outside of the premise of Transform Ground
    /// </summary>
    private void AdjustForCameraBorder()
    {
        if (GetComponent<Camera>() == null) { return; }

        if (target.transform.position.x < bottomLeft.position.x)
        {
            target.transform.position = new Vector3(bottomLeft.position.x, target.transform.position.y, target.transform.position.z);
        }
        else if (target.transform.position.x > topRight.position.x)
        {
            target.transform.position = new Vector3(topRight.position.x, target.transform.position.y, target.transform.position.z);
        }

        if (target.transform.position.z < bottomLeft.position.z)
        {
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, bottomLeft.position.z);
        }
        else if (target.transform.position.z > topRight.position.z)
        {
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, topRight.position.z);
        }
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.position += new Vector3(offset.x, 0, offset.y);
    }

    /// <summary>
    /// Apply Offset to the camera from the target
    /// </summary>
    /// <param name="_offset"></param>
    private void ApplyOffset(Vector2 _offset)
    {
        if(target == null) { return; }
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.position += new Vector3(_offset.x, 0, _offset.y);
    }

}
