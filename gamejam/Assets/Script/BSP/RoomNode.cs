using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfo
{
    public Vector3 _doorPosition;
    public Quaternion _doorRotation;
    public string _name;
    public int _corridorLength;
    public bool _hasCorridor;       //from -> to 에서 from 문에만 복도를 그린다.
    public eRelativeRectDirection _doorDirection;
}

public class RoomNode
{
    private RoomNode _left;
    private RoomNode _right;

    private RectInt _spaceSize;
    private RectInt _roomSize;

    private int _layerIndex;

    private List<RoomNode> _neighborNodes = new List<RoomNode>();
    private List<RoomNode> _connectedNodes = new List<RoomNode>();
    private List<DoorInfo> _doorInfos = new List<DoorInfo>();

    public RoomNode Left
    {
        get { return _left; }
        set { _left = value; }
    }
    public RoomNode Right
    {
        get { return _right; }
        set { _right = value; }
    }

    public int SpaceWidth => _spaceSize.width;
    public int SpaceHeight => _spaceSize.height;
    public Vector2 SpaceCenter => _spaceSize.center;
    public Vector2Int BottomLeftAnchor => new Vector2Int(_spaceSize.xMin, _spaceSize.yMin);
    public Vector2Int TopRightAnchor => new Vector2Int(_spaceSize.xMax, _spaceSize.yMax);
    public RectInt RoomSize => _roomSize;

    public bool IsLeaf => (null == _left) && (null == _right);
    public bool HasLeaf { get; set; }
    public bool Visited { get; set; }
    public string RoomName { get; set; }
    public List<RoomNode> NeighborNodes => _neighborNodes;
    public List<RoomNode> ConnectedNodes => _connectedNodes;

    public List<DoorInfo> DoorInfos => _doorInfos;

    public RoomNode(RectInt size)
    {
        _spaceSize = size;
    }

    public void InitRoomSizeBySpace(int minPadding, int maxPadding)
    {
        int deltaX = UnityEngine.Random.Range(minPadding, Mathf.FloorToInt(_spaceSize.width / (float)maxPadding));
        int deltaY = UnityEngine.Random.Range(minPadding, Mathf.FloorToInt(_spaceSize.height / (float)maxPadding));
        int x = _spaceSize.x + deltaX;
        int y = _spaceSize.y + deltaY;
        int w = _spaceSize.width - deltaX;      //이동한 좌표만큼 빼줘서 길이를 맞춰준다.
        int h = _spaceSize.height - deltaY;

        w -= UnityEngine.Random.Range(minPadding, w / maxPadding);
        h -= UnityEngine.Random.Range(minPadding, h / maxPadding);

        //홀,짝수 길이 보정
        int roomWidth = (w % 2 == 0) ? w - 1 : w;
        int roomHeight = (h % 2 == 0) ? h - 1 : h;

        _roomSize = new RectInt(x, y, roomWidth, roomHeight);
    }

    public void MakePathInfo(RoomNode toNode)
    {
        for (int i = 0; i < _connectedNodes.Count; i++)
        {
            //이미 연결되어 있음
            if (_connectedNodes[i] == toNode)
                return;
        }

        DoorInfo fromDoor = new DoorInfo();
        DoorInfo toDoor = new DoorInfo();

        RectInt from = _roomSize;
        RectInt to = toNode.RoomSize;

        int doorXMin = Mathf.Max(from.xMin, to.xMin);
        int doorXMax = Mathf.Min(from.xMax, to.xMax);
        int doorYMin = Mathf.Max(from.yMin, to.yMin);
        int doorYMax = Mathf.Min(from.yMax, to.yMax);

        eRelativeRectDirection relativeDirection = to.DistinguishRectPosition(from);
        if (eRelativeRectDirection.LEFT == relativeDirection)
        {
            //to가 왼쪽 방에 있을 때
            int yPos = UnityEngine.Random.Range(doorYMin + 3, doorYMax - 3);
            fromDoor._doorPosition = new Vector3(from.xMin, 0, yPos);
            toDoor._doorPosition = new Vector3(to.xMax, 0, yPos);

            fromDoor._doorRotation = Quaternion.Euler(0f, 90f, 0f);
            toDoor._doorRotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (eRelativeRectDirection.RIGHT == relativeDirection)
        {
            //to가 오른쪽 방
            int yPos = UnityEngine.Random.Range(doorYMin + 3, doorYMax - 3);
            fromDoor._doorPosition = new Vector3(from.xMax, 0, yPos);
            toDoor._doorPosition = new Vector3(to.xMin, 0, yPos);

            fromDoor._doorRotation = Quaternion.Euler(0f, -90f, 0f);
            toDoor._doorRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (eRelativeRectDirection.DOWN == relativeDirection)
        {
            //to가 아래쪽
            int xPos = UnityEngine.Random.Range(doorXMin + 3, doorXMax - 3);
            fromDoor._doorPosition = new Vector3(xPos, 0, from.yMin);
            toDoor._doorPosition = new Vector3(xPos, 0, to.yMax);

            fromDoor._doorRotation = Quaternion.identity;
            toDoor._doorRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (eRelativeRectDirection.UP == relativeDirection)
        {
            //위
            int xPos = UnityEngine.Random.Range(doorXMin + 3, doorXMax - 3);
            fromDoor._doorPosition = new Vector3(xPos, 0, from.yMax);
            toDoor._doorPosition = new Vector3(xPos, 0, to.yMin);

            fromDoor._doorRotation = Quaternion.Euler(0f, 180f, 0f);
            toDoor._doorRotation = Quaternion.identity;
        }

        _connectedNodes.Add(toNode);
        toNode.ConnectedNodes.Add(this);

        fromDoor._name = string.Format("{0}->{1}", RoomName, toNode.RoomName);
        fromDoor._corridorLength = (int)Vector3.Distance(fromDoor._doorPosition, toDoor._doorPosition) - 1;
        fromDoor._hasCorridor = true;
        fromDoor._doorDirection = relativeDirection;

        toDoor._name = string.Format("{0}->{1}", toNode.RoomName, RoomName);
        _doorInfos.Add(fromDoor);
        toNode.DoorInfos.Add(toDoor);
    }
}