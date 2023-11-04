using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    int roomSize = 5; // n값을 입력받을 변수
    [SerializeField]
    GameObject roomPrefab; // 방을 생성할 스프라이트 프리팹
    [SerializeField]
    GameObject corridorPrefab; // 복도를 생성할 스프라이트 프리팹

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
                // 방을 생성하고 위치를 조정하는 코드를 작성
                Vector3 roomPosition = new Vector3(x * 2, y * 2, 0); // 방의 위치 조정 (2는 방 사이의 간격)
                GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
                rooms.Add(room);
            }
        }
    }

    void ConnectRooms()
    {
        // 무작위로 방을 연결하는 코드를 작성
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
