using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class for updating the minimap UI of the main camera, visualizing where the camera is on the map
/// 日本語：ミニマップ上のカメラの位置を更新し、表現するクラス
/// </summary>
public class CameraMinimapUI : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private void LateUpdate()
    {
        if(lineRenderer == null) { return; }
        Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0), Camera.MonoOrStereoscopicEye.Mono);
        Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0), Camera.MonoOrStereoscopicEye.Mono);
        RaycastHit hit;
        Vector3[] hitPointArray = new Vector3[4];
        Physics.Raycast(topLeft, transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));
        hitPointArray[0] = hit.point;
        Physics.Raycast(topRight, transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));
        hitPointArray[1] = hit.point;
        Physics.Raycast(bottomRight, transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));
        hitPointArray[2] = hit.point;
        Physics.Raycast(bottomLeft, transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));
        hitPointArray[3] = hit.point;

        lineRenderer.SetPositions(hitPointArray);
    }
}
