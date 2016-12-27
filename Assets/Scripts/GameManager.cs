using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MainWindow mainWindow = null;

    public CameraController camController = null;
    public MeshRenderer grid = null;

    public Transform poolContainer = null;
    public Transform buildingContainer = null;

    public Building[] poolReservePrefabs = null;
    public int poolReserveCount = 20;

    public Color cellRightColor = Color.green.setA(0.5f);
    public Color cellWrongColor = Color.red.setA(0.5f);

    public Size fieldSize = new Size {x = 100, y = 100};

    public Building currentBuilding { get; private set; }
    public Building selectedBuilding { get; private set; }

    public bool[,] field { get; private set; }
    public bool fieldHalfX { get; private set; }
    public bool fieldHalfY { get; private set; }
    public int engagedCellsCount { get; private set; }

    public bool isBuildMode
    {
        get { return currentBuilding != null; }
    }

    public MultiObjectPool pool { get; private set; }

    public void lockBuilding(Building prefab)
    {
        if (currentBuilding != null)
        {
            pool.unlockObj(currentBuilding);
        }
        currentBuilding = pool.lockObj(prefab);
    }

    public void resetCurrentBuilding()
    {
        if (isBuildMode)
        {
            pool.unlockObj(currentBuilding);
            currentBuilding = null;
        }
    }

    public void resetSelectedBuilding()
    {
        selectedBuilding = null;
    }

    public bool checkCellAvailability(Vector3 pos)
    {
        var x = Mathf.FloorToInt(pos.x) + fieldSize.x/2;
        var y = Mathf.FloorToInt(pos.z) + fieldSize.y/2;

        return x >= 0 && x < fieldSize.x && y >= 0 && y < fieldSize.y && !field[x, y];
    }

    public bool setCellAvailability(Vector3 pos, bool engaged)
    {
        var x = Mathf.FloorToInt(pos.x) + fieldSize.x/2;
        var y = Mathf.FloorToInt(pos.z) + fieldSize.y/2;

        if (x >= 0 && x < fieldSize.x && y >= 0 && y < fieldSize.y)
        {
            field[x, y] = engaged;
            engagedCellsCount += engaged ? 1 : -1;
        }
        else
        {
            Debug.LogError("GameManager: Wrong array index " + x + "," + y);
        }

        return x < 0 || x >= fieldSize.x || y < 0 || y >= fieldSize.y || !field[x, y];
    }

    protected override void Initialize()
    {
        pool = new MultiObjectPool(poolContainer, 0);
        if (poolReservePrefabs != null)
        {
            for (var i = 0; i < poolReservePrefabs.Length; ++i)
            {
                pool.reserve(poolReservePrefabs[i], poolReserveCount);
            }
        }

        grid.transform.localScale = new Vector3(fieldSize.x, fieldSize.y, 1);
        grid.material.mainTextureScale = new Vector2(fieldSize.x, fieldSize.y);

        field = new bool[fieldSize.x, fieldSize.y];
        fieldHalfX = fieldSize.x%2 > 0;
        fieldHalfY = fieldSize.y%2 > 0;

        mainWindow.onLeftMouseButtonClick += _onLeftMouseButtonClick;
        mainWindow.onRightMouseButtonClick += _onRightMouseButtonClick;

        var cnt = Mathf.FloorToInt(0.1f*fieldSize.x*fieldSize.y);
        while (engagedCellsCount < cnt)
        {
            if (poolReservePrefabs != null)
            {
                var b = pool.lockObj(poolReservePrefabs[Random.Range(0, poolReservePrefabs.Length)]);
                var i = 0;
                while (!b.canBuild && i < 5)
                {
                    var x = Random.Range(0, fieldSize.x) - fieldSize.x/2f + 0.5f;
                    var y = Random.Range(0, fieldSize.y) - fieldSize.y/2f + 0.5f;
                    b.setPosition(new Vector3(x, 0, y));
                    ++i;
                }
                if (b.canBuild)
                {
                    b.build();
                }
                else
                {
                    pool.unlockObj(b);
                }
            }
        }

        //for (var i = 0; i < 99; ++i)
        //{
        //    var b = pool.lockObj(poolReservePrefabs[0]);
        //    buildingContainer.addChildResetPRS(b);
        //    b.setPosition(new Vector3(i - 49f, 0, 0));
        //    b.build();
        //}
    }

    private void _rayCast(Vector3 pos)
    {
        var ray = camController.cam.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Building building = null;
            if (hit.transform.parent != null)
            {
                building = hit.transform.parent.GetComponent<Building>();
            }
            if (building != null)
            {
                if (selectedBuilding != building)
                {
                    selectedBuilding = building;
                    mainWindow.showInfo(pos, selectedBuilding.transform.position);
                }
                else
                {
                    selectedBuilding = building;
                }
            }
        }
    }

    private void _onLeftMouseButtonClick(Vector3 pos)
    {
        if (isBuildMode)
        {
            if (currentBuilding.canBuild)
            {
                if (currentBuilding.build())
                {
                    currentBuilding = null;
                }
            }
        }
        else
        {
            _rayCast(pos);
        }
    }

    private void _onRightMouseButtonClick(Vector3 pos)
    {
        if (isBuildMode)
        {
            resetCurrentBuilding();
        }
        else
        {
            _rayCast(pos);
        }
    }

    private void Update()
    {
        if (isBuildMode)
        {
            var ray = camController.cam.ScreenPointToRay(Input.mousePosition);
            var pos = ray.GetPoint(ray.origin.y/Mathf.Cos(Mathf.Deg2Rad*Vector3.Angle(Vector3.down, ray.direction)));
            pos.y = 0;
            currentBuilding.setPosition(pos);
        }
    }
}