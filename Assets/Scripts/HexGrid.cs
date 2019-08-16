using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.green;

    public HexCell[] cells;

    public Text cellLabelPrefab;

    Canvas gridCanvas;

    HexMesh hexMesh;

    Text[] labels;

    public int SelectedCellIndex = -1; //The index of a selected cell

    [SerializeField]
    UIController uiController;

    //GameController gController;

    private void Awake()
    {
        //gController = GameObject.Find("GameController").GetComponent<GameController>();
        SelectedCellIndex = -1;
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        uiController = GameObject.Find("UICanvas").GetComponent<UIController>();

        cells = new HexCell[height * width];
        labels = new Text[height * width];
        for(int z = 0, i = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput0();
        }
        if (Input.GetMouseButton(1))
        {
            HandleInput1();
        }
    }

    void HandleInput0()
    {
        //Debug.Log("click");
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            TouchCell(hit.point);
        }
    }
    void HandleInput1()
    {
        DeselectCell();
    }

    void DeselectCell()
    {
        //Clear all cell selection
        if (SelectedCellIndex != -1)
        {
            HexCell cell = cells[SelectedCellIndex];
            cell.selectedCell = false;
            cell.SetCellTypeProperties();
            if(cell.cellOwner == -1)
            {
                cell.color = defaultColor;
            }
            else
            {
                cell.color = cell.CellOwnerColor;
            }
            SelectedCellIndex = -1;
            hexMesh.Triangulate(cells);
        }
    }

    void TouchCell(Vector3 position)
    {
        //Debug.Log("Touch ");
        //Debug.Log(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        DeselectCell();
        SelectedCellIndex = index;
        HexCell cell = cells[index];
        cell.color = touchedColor;
        cell.selectedCell = true;
        hexMesh.Triangulate(cells);
        uiController.UpdateCellPanel(cell);
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z *0.5f - z / 2) * (GameController.HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * GameController.HexMetrics.outerRadius * 1.5f;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        cell.CellOwnerColor = Color.white;

        SetCellLabel(position, cell, i);
    }

    void SetCellLabel(Vector3 position, HexCell cell, int i)
    {
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        //label.text = x.ToString() + '\n' + z.ToString();
        //label.text = cell.coordinates.ToStringOnSeparateLines();
        label.text = cell.CellTypeShortString;
        labels[i] = label; //Add this label into the array of labels
    }

    public void UpdateCellLabel(HexCell cell, int i)
    {
        Text label = labels[i];
        label.text = cell.CellTypeShortString;
    }

    public void placeModel()
    {
        //This method will place the appropriate 3D model on top of each tile.
        //The transform for the industrial model needs to be scaled to 2 on each axis.
        //The models need to be placed at the central position of each hexCell but with a y=2 to place it above the cell so that it is visible.
        //Either the camera should be set to an angle of 45degrees on the X-axis meaning that it needs to be moved to (50,50,-5), or the models will need to be placed (with a different scaling) 
        //at an angle to the camera. It is probably easier to move the camera than the models.
    }
}
