using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class HollwayGenerator : MonoBehaviour
{
    public HashSet<Vector3Int> _crossroadPosition = new HashSet<Vector3Int>();

    [Header("Links")]
    [SerializeField] private Tilemap _ground;
    [SerializeField] private Tilemap _walls;
    [SerializeField] private TileBase _room;
    [SerializeField] private GameObject _wallTilesObj;
    [SerializeField] private GameObject _groundTilesObj;

    private Grid _grid;

    [Header("Tiles")]
    [SerializeField] private Tile _groundTiles;
    [SerializeField] private Tile[] _wallTiles;

    [Header("Settings")]
    [SerializeField] private int _hollwayCount;
    [SerializeField] private int _hollwayLenght = 10;
    [SerializeField] private int _spawnHollOffset = 2;

    private Vector3Int setHollPosition;
    private Vector3Int endHollPosition = new Vector3Int(0, 0, 0);
    private Vector3Int lastHollwayDirection;

    private void Start()
    {
        _grid = GetComponent<Grid>();

        GenerateRoom();

        _groundTilesObj.SetActive(false);
        _groundTilesObj.SetActive(true);
        _wallTilesObj.SetActive(false);
        _wallTilesObj.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void GenerateRoom()
    {
        for (int i = 0; i < _hollwayCount; i++)
        {
            setHollPosition = endHollPosition;

            Vector3Int hollwayDirection;

            if (i == 0)
            {
                hollwayDirection = new Vector3Int(0, 1, 0);
                lastHollwayDirection = hollwayDirection;
                StartCorridor(setHollPosition, hollwayDirection);
            }
            else
            {
                hollwayDirection = SetDirection();
            }

            for (int j = 0; j <= _hollwayLenght; j++)
            {
                setHollPosition = (j == 0) ? (setHollPosition - hollwayDirection * _spawnHollOffset) : setHollPosition; // при старте постройки корридора, корридор смещается на 2 координаты по направлению

                _ground.SetTile(setHollPosition += hollwayDirection, _groundTiles); // ось ценра коридора
                
                for (int k = -2; k <= 2; k++) // выставление ширины корридору относительно направления и постройка стен.
                {
                    if (k != -2 && k != 2)
                    {
                        _ground.SetTile(setHollPosition + new Vector3Int(k * hollwayDirection.y, k * hollwayDirection.x, 0), _groundTiles);
                    }

                    if (k == -1 || k == 1) // основные стены параллельные по направлению коридора
                    {
                        Vector2Int wallIndex = GetIndexFromDirection(hollwayDirection); // x - left, y - right

                        if (k == -1)
                            _walls.SetTile(setHollPosition + new Vector3Int(k * hollwayDirection.y * 2, k * hollwayDirection.x * 2, 0), _wallTiles[wallIndex.x]);
                        
                        if (k == 1)
                            _walls.SetTile(setHollPosition + new Vector3Int(k * hollwayDirection.y * 2, k * hollwayDirection.x * 2, 0), _wallTiles[wallIndex.y]);
                    }
                    
                    if (j == _hollwayLenght) // конечные стены перпендикулярные по направлению коридора
                    {
                        Vector2Int wallIndex = GetIndexFromDirection(hollwayDirection, "end");

                        Vector3Int endWallPosition = setHollPosition + hollwayDirection;
                        
                        _walls.SetTile(endWallPosition + new Vector3Int(k * hollwayDirection.y, k * hollwayDirection.x, 0), _wallTiles[wallIndex.x]);
                    }
                }
                
                if (j == _hollwayLenght)
                {
                    endHollPosition = setHollPosition - new Vector3Int(1, 1, 0) * hollwayDirection;

                    _crossroadPosition.Add(endHollPosition);
                }

                lastHollwayDirection = hollwayDirection;
            }
        }

        ClearCrossroadWalls();
    }

    private Vector3Int SetDirection()
    {
        int directionIndex = Random.Range(0, 5);

        Vector3Int direction = new Vector3Int(0, 0, 0);

        switch (directionIndex) 
        {
            case 1:
                direction = new Vector3Int(-1, 0, 0); // left
                break;

            case 2:
                direction = new Vector3Int(0, 1, 0); // top
                break;

            case 3:
                direction = new Vector3Int(1, 0, 0); // right
                break;

            case 4:
                direction = new Vector3Int(0, -1, 0); // bottom
                break;

            default:
                direction = new Vector3Int(0, 1, 0);
                break;
        }

        if (direction == lastHollwayDirection)
            direction = new Vector3Int(direction.y, direction.x, 0);

        return direction;
    }

    private void ClearCrossroadWalls() // and set corners
    {
        foreach (Vector3Int position in _crossroadPosition)
        {
            bool leftHoll = false, topHoll = false, rightHoll = false, bottomHoll = false;

            if (_ground.GetTile(position + new Vector3Int(-5, 0, 0))) // left ?
            {
                DeleteWall(position, -2, false);
                leftHoll = true;
            }

            if (_ground.GetTile(position + new Vector3Int(0, 5, 0))) // top ?
            {
                DeleteWall(position, 2, true);
                topHoll = true;
            }

            if (_ground.GetTile(position + new Vector3Int(5, 0, 0))) // right ?
            {
                DeleteWall(position, 2, false);
                rightHoll = true;
            }

            if (_ground.GetTile(position + new Vector3Int(0, -5, 0))) // bottom ?
            {
                DeleteWall(position, -2, true);
                bottomHoll = true;
            }

            // shit...
            CheckHollways(position, leftHoll, topHoll, rightHoll, bottomHoll);
        }
    }
    //              /\              //
    private void DeleteWall(Vector3Int position, int offset, bool reverse)
    {
        for (int i = -1; i <= 1; i++)
        {
            if (reverse)
            {
                _walls.SetTile(position + new Vector3Int(i, offset, 0), null);
            }
            else
            {
                _walls.SetTile(position + new Vector3Int(offset, i, 0), null);
            }
        }
    }
    //              /\              //
    private void CheckHollways(Vector3Int position, bool left, bool top, bool right, bool bottom)
    {
        if (left && top && right && bottom)
        {
            SetCorners(position, 6, 7, 9, 8);
        }
        else if (left && top && right)
        {
            SetCorners(position, 6, 7, 3, 3);
        }
        else if (top && right && bottom)
        {
            SetCorners(position, 0, 7, 9, 0);
        }
        else if (right && bottom && left)
        {
            SetCorners(position, 1, 1, 9, 8);
        }
        else if (bottom && left && top)
        {
            SetCorners(position, 6, 2, 2, 8);
        }
        else if (left && top)
        {
            SetCorners(position, 6, 2, 11, 3);
        }
        else if (top && right)
        {
            SetCorners(position, 0, 7, 3, 10);
        }
        else if (right && bottom)
        {
            SetCorners(position, 4, 1, 9, 0);
        }
        else if (bottom && left)
        {
            SetCorners(position, 1, 5, 2, 8);
        }
        else if (left && right)
        {
            SetCorners(position, 1, 1, 3, 3);
        }
        else if (top && bottom)
        {
            SetCorners(position, 0, 2, 2, 0);
        }
        else if (left)
        {
            SetCorners(position, 1, 5, 11, 3);
        }
        else if (top)
        {
            SetCorners(position, 0, 2, 11, 10);
        }
        else if (right)
        {
            SetCorners(position, 4, 1, 3, 10);
        }
        else if (bottom)
        {
            SetCorners(position, 4, 5, 2, 0);
        }
    }
    //              /\              //
    private void SetCorners(Vector3Int position, int topLeft, int topRight, int bottomRight, int bottomLeft)
    {
        _walls.SetTile(new Vector3Int(position.x - 2, position.y + 2, 0), _wallTiles[topLeft]);
        _walls.SetTile(new Vector3Int(position.x + 2, position.y + 2, 0), _wallTiles[topRight]);
        _walls.SetTile(new Vector3Int(position.x + 2, position.y - 2, 0), _wallTiles[bottomRight]);
        _walls.SetTile(new Vector3Int(position.x - 2, position.y - 2, 0), _wallTiles[bottomLeft]);
    }

    private Vector2Int GetIndexFromDirection(Vector3Int direction, string wallType = "line") // none "line" = end
    {
        Vector2Int wallIndex = new Vector2Int(0, 0);

        if (direction.x == 0 && direction.y > 0) // top
        {
            if (wallType == "line")
                wallIndex = new Vector2Int(0, 2);
            else
                wallIndex = new Vector2Int(1, 1);
        }
        else if (direction.x == 0 && direction.y < 0) // bottom
        {
            if (wallType == "line")
                wallIndex = new Vector2Int(2, 0);
            else
                wallIndex = new Vector2Int(3, 3);
        }
        else if (direction.x > 0 && direction.y == 0) // right
        {
            if (wallType == "line")
                wallIndex = new Vector2Int(3, 1);
            else
                wallIndex = new Vector2Int(2, 2);
        }
        else if (direction.x < 0 && direction.y == 0) // left
        {
            if (wallType == "line")
                wallIndex = new Vector2Int(1, 3);
            else
                wallIndex = new Vector2Int(0, 0);
        }
        
        return wallIndex;
    }

    private void StartCorridor(Vector3Int startPosition, Vector3Int direction)
    {
        int offset = -2;

        for (int i = -2; i <= 2; i++)
        {
            _walls.SetTile(startPosition + new Vector3Int(i, offset, 0), _wallTiles[3]);
        }
    }
}