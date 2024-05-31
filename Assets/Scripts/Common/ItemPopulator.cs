using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemPopulator : MonoBehaviour
{
    private CleaningManager _cleaningManager;

    public Vector2[] polygonVertices; // Vertices defining the polygonal area
    public int trashNumber = 0;
    public int plantNumber = 0;
    public float minDistance = 1.0f; // Minimum distance between items

    private List<Vector2> usedPositions = new List<Vector2>();

    void Start()
    {
        _cleaningManager = CleaningManager.Instance;

        if (polygonVertices == null || polygonVertices.Length < 3)
        {
            Debug.LogError("Polygon vertices must have at least 3 points.");
            return;
        }
    }

    public void PopulateItems(bool reset = true)
    {
        if (reset)
            _cleaningManager.Reset();
        
        for (int i = 0; i < trashNumber; i++)
        {
            Vector2 position = GetValidRandomPointInPolygon();
            _cleaningManager.SpawnTrash(new Vector3(position.x+transform.position.x, transform.position.y, position.y+transform.position.z));
        }

        for (int i = 0; i < plantNumber; i++)
        {
            Vector2 position = GetValidRandomPointInPolygon();
            _cleaningManager.SpawnPlant(new Vector3(position.x+transform.position.x, transform.position.y, position.y+transform.position.z));
        }
    }

    Vector2 GetValidRandomPointInPolygon()
    {
        Vector2 randomPoint;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            randomPoint = GetRandomPointInPolygon();
            attempts++;
        } while (!IsValidPosition(randomPoint) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a valid position after maximum attempts.");
        }

        usedPositions.Add(randomPoint);
        return randomPoint;
    }

    bool IsValidPosition(Vector2 point)
    {
        foreach (var usedPosition in usedPositions)
        {
            if (Vector2.Distance(point, usedPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    Vector2 GetRandomPointInPolygon()
    {
        
        // Find the bounding box of the polygon
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector2 vertex in polygonVertices)
        {
            minX = Mathf.Min(minX, vertex.x);
            maxX = Mathf.Max(maxX, vertex.x);
            minY = Mathf.Min(minY, vertex.y);
            maxY = Mathf.Max(maxY, vertex.y);
        }

        // Generate random points until a point inside the polygon is found
        Vector2 randomPoint;
        do
        {
            randomPoint = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );
        } while (!IsPointInPolygon(randomPoint));

        return randomPoint;
    }


    bool IsPointInPolygon(Vector2 point)
    {
        bool isInside = false;
        int j = polygonVertices.Length - 1;

        for (int i = 0; i < polygonVertices.Length; i++)
        {
            if ((polygonVertices[i].y < point.y && polygonVertices[j].y >= point.y ||
                polygonVertices[j].y < point.y && polygonVertices[i].y >= point.y) &&
                (polygonVertices[i].x + (point.y - polygonVertices[i].y) / (polygonVertices[j].y - polygonVertices[i].y) * (polygonVertices[j].x - polygonVertices[i].x) < point.x))
            {
                isInside = !isInside;
            }
            j = i;
        }

        return isInside;
    }

    void OnDrawGizmos()
    {
        if (polygonVertices == null || polygonVertices.Length < 3)
            return;

        Gizmos.color = Color.green;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.magenta;

        for (int i = 0; i < polygonVertices.Length; i++)
        {
            int nextIndex = (i + 1) % polygonVertices.Length;
            Vector3 position1 = new Vector3(
                transform.position.x + polygonVertices[i].x,
                transform.position.y,
                transform.position.z + polygonVertices[i].y
            );
            Vector3 position2 = new Vector3(
                transform.position.x + polygonVertices[nextIndex].x,
                transform.position.y,
                transform.position.z + polygonVertices[nextIndex].y
            );
            
            Gizmos.DrawLine(position1, position2);
            
            Handles.Label(position1, $"{i}", style);
        }
    }
}
