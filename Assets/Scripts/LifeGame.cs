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

    private List<CellObject> cellObjects;
    private List<Vector3Int> checkingCells;
    private List<Vector3Int> livingCells;
    private List<Vector3Int> nextLivingCells;
    private List<Vector3Int> nextDeathCells;
    private bool isRunning;
    private Vector3Int[] offsets;
    private Vector3Int[] offsetsExceptMe;

    void Start()
    {
        cellObjects = new List<CellObject>();
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
        offsetsExceptMe = new Vector3Int[]
        {
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 0, 0),
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
            if(cellObjects.Count > 0)
            {
                for (int i = 0; i < cellObjects.Count; i++)
                {
                    Destroy(cellObjects[i].obj);
                }
                cellObjects.Clear();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunning = !isRunning;
            if (isRunning)
            {
                foreach (CellObject cellObject in cellObjects)
                {
                    SpriteRenderer sprite = cellObject.obj.GetComponent<SpriteRenderer>();
                    sprite.color = new Color32(16, 255, 0, 255);
                }
                StartCoroutine(ProcessAtInterval());
            }
            else
            {
                foreach(CellObject cellObject in cellObjects)
                {
                    SpriteRenderer sprite = cellObject.obj.GetComponent<SpriteRenderer>();
                    sprite.color = new Color32(130, 130, 130, 255);
                }
            }
        }
    }

    IEnumerator ProcessAtInterval()
    {
        while (isRunning)
        {
            checkingCells = new List<Vector3Int>();
            livingCells = new List<Vector3Int>();
            nextLivingCells = new List<Vector3Int>();
            nextDeathCells = new List<Vector3Int>();

            foreach (CellObject cellObject in cellObjects)
            {
                Vector3Int coord = cellObject.coord;
                livingCells.Add(coord);
                foreach (Vector3Int offset in offsets)
                {
                    if (!checkingCells.Contains(coord + offset))
                    {
                        checkingCells.Add(coord + offset);
                    }
                }
            }

            foreach (Vector3Int coord in checkingCells)
            {
                foreach (CellObject cellObject in cellObjects)
                {
                    if (coord == cellObject.coord)
                    {
                        livingCells.Add(coord);
                    }
                }
            }

            foreach (Vector3Int coord in checkingCells)
            {
                int livingNum = 0;
                foreach (Vector3Int offset in offsetsExceptMe)
                {
                    if (livingCells.Contains(coord + offset))
                    {
                        livingNum++;
                    }
                }
                bool isCurrentlyAlive = livingCells.Contains(coord);
                if (isCurrentlyAlive && (livingNum == 2 || livingNum == 3))
                {
                    // 現在生存しており、次も生存する条件
                    nextLivingCells.Add(coord);
                }
                else if (!isCurrentlyAlive && livingNum == 3)
                {
                    // 現在は死んでいるが、次に生まれ変わる条件
                    nextLivingCells.Add(coord);
                }
                else
                {
                    // 死亡、または状態の変更なし
                    if (isCurrentlyAlive)
                    {
                        nextDeathCells.Add(coord);
                    }
                }
            }



            foreach (Vector3Int coord in nextLivingCells)
            {
                PaintCell(coord);
            }

            foreach (Vector3Int coord in nextDeathCells)
            {
                UnpaintCell(coord); 
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
        foreach (CellObject cellItr in cellObjects)
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
            SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
            if (!isRunning)
            {
                sprite.color = new Color32(130, 130, 130, 255);
            }
            cell.coord = point;
            cellObjects.Add(cell);
        }
    }

    void UnpaintCell(Vector3Int point)
    {
        int cellTmp = -1;
        for (int i = 0; i < cellObjects.Count; i++)
        {
            if (cellObjects[i].coord == point)
            {
                cellTmp = i;
                break;
            }
        }
        if (cellTmp != -1)
        {
            Destroy(cellObjects[cellTmp].obj);
            cellObjects.RemoveAt(cellTmp);
        }
    }
}
