using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    int roomSize = 5; // n���� �Է¹��� ����
    [SerializeField]
    GameObject roomPrefab; // ���� ������ ��������Ʈ ������
    [SerializeField]
    GameObject corridorPrefab; // ������ ������ ��������Ʈ ������

    private List<GameObject> rooms = new List<GameObject>();

    void Start()
    {
        GenerateRooms();
        ConnectRooms();
    }

    void GenerateRooms()
    {
        for (int x = 0; x < roomSize; x++)
        {
            for (int y = 0; y < roomSize; y++)
            {
                // ���� �����ϰ� ��ġ�� �����ϴ� �ڵ带 �ۼ�
                Vector3 roomPosition = new Vector3(x * 2, y * 2, 0); // ���� ��ġ ���� (2�� �� ������ ����)
                GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
                rooms.Add(room);
            }
        }
    }

    void ConnectRooms()
    {
        // �������� ���� �����ϴ� �ڵ带 �ۼ�
        for (int i = 0; i < rooms.Count; i++)
        {
            int randomIndex = Random.Range(0, rooms.Count);

            if (i != randomIndex)
            {
                Vector3 startPosition = rooms[i].transform.position;
                Vector3 endPosition = rooms[randomIndex].transform.position;
                Vector3 middlePosition = (startPosition + endPosition) / 2;

                GameObject corridor = Instantiate(corridorPrefab, middlePosition, Quaternion.identity);
            }
        }
    }
}
