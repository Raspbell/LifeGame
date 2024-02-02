using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class LifeGame : MonoBehaviour
{
   
    public Camera camera;
    public GameObject paintedCellObject;
    public float interval;

    private class CellObject
    {
        public GameObject obj;
        public Vector3Int coord;
    }

    private class CellStatus
    {
        public bool isLiving;
        public int livingNum;
        public Vector3Int coord;
    }

    private List<CellObject> cells;
    private List<Vector3Int> checkingCells;
    private List<Vector3Int> livingCells;
    private bool isRunning;
    private Vector3Int[] offsets;

    void Start()
    {
        cells = new List<CellObject>();
        offsets = new Vector3Int[]
        {
            new Vector3Int(-1, 1, 0),  
            new Vector3Int(0, 1, 0),   
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),  
            new Vector3Int(-1, -1, 0), 
            new Vector3Int(0, -1, 0),  
            new Vector3Int(1, -1, 0)  
        };
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3Int point = GetWorldPointInt(Input.mousePosition);
            PaintCell(point);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3Int point = GetWorldPointInt(Input.mousePosition);
            UnpaintCell(point);
        }

        if(Input.GetKey(KeyCode.R)) { 
            if(cells.Count > 0)
            {
                for (int i = 0; i < cells.Count; i++)
                {
                    Destroy(cells[i].obj);
                }
                cells.Clear();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunning = !isRunning;
            if (isRunning)
            {
                StartCoroutine(ProcessAtInterval());
            }
        }
    }

    IEnumerator ProcessAtInterval()
    {
        while (isRunning)
        {
            checkingCells = new List<Vector3Int>();
            livingCells = new List<Vector3Int>();
            foreach (CellObject cell in cells)
            {
                Vector3Int coord = cell.coord;
                livingCells.Add(coord);
            }

            foreach (Vector3Int offset in offsets)
            {
                if(!checkingCells.Contains(coord + offset))
                {
                    checkingCells.Add(coord + offset);
                }
            }

            foreach(Vector3Int coord in checkingCells)
            {

            }

            yield return new WaitForSeconds(interval);
        }
    }

    Vector3Int GetWorldPointInt(Vector3 screenPoint)
    {
        screenPoint.z = 10.0f;
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
        return new Vector3Int(Mathf.FloorToInt(worldPoint.x), Mathf.FloorToInt(worldPoint.y), Mathf.FloorToInt(worldPoint.z));
    }

    void PaintCell(Vector3Int point)
    {
        bool isCoordExisted = false;
        foreach (CellObject cellItr in cells)
        {
            if (cellItr.coord == point)
            {
                isCoordExisted = true;
            }
        }
        if (!isCoordExisted)
        {
            GameObject obj = Instantiate(paintedCellObject, point + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            CellObject cell = new CellObject();
            cell.obj = obj;
            cell.coord = point;
            cells.Add(cell);
        }
    }

    void UnpaintCell(Vector3Int point)
    {
        int cellTmp = -1;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].coord == point)
            {
                cellTmp = i;
                break;
            }
        }
        if (cellTmp != -1)
        {
            Destroy(cells[cellTmp].obj);
            cells.RemoveAt(cellTmp);
            Debug.Log(cells.Count);
        }
    }
}
