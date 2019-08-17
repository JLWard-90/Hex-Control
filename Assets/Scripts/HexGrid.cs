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

    [SerializeField]
    GameObject industrial3DModelPrefab;
    Vector3 indModelScale = new Vector3(2,2,2);
    [SerializeField]
    GameObject residential3DModelPrefab;
    Vector3 resModelScale = new Vector3(2, 2, 2);
    [SerializeField]
    GameObject powerplant3DModelPrefab;
    Vector3 powModelScale = new Vector3(3, 3, 3);
    [SerializeField]
    GameObject civic3DModelPrefab;
    Vector3 civicModelScale = new Vector3(2, 2, 2);
    [SerializeField]
    GameObject Landmark3DModelPrefab;
    Vector3 LanModelScale = new Vector3(1, 1, 1);

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

    public void DeselectCell()
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
        placeModel(cell);
    }

    public void UpdateCellLabel(HexCell cell, int i)
    {
        Text label = labels[i];
        label.text = cell.CellTypeShortString;
        placeModel(cell);
    }

    public void initialPlaceModels()
    {
        //Industrial models will be placed 
    }

    public void placeModel(HexCell cell)
    {
        int celltype = cell.cellType;
        if(celltype == 1) //Residential
        {
            GameObject threeDModel = Instantiate<GameObject>(residential3DModelPrefab);
            threeDModel.transform.SetParent(cell.gameObject.transform);
            Vector3 posadj = new Vector3(1, 50, 2);
            Vector3 pos = cell.transform.position + posadj;
            Quaternion rot = Quaternion.Euler(45, 57, 45);
            threeDModel.transform.SetPositionAndRotation(pos, rot);
            threeDModel.transform.localScale = resModelScale;
        }
        else if(celltype == 2)//Industrial
        {
            GameObject threeDModel = Instantiate<GameObject>(industrial3DModelPrefab);
            threeDModel.transform.SetParent(cell.gameObject.transform);
            Vector3 posadj = new Vector3(3, 50, 0);
            Vector3 pos = cell.transform.position + posadj;
            Quaternion rot = Quaternion.Euler(45, 60, 45);
            threeDModel.transform.SetPositionAndRotation(pos, rot);
            threeDModel.transform.localScale = indModelScale;
        }
        else if(celltype == 3) //Power plant
        {
            GameObject threeDModel = Instantiate<GameObject>(powerplant3DModelPrefab);
            threeDModel.transform.SetParent(cell.gameObject.transform);
            Vector3 posadj = new Vector3(4, 50, -1);
            Vector3 pos = cell.transform.position + posadj;
            Quaternion rot = Quaternion.Euler(45, 60, 45);
            threeDModel.transform.SetPositionAndRotation(pos, rot);
            threeDModel.transform.localScale = powModelScale;
        }
        else if(celltype == 4) //Civic
        {
            GameObject threeDModel = Instantiate<GameObject>(civic3DModelPrefab);
            threeDModel.transform.SetParent(cell.gameObject.transform);
            Vector3 posadj = new Vector3(0, 50, 0);
            Vector3 pos = cell.transform.position + posadj;
            Quaternion rot = Quaternion.Euler(-45, -120, -45);
            threeDModel.transform.SetPositionAndRotation(pos, rot);
            threeDModel.transform.localScale = civicModelScale;
        }
        else if(celltype == 5)//Landmark
        {
            GameObject threeDModel = Instantiate<GameObject>(Landmark3DModelPrefab);
            threeDModel.transform.SetParent(cell.gameObject.transform);
            Vector3 posadj = new Vector3(-3, 50, -5);
            Vector3 pos = cell.transform.position + posadj;
            Quaternion rot = Quaternion.Euler(-45,-120,-45);
            threeDModel.transform.SetPositionAndRotation(pos, rot);
            threeDModel.transform.localScale = LanModelScale;
        }
        else
        {
            Debug.Log("Cell type not recognised! No model instantiated.");
        }
         
    }

    public void destroyModel(HexCell cell)
    {
        GameObject threeDModel = cell.gameObject.transform.GetChild(0).gameObject; //I think each HexCell game object should only have a single child
        if(threeDModel != null)
        {
            Destroy(threeDModel);
        }
    }
}
