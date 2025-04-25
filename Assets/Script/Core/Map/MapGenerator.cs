using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Spawn, Normal, Elite, Shop, Rest, Boss }

public class MapData
{
    public Vector2Int gridPos;
    public Room roomInstance;
    public bool[] doors = new bool[4]; 
    public bool isCleared = false;
    public RoomType roomType;
}

public class MapGenerator : Singleton<MapGenerator>
{
    [SerializeField] private RoomPrefabType[] roomPrefabs;
    [SerializeField] private GameObject horizontalPath;
    [SerializeField] private GameObject verticalPath;

    private Dictionary<RoomType, GameObject> _prefabDict;
    private Dictionary<Vector2Int, MapData> _map = new();

    private void OnEnable()
    {
        _prefabDict = new();
        
        foreach (var r in roomPrefabs)
            _prefabDict[r.roomType] = r.roomPrefab;
    }

    public void GenerateMap()
    {
        _map.Clear();
        
        int maxRooms = 10;
        Stack<Vector2Int> stack = new();
        Vector2Int start = Vector2Int.zero;

        CreateRoom(start, RoomType.Spawn);
        stack.Push(start);

        for (int i = 0; i < maxRooms; i++)
        {
            if (stack.Count == 0) break;
            
            var current = stack.Peek();
            var next = GetRandomEmptyNeighbor(current);
            
            RoomType type = i == maxRooms - 1 ? RoomType.Boss : RandomRoomType();

            if (next != null)
            {
                CreateRoom(next.Value, type);
                stack.Push(next.Value);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    private void CreateRoom(Vector2Int pos, RoomType type)
    {
        GameObject prefab = _prefabDict[type];
        GameObject go = Instantiate(prefab, new Vector3(pos.x * 120, 0, pos.y * 120), Quaternion.identity);
        Room room = go.GetComponent<Room>();

        MapData data = new()
        {
            gridPos = pos,
            roomInstance = room,
            doors = new bool[4],
            roomType = type,
            isCleared = false
        };

        _map[pos] = data;
        ConnectToNeighbors(pos);
        room.Init(data);
    }

    private void ConnectToNeighbors(Vector2Int pos)
    {
        List<(Vector2Int, int, int, GameObject)> dirs = new()
        {
            (Vector2Int.up, 0, 1, verticalPath),
            (Vector2Int.down, 1, 0, verticalPath),
            (Vector2Int.right, 2, 3, horizontalPath),
            (Vector2Int.left, 3, 2, horizontalPath),
        };

        foreach (var (dir, fromIdx, toIdx, path) in dirs)
        {
            Vector2Int neighbor = pos + dir;
            if (_map.TryGetValue(neighbor, out var neighborData))
            {
                _map[pos].doors[fromIdx] = true;
                neighborData.doors[toIdx] = true;

                Vector3 from = new Vector3(pos.x * 120, 0, pos.y * 120);
                Vector3 to = new Vector3(neighbor.x * 120, 0, neighbor.y * 120);
                Vector3 center = (from + to) * 0.5f;

                Instantiate(path, center, Quaternion.identity);
            }
        }
    }

    private RoomType RandomRoomType()
    {
        RoomType[] pool = new RoomType[]
        {
            RoomType.Normal, RoomType.Normal, RoomType.Normal,
            RoomType.Elite, 
            RoomType.Shop, RoomType.Shop, 
            RoomType.Rest
        };
        return pool[Random.Range(0, pool.Length)];
    }

    private Vector2Int? GetRandomEmptyNeighbor(Vector2Int current)
    {
        List<Vector2Int> directions = new() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Shuffle(directions);

        foreach (var dir in directions)
        {
            Vector2Int next = current + dir;
            if (!_map.ContainsKey(next))
                return next;
        }
        return null;
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
