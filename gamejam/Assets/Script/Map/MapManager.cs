using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    int roomSize = 5; // n���� �Է¹��� ����
    [SerializeField]
    int roomGap;    //�� ���� �Ÿ�
    [SerializeField]
    GameObject roomPrefab; // ���� ������ ��������Ʈ ������
    [SerializeField]
    GameObject corridorPrefab; // ������ ������ ��������Ʈ ������
    [SerializeField]
    GameObject wallTile;    //�� Ÿ��
    [SerializeField]
    GameObject tilemap;

    private List<GameObject> rooms = new List<GameObject>();
    private TreeNode roomTree;
    private List<List<GameObject>> roomLevel = new List<List<GameObject>>();
    private List<Vector3> spawnPoints = new List<Vector3>();
    enum direction { UP, DOWN, LEFT, RIGHT};

    void Start()
    {
        GenerateRooms();
        ConnectRooms();
    }

    void GenerateRooms()
    {
        Vector3 roomPosition;
        GameObject room;

        for (int i = 0; i < roomSize / 2 +1; i++)
        {
            if (i == 0)
            {
                List<GameObject> level = new List<GameObject>();
                roomPosition = new Vector3(0, 0, 0); // ���� ��ġ ����)
                room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
                room.transform.parent = tilemap.transform;
                level.Add(room);
            }
            else
            {
                List<GameObject> level = new List<GameObject>();
                for (int y = -i; y <= i; y++)
                {
                    for(int x=-i; x <= i; x++)
                    {
                        roomPosition = new Vector3(x * roomGap, -y * roomGap, 0); // ���� ��ġ ����)
                        room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
                        room.transform.parent = tilemap.transform;
                        level.Add(room);
                        if (i==roomSize / 2)    //������ level
                        {
                            if ((x == -i && y == i) || (x == i && y == i) || (x == -i && y == -i) || (x == i && y == -i))    //�� �𼭸�
                                spawnPoints.Add(roomPosition);
                        }
                    }
                }
                roomLevel.Add(level);
            }
        }
        foreach (Vector3 i in spawnPoints)
        {
            Debug.Log(i);
        }
       
    }
    void ConnectRooms()
    {
        foreach (List<GameObject> level in roomLevel)
        {
            for (int i = 0; i < level.Count; i++)
            {
                // ���� ��� ������ ������ ������ ����
                direction randomDirection = (direction)UnityEngine.Random.Range(0, 4);

                int nextRoomIndex = -1;

                switch (randomDirection)
                {
                    case direction.UP:
                        nextRoomIndex = i + roomSize / 2 + 1;
                        break;
                    case direction.DOWN:
                        nextRoomIndex = i - roomSize / 2 - 1;
                        break;
                    case direction.LEFT:
                        nextRoomIndex = i - 1;
                        break;
                    case direction.RIGHT:
                        nextRoomIndex = i + 1;
                        break;
                }

                if (nextRoomIndex >= 0 && nextRoomIndex < level.Count)
                {
                    // ������ �����ϰ� ��� �� ���̿� ��ġ
                    Vector3 startPoint = level[i].transform.position;
                    Vector3 endPoint = level[nextRoomIndex].transform.position;
                    CreateCorridor(startPoint, endPoint);
                }
            }
        }
    }

    void CreateCorridor(Vector3 start, Vector3 end)
    {
        // ���� �������� �����ؼ� ����
        GameObject corridor = Instantiate(corridorPrefab, (start + end) / 2, Quaternion.identity);
        corridor.transform.parent = tilemap.transform;

        // ������ �������� �����Ͽ� ���۰� ���� ����
        float corridorLength = Vector3.Distance(start, end);
        Vector3 scale = new Vector3(corridorLength, 1f, 1f);
        corridor.transform.localScale = scale;

        // ������ ȸ���� �����Ͽ� ��� ���� ���� (�밢�� ������ ����)
        Vector3 directionVector = end - start;

        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            // ���η� �� ����
            corridor.transform.rotation = Quaternion.Euler(0, 0, directionVector.x < 0 ? 0 : 180);
        }
        else
        {
            // ���η� �� ����
            corridor.transform.rotation = Quaternion.Euler(0, 0, directionVector.y < 0 ? 90 : -90);
        }
    }






}
