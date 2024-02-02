using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMesh : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float cellSize = 1.0f;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[(gridSizeX + 1) * (gridSizeY + 1)];
        int[] triangles = new int[gridSizeX * gridSizeY * 6];

        for (int i = 0, y = 0; y <= gridSizeY; y++)
        {
            for (int x = 0; x <= gridSizeX; x++, i++)
            {
                vertices[i] = new Vector3(x * cellSize, y * cellSize, 0);
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < gridSizeY; y++, vi++)
        {
            for (int x = 0; x < gridSizeX; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + gridSizeX + 1;
                triangles[ti + 5] = vi + gridSizeX + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 localPoint = transform.InverseTransformPoint(hit.point);
                int x = Mathf.FloorToInt(localPoint.x / cellSize);
                int y = Mathf.FloorToInt(localPoint.y / cellSize);
                Debug.Log($"Clicked on cell: {x}, {y}");

                // ここでセルの色を赤に変更する処理を呼び出します
            }
        }
    }

}
