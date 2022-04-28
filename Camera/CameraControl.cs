using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private float maxSize;
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float mouseScrollSensitivity;
    [SerializeField]
    private float mouseDragSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        LookAtTarget(offset);
    }

    // Update is called once per frame
    void Update()
    {
        //LookAtTarget();
        float x = 0;
        float y = 0;
        if (Input.mousePosition.x > Screen.width -1)
            x = 1;
        else if (Input.mousePosition.x < 1)
            x = -1;
        else
            x = 0;

        if(Input.mousePosition.y > Screen.height - 1)
            y = 1;
        else if(Input.mousePosition.y <= 1)
            y = -1;
        else
            y = 0;
        AdjustPosition(new Vector2(x, y), mouseDragSensitivity);
        AdjustZoom(Input.mouseScrollDelta.y * mouseScrollSensitivity);
    }

    private void AdjustPosition(Vector2 _vector, float _amount)
    {
        Vector3 dir = new Vector3(_vector.x, 0, _vector.y);
        transform.Translate(dir.normalized * _amount, Space.World);
        target.transform.Translate(dir.normalized * _amount, Space.World);
    }

    private void AdjustZoom(float _amount)
    {
        GetComponent<Camera>().orthographicSize -= _amount;
        if (GetComponent<Camera>().orthographicSize < minSize)
            GetComponent<Camera>().orthographicSize = minSize;
        if(GetComponent<Camera>().orthographicSize > maxSize)
            GetComponent<Camera>().orthographicSize = maxSize; ;

        //Vector3 dir = (target.position - transform.position).normalized;
        //transform.Translate(dir * _amount, Space.World);
        //if (Vector3.Distance(transform.position, target.position) > maxDistance)
        //    transform.position = target.position - dir * maxDistance;
        //if (Vector3.Distance(transform.position, target.position) < minDistance)
        //    transform.position = target.position - dir * minDistance;
    }

    private void LookAtTarget(Vector2 _offset)
    {
        if(target == null) { return; }
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.position += new Vector3(_offset.x, 0, _offset.y);
        transform.LookAt(target);
    }
}
