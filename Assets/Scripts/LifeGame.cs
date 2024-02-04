using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class LifeGame : MonoBehaviour
{
   
    public Camera camera;
    public GameObject paintedCellObject;
    public GameObject generationCanvas;
    public GameObject helpCanvas;
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI lifeNumText;
    public float interval;

    private class CellObject
    {
        public GameObject obj;
        public Vector3Int coord;
    }

    private enum UIMode {
        None,
        OnlyStatus,
        All
    }

    private List<CellObject> cellObjects;
    private List<Vector3Int> checkingCells;
    private List<Vector3Int> livingCells;
    private List<Vector3Int> nextLivingCells;
    private List<Vector3Int> nextDeathCells;
    private bool isRunning = false;
    private bool isOneStepSimulation = false;
    private int generation = 1;
    private int lifeNum = 0;
    private UIMode mode = UIMode.All;
    private Vector3Int[] offsets;
    private Vector3Int[] offsetsExceptCenter;
    //private bool isTaskRun = false

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
        offsetsExceptCenter = new Vector3Int[]
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
        lifeNum = cellObjects.Count;
        generationText.text = generation + "";
        lifeNumText.text = lifeNum + "";

        if(isRunning)
        {
            foreach (CellObject cellObject in cellObjects)
            {
                SpriteRenderer sprite = cellObject.obj.GetComponent<SpriteRenderer>();
                sprite.color = new Color32(16, 255, 0, 255);
            }
        }
        else
        {
            foreach (CellObject cellObject in cellObjects)
            {
                SpriteRenderer sprite = cellObject.obj.GetComponent<SpriteRenderer>();
                sprite.color = new Color32(130, 130, 130, 255);
            }
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (mode) {
                case UIMode.None:
                    helpCanvas.SetActive(true);
                    generationCanvas.SetActive(true);
                    mode = UIMode.All;
                    break;
                case UIMode.OnlyStatus:
                    generationCanvas.SetActive(false);
                    mode = UIMode.None;
                    break;
                case UIMode.All:
                    helpCanvas.SetActive(false);
                    mode = UIMode.OnlyStatus;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) { 
            if(cellObjects.Count > 0)
            {
                for (int i = 0; i < cellObjects.Count; i++)
                {
                    Destroy(cellObjects[i].obj);
                }
                cellObjects.Clear();
            }
            generation = 1;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isRunning)
            {
                isRunning = true;
                isOneStepSimulation = true;
                StartCoroutine(ProcessAtInterval());
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
            nextLivingCells = new List<Vector3Int>();
            nextDeathCells = new List<Vector3Int>();
            foreach (CellObject cellObject in cellObjects)
            {
                Vector3Int coord = cellObject.coord;
                if (!livingCells.Contains(coord))
                {
                    livingCells.Add(coord);
                }
                foreach (Vector3Int offset in offsets)
                {
                    Vector3Int adjacentCoord = coord + offset;
                    if (!checkingCells.Contains(adjacentCoord))
                    {
                        checkingCells.Add(adjacentCoord);
                    }
                }
            }

            foreach (Vector3Int coord in checkingCells)
            {
                int livingNumAroundCell = 0;
                foreach (Vector3Int offset in offsetsExceptCenter)
                {
                    if (livingCells.Contains(coord + offset))
                    {
                        livingNumAroundCell++;
                    }
                }
                bool isCurrentlyAlive = livingCells.Contains(coord);
                if (isCurrentlyAlive && (livingNumAroundCell == 2 || livingNumAroundCell == 3))
                {
                    if (!nextLivingCells.Contains(coord))
                    {
                        nextLivingCells.Add(coord);
                    }
                }
                else if (!isCurrentlyAlive && livingNumAroundCell == 3)
                {
                    if (!nextLivingCells.Contains(coord))
                    {
                        nextLivingCells.Add(coord);
                    }
                }
                else
                {
                    if (isCurrentlyAlive)
                    {
                        if (!nextDeathCells.Contains(coord))
                        {
                            nextDeathCells.Add(coord);
                        }
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

            generation++;
            lifeNum = cellObjects.Count;
            if (isOneStepSimulation)
            {
                isOneStepSimulation = false;
                isRunning = false;
            }
            else
            {
                yield return new WaitForSeconds(interval);
            }
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
