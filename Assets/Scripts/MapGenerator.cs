using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject molePrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private GameObject dynamitePrefab;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] [Range(2,50)] private int mapSize = 10;
    [SerializeField] [Range(1,100)] private int moles = 10;
    
    [SerializeField] private TextMeshProUGUI molesText;
    [SerializeField] private TextMeshProUGUI tntText;

    private readonly string _increaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Increase);
    private readonly string _decreaseTxt = Enum.GetName(typeof(ChangeTypes.Types), ChangeTypes.Types.Decrease);
    
    private LevelManager _levelManager;
    private SaveProgress _saveProgress;
    private List<Vector3> _molePosition;
    private List<Vector3> _moleNeighbors;
    private List<Vector3> _grass;
    public List<Vector3> currentGrass;

    private int _buttonSelection;
    private int _seed = 0;
    private bool _isMoleSelected = false;
    private int _tntCounter = 0;
    private DifficultyTypes.Types _difficulty;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _saveProgress = FindObjectOfType<SaveProgress>();
        // _saveProgress.LoadData();
    }

    void Start()
    {
        _seed = _saveProgress.GetCurrentLevel();
        _difficulty = _saveProgress.GetCurrentDifficulty();
        
        InitializeVariables();
        GenerateMapData();
    }

    void InitializeVariables()
    {
        _tntCounter = 0;
        Random.InitState(_seed);
        _buttonSelection = (int) TileTypes.Types.Grass;
        _isMoleSelected = false;
        _molePosition = new List<Vector3>();
        _moleNeighbors = new List<Vector3>();
        _grass = new List<Vector3>();
        currentGrass = new List<Vector3>();
    }
    
    void CleanMap()
    {
        CleanData();
        foreach (var child in gameObject.GetComponentsInChildren<Tile>())
        {
            Destroy(child.gameObject);
        }
    }

    void CleanData()
    {
        _tntCounter = 0;
        Random.InitState(_seed);
        _buttonSelection = (int) TileTypes.Types.Grass;
        _isMoleSelected = false;
        _molePosition.Clear();
        _moleNeighbors.Clear();
        _grass.Clear();
        currentGrass.Clear();
    }
    
    void GenerateMapData()
    {
        Progress progress = new Progress(_seed, _difficulty, true);
        _saveProgress.AddProgress(progress);
        
        GenerateMoles();
        CleanNeighbors();
        
        GenerateMapVisuals();
        CleanGrass();
        
        CenterCamera();
        
        _levelManager.ChangeLevelText($"{_seed}");
    }
    
    void GenerateMoles()
    {
        molesText.text = $"{moles}";
        tntText.text = $"{_tntCounter}";
        
        int maxMoles = mapSize * mapSize - 1;
        if (moles > maxMoles)
            moles = maxMoles;
        
        for (int i = 0; i < moles; i++)
        {
            int x = GetRandomPositionNumber();
            int y = GetRandomPositionNumber();
            
            Vector3 position = new Vector3(x,y,0f);

            if (_molePosition.Contains(position))
            {
                i--;
                continue;
            }
            
            _molePosition.Add(position);
            List<Vector3> neighbors = GetNeighbors(position);
            _moleNeighbors.AddRange(neighbors);
        }
    }

    int GetRandomPositionNumber()
    {
        return Mathf.Clamp(Random.Range(0, mapSize), 0, mapSize);
    }

    public List<Vector3> GetNeighbors(Vector3 position)
    {
        List<Vector3> neighbors = new List<Vector3>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if(x == 0 && y == 0)
                    continue;
                
                Vector3 neighbor = position + new Vector3(x, y, 0);
                
                if(IsPositionInMap(neighbor))
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public bool IsPositionInMap(Vector3 position)
    {
        bool isXInMap = position.x >= 0 && position.x <= mapSize;
        bool isYInMap = position.y >= 0 && position.y <= mapSize;
        return isXInMap && isYInMap;
    }

    void CleanNeighbors()
    {
        _moleNeighbors = _moleNeighbors.ToList();
    }

    void GenerateMapVisuals()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector3 position = new Vector3(x, y, 0f);
                
                _grass.Add(position);
                
                InstantiateHole(position);
            }
        }
    }

    void CleanGrass()
    {
        List<Vector3> nonGrass = new List<Vector3>();
        nonGrass.AddRange(_molePosition);
        nonGrass.AddRange(_moleNeighbors);
        _grass = _grass.Where(g => !nonGrass.Contains(g)).ToList();
    }
    
    void CenterCamera()
    {
        //TODO: Organizar el centrado en el mapa
        // float halfMapSize = mapSize / 2f;
        // int width = Screen.width;
        // int height = Screen.height;
        // int test = width - mapSize;
        
        Vector3 center = new Vector3(4.5f,2,-10f);
        mainCamera.transform.position = center;
    }

    public void InstantiateMole(Vector3 position)
    {
        _isMoleSelected = true;
        string _tag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Mole);
        InstantiateTile(molePrefab, position, _tag);
    }
    public void InstantiateNumber(Vector3 position)
    {
        string _tag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Number);
        InstantiateTile(numberPrefab, position, _tag);
    }
    public void InstantiateGrass(Vector3 position)
    {
        string _tag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Grass);
        InstantiateTile(grassPrefab, position, _tag);
    }
    public void InstantiateDynamite(Vector3 position)
    {
        string _tag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Dynamite);
        InstantiateTile(dynamitePrefab, position, _tag);
    }
    public void InstantiateHole(Vector3 position)
    {
        string _tag = Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Hole);
        InstantiateTile(holePrefab, position, _tag);
    }
    
    void InstantiateTile(GameObject prefab, Vector3 position, string _tag)
    {
        GameObject tile = Instantiate(prefab, position , Quaternion.identity);
        ConfigureTile(tile, $"{_tag} ({position.x},{position.y})", _tag);
    }
    void ConfigureTile(GameObject tile, string name, string _tag)
    {
        tile.name = name;
        tile.tag = _tag;
        tile.transform.SetParent(transform);
        tile.isStatic = true;
        
        if (_tag != Enum.GetName(typeof(TileTypes.Types), TileTypes.Types.Number)) return;
        
        int neighborCount = _moleNeighbors.Count(mn => mn == tile.transform.position);
        tile.GetComponentInChildren<TextMeshPro>().text = $"{neighborCount}";
    }

    public void SetButtonSelection(string type)
    {
        var selection = (int)Enum.Parse(typeof(TileTypes.Types), type);
        _buttonSelection = selection;
    }
    public int GetButtonSelection()
    {
        return _buttonSelection;
    }

    public Tile GetTileByPosition(Vector3 position)
    {
        return gameObject.GetComponentsInChildren<Tile>().FirstOrDefault(t=>t.transform.position == position);
    }
    public List<Vector3> GetGrass()
    {
        return _grass;
    }
    public List<Vector3> GetNumbers()
    {
        return _moleNeighbors;
    }
    
    public bool IsPositionAMole(Vector3 position)
    {
        return _molePosition.Contains(position);
    }
    public bool IsPositionAMoleNeighbor(Vector3 position)
    {
        return _moleNeighbors.Contains(position);
    }
    public bool IsPositionGrass(Vector3 position)
    {
        return _grass.Contains(position);
    }

    public bool HasMoleBeenSelected()
    {
        _levelManager.ActivateRedoButton(_isMoleSelected);
        return _isMoleSelected;
    }

    public void ChangeLevelSeed(int seed)
    {
        _seed = seed;
        
        CleanMap();
        
        GenerateMapData();
        
        _saveProgress.SaveData();
    }

    public int GetSeed()
    {
        return _seed;
    }

    public void ChangeTnTCounter(string change)
    {
        if (change == _increaseTxt && _tntCounter <= mapSize*mapSize)
            _tntCounter++;
        else if (change == _decreaseTxt && _tntCounter >= 0 )
            _tntCounter--;
        tntText.text = $"{_tntCounter}";
    }
    
    
}
