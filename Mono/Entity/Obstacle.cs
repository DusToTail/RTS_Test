using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: a class for all obstacles (can be attached as a child to structure)
/// 日本語：障害物のクラス（ビルディングなどにChildとしてつけることができる）
/// </summary>
public class Obstacle : MonoBehaviour, IObstacle
{
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private bool displayGizmos;
    private Vector3 startPosition;
    private float cellRadius = 1;

    private void Start()
    {
        cellRadius = FindObjectOfType<GridController>().cellRadius;
        gridSize = new Vector2Int(Mathf.FloorToInt(gridSize.x / cellRadius), Mathf.FloorToInt(gridSize.y / cellRadius));

        startPosition = transform.position - new Vector3(gridSize.x, 0, gridSize.y) * cellRadius;
    }


    public Vector2Int GetObstacleGridSize()
    {
        return gridSize;
    }
    
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        startPosition = transform.position - new Vector3(gridSize.x, 0, gridSize.y) * cellRadius;
        Gizmos.color = Color.yellow;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 center = startPosition + new Vector3(cellRadius * 2 * x + cellRadius, 0, cellRadius * 2 * y + cellRadius);
                Vector3 size = Vector3.one * cellRadius * 2;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }

}
