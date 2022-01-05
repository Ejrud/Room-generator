using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Grid _grid;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _wallsTilemap;
    [SerializeField] private GameObject groundTiles;
    [SerializeField] private GameObject wallTiles;

    [Header("Settings")]
    [Range(3, 24)] [SerializeField] private int _hollwayCount;
    [Range(3, 15)] [SerializeField] private int _hollwayLenght;

    [Header("Tiles")]
    [SerializeField] private Tile _floor;
    [SerializeField] private Tile _wall;

    private Vector3Int _currentPosition;
    private Vector3Int _startFloorPosition, _endFloorPosition;
    private Vector3Int _startWallPosition, _endWallPosition;

    private HashSet<Vector3Int> _crosswalkPositions = new HashSet<Vector3Int>();

    private int previousDirIndex = 0;
    
    private void Start()
    {
        GenerateHollways();

        groundTiles.SetActive(false); // update light2d
        groundTiles.SetActive(true);
        wallTiles.SetActive(false); 
        wallTiles.SetActive(true); 
    }

    private void GenerateHollways()
    {
        _wallsTilemap.SetTile(new Vector3Int(-300, -300, 0), _wall); // Set borders for fucking boxFiil...
        _wallsTilemap.SetTile(new Vector3Int(300, 300, 0), _wall);
        _groundTilemap.SetTile(new Vector3Int(-300, -300, 0), _wall);
        _groundTilemap.SetTile(new Vector3Int(300, 300, 0), _wall);
        

        _currentPosition = new Vector3Int(0, 0, 0);
        _crosswalkPositions.Add(_currentPosition);

        for (int i = 0; i < _hollwayCount; i++)
        {
            PrepareHollway();

            _groundTilemap.BoxFill(_currentPosition, null, _startFloorPosition.x, _startFloorPosition.y, _endFloorPosition.x, _endFloorPosition.y);
            _groundTilemap.BoxFill(_currentPosition, _floor, _startFloorPosition.x, _startFloorPosition.y, _endFloorPosition.x, _endFloorPosition.y);
            _wallsTilemap.BoxFill(_currentPosition, null, _startWallPosition.x, _startWallPosition.y, _endWallPosition.x, _endWallPosition.y);
            _wallsTilemap.BoxFill(_currentPosition, _wall, _startWallPosition.x, _startWallPosition.y, _endWallPosition.x, _endWallPosition.y);

            _crosswalkPositions.Add(_currentPosition);
        }

        ClearWalls();

        _crosswalkPositions = DeleteRepeatPositions(_crosswalkPositions);

        foreach (Vector3Int position in _crosswalkPositions)
        {
            Debug.Log(position);
        }
    }

    private void PrepareHollway()
    {
        int indexDirection = Random.Range(0, 5);        // 1 - left, 2 - top, 3 - right, 4 - bottom

        if (indexDirection == previousDirIndex)
        {
            if(indexDirection != 4)
            {
                indexDirection++;
            }
            else
            {
                indexDirection--;
            }
        }
                                                        // set directions
        switch (indexDirection)
        {
            case 1:
                CalculateFloorSize(-_hollwayLenght - 1, -1, 1, 1);
                CalculateWallSize(-_hollwayLenght - 2, -2, 2, 2);
                _currentPosition += new Vector3Int(-_hollwayLenght, 0, 0);
                break;

            case 2:
                CalculateFloorSize(-1, -1, 1, 1 + _hollwayLenght);
                CalculateWallSize(-2, -2, 2, 2 + _hollwayLenght);
                _currentPosition += new Vector3Int(0, _hollwayLenght, 0);
                break;

            case 3:
                CalculateFloorSize(-1, -1, 1 + _hollwayLenght, 1);
                CalculateWallSize(-2, -2, 2 + _hollwayLenght, 2);
                _currentPosition += new Vector3Int(_hollwayLenght, 0, 0);
                break;

            case 4:
                CalculateFloorSize(-1, -1 - _hollwayLenght, 1, 1);
                CalculateWallSize(-2, -2 - _hollwayLenght, 2, 2);
                _currentPosition += new Vector3Int(0, -_hollwayLenght, 0);
                break;
        }

        previousDirIndex = indexDirection;
    }

    private void CalculateFloorSize(int startOffsetX, int startOffsetY, int endOffsetX, int endOffsetY)
    {
        _startFloorPosition = new Vector3Int(_currentPosition.x + startOffsetX, _currentPosition.y + startOffsetY, 0);
        _endFloorPosition = new Vector3Int(_currentPosition.x + endOffsetX, _currentPosition.y + endOffsetY, 0);
    }

    private void CalculateWallSize(int startOffsetX, int startOffsetY, int endOffsetX, int endOffsetY)
    {
        _startWallPosition = new Vector3Int(_currentPosition.x + startOffsetX, _currentPosition.y + startOffsetY, 0);
        _endWallPosition = new Vector3Int(_currentPosition.x + endOffsetX, _currentPosition.y + endOffsetY, 0);
    }

    private void ClearWalls()
    {
        for (int x = -300; x <= 300; x++)
        {
            for (int y = -300; y <= 300; y++)
            {
                if(_groundTilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    _wallsTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    private HashSet<Vector3Int> DeleteRepeatPositions(HashSet<Vector3Int> buffer)
    {
        HashSet<Vector3Int> clearPositions = new HashSet<Vector3Int>();

        foreach(Vector3Int position in buffer)
        {
            int repeatCount = 0;

            foreach(Vector3Int otherPosition in buffer)
            {
                if (position == otherPosition)
                {
                    repeatCount++;
                }
            }

            if (repeatCount <= 1)
            {
                clearPositions.Add(position);
            }
        }

        return clearPositions;
    }
}