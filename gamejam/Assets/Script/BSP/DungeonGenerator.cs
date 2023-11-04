using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// ���� ���� Ŭ����
/// ���� �����, ó�� ������ �����ϰ�, ������ �������ִ� Ŭ����
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Size")]
    [SerializeField] int _mapSize;

    [Header("Settings")]
    [SerializeField, Range(1, 6)]
    private int _maxIterations = 4;

    [SerializeField, Range(0.3f, 0.7f)]
    private float _divideRatio = 0.45f;

    [SerializeField] int _minRoomPadding = 1;
    [SerializeField] int _maxRoomPadding = 3;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _revisitNodeRatio = 0.25f;    //��湮 ������ ����� ����(��湮 ������ ������ �� ��湮 ���� ����)

    [SerializeField, Range(0.0f, 1.0f)]
    private float _revisitPercentage = 0.3f;    //��湮 Ȯ��

    [Header("Assets")]
    [SerializeField] GameObject _roomPrefab;
    [SerializeField] GameObject _backfillPrefab;

    private float _minDivideRatio;
    private float _maxDivideRatio;
    private List<RoomNode> _leafNodes = new List<RoomNode>();    //������� = ���� ������ ����

    private readonly int _DOOR_LENGTH = 6;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        RectInt dungeonSize = new RectInt(0, 0, _mapSize, _mapSize);
        _minDivideRatio = Mathf.Min(_divideRatio, 1 - _divideRatio);
        _maxDivideRatio = 1 - _minDivideRatio;

        //BSP�� �̿��� ���� ����
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonSize, _minDivideRatio, _maxDivideRatio);
        RoomNode treeNode = bsp.CreateRoomTree(_maxIterations);

        InitRoomInfo(treeNode);

        //������� Ʈ���� �̿��� �� ����
        GenerateRooms();
    }

    private void InitRoomInfo(RoomNode treeNode)
    {
        TraverseNode(treeNode);

        for (int i = 0; i < _leafNodes.Count; i++)
        {
            _leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);
            _leafNodes[i].RoomName = string.Format("{0} Node", i);
        }

        for (int i = 0; i < _leafNodes.Count; i++)
        {
            FindAndAddNeighborNode(_leafNodes[i]);
        }

        MakePathInfo();
    }

    private void FindAndAddNeighborNode(RoomNode curNode)
    {
        var nodesByDirection = new List<Tuple<RoomNode, float>>[4];      //�����¿�
        for (int i = 0; i < 4; i++)
        {
            nodesByDirection[i] = new List<Tuple<RoomNode, float>>();
        }

        RectInt from = curNode.RoomSize;

        foreach (RoomNode node in _leafNodes)
        {
            if (curNode == node)
            {
                continue;
            }

            RectInt to = node.RoomSize;
            eRelativeRectDirection relativeDirection = to.DistinguishRectPosition(from);
            switch (relativeDirection)
            {
                case eRelativeRectDirection.LEFT:
                    nodesByDirection[0].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.xMax - from.xMin)));
                    break;
                case eRelativeRectDirection.RIGHT:
                    nodesByDirection[1].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.xMin - from.xMax)));
                    break;
                case eRelativeRectDirection.DOWN:
                    nodesByDirection[2].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.yMax - from.yMin)));
                    break;
                case eRelativeRectDirection.UP:
                    nodesByDirection[3].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.yMin - from.yMax)));
                    break;
                case eRelativeRectDirection.NONE:
                default:
                    break;
            }
        }

        foreach (List<Tuple<RoomNode, float>> neighborNodes in nodesByDirection)
        {
            neighborNodes.Sort((lhs, rhs) =>
            {
                return lhs.Item2.CompareTo(rhs.Item2);
            });

            if (0 != neighborNodes.Count && CanMakeDoor(from, neighborNodes[0].Item1.RoomSize, _DOOR_LENGTH))
            {
                curNode.NeighborNodes.Add(neighborNodes[0].Item1);
            }
        }
    }

    private bool CanMakeDoor(RectInt from, RectInt to, int doorLength)
    {
        int doorXMin = Mathf.Max(from.xMin, to.xMin);
        int doorXMax = Mathf.Min(from.xMax, to.xMax);
        int doorYMin = Mathf.Max(from.yMin, to.yMin);
        int doorYMax = Mathf.Min(from.yMax, to.yMax);

        bool canMakeDoor = false;

        eRelativeRectDirection relative = from.DistinguishRectPosition(to);
        if (eRelativeRectDirection.LEFT == relative || eRelativeRectDirection.RIGHT == relative)
        {
            canMakeDoor = Mathf.Abs(doorYMax - doorYMin) >= doorLength;
        }
        else if (eRelativeRectDirection.UP == relative || eRelativeRectDirection.DOWN == relative)
        {
            canMakeDoor = Mathf.Abs(doorXMax - doorXMin) >= doorLength;
        }

        return canMakeDoor;
    }

    private void MakePathInfo()
    {
        Queue<RoomNode> traverseQueue = new Queue<RoomNode>();
        int randomStartIndex = UnityEngine.Random.Range(0, _leafNodes.Count - 1);
        traverseQueue.Enqueue(_leafNodes[randomStartIndex]);
        int revisit = 0;

        while (0 != traverseQueue.Count)
        {
            RoomNode curNode = traverseQueue.Dequeue();

            foreach (RoomNode neighborNode in curNode.NeighborNodes)
            {
                if (!neighborNode.Visited)
                {
                    traverseQueue.Enqueue(neighborNode);
                    curNode.MakePathInfo(neighborNode);
                    neighborNode.Visited = true;
                }
                else
                {
                    if (((float)revisit / _leafNodes.Count < _revisitNodeRatio) && UnityEngine.Random.value > 1 - _revisitPercentage)
                    {
                        curNode.MakePathInfo(neighborNode);
                        revisit++;
                    }
                }
            }
        }
    }

    private void GenerateRooms()
    {
        foreach (RoomNode roomNode in _leafNodes)
        {
            GameObject instance = Instantiate(_roomPrefab);
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            Room room = instance.GetComponent<Room>();
            room.CreateRoom(roomNode);
        }
    }

    private void TraverseNode(RoomNode node)
    {
        if (null == node)
            return;

        TraverseNode(node.Left);

        if (node.IsLeaf)
        {
            _leafNodes.Add(node);
        }

        TraverseNode(node.Right);
    }
}