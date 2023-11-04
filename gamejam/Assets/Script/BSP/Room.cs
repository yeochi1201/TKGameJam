using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _wallPrefab;
    [SerializeField] GameObject _doorwayPrefab;
    [SerializeField] GameObject _corridor1mPrefab;

    private RoomNode _roomNode;

    public void CreateRoom(RoomNode roomNode)
    {
        _roomNode = roomNode;
        gameObject.name = _roomNode.RoomName;
        transform.position = new Vector3(_roomNode.RoomSize.center.x, 0, _roomNode.RoomSize.center.y);

        CreateTiles();
        CreateEdges();
        CreatePath();
    }

    private void CreateTiles()
    {
        RectInt room = _roomNode.RoomSize;
        int tileSize = 2;
        float tilePivot = 0.5f;

        for (int y = room.yMin; y < room.yMax; y += tileSize)
        {
            for (int x = room.xMin; x < room.xMax; x += tileSize)
            {
                GameObject instance = Instantiate(_tilePrefab, transform);
                instance.transform.position = new Vector3(x + tilePivot, 0, y + tilePivot);
                instance.transform.localScale = Vector3.one;
            }
        }
    }

    private void CreateEdges()
    {
        int wallSize = 1;
        RectInt size = _roomNode.RoomSize;
        int width = size.width;
        int height = size.height;

        int xPos = 0;
        int zPos = 0;

        //Top edge
        for (int i = 0; i < width + 1; i++)
        {
            xPos = size.xMin + i * wallSize;
            zPos = size.yMax;

            if (!CheckDoorPosition(xPos, zPos, isHorizontal: true))
            {
                CreateWall(xPos, zPos, -180f);
            }
        }

        //Bottom edge
        for (int i = 0; i < width + 1; i++)
        {
            xPos = size.xMin + i * wallSize;
            zPos = size.yMin;

            if (!CheckDoorPosition(xPos, zPos, isHorizontal: true))
            {
                CreateWall(xPos, zPos, 0f);
            }
        }

        //Left side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMin;
            zPos = size.yMin + (i + 1) * wallSize;

            if (!CheckDoorPosition(xPos, zPos, isHorizontal: false))
            {
                CreateWall(xPos, zPos, 90f);
            }
        }

        //Right side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMax;
            zPos = size.yMin + (i + 1) * wallSize;

            if (!CheckDoorPosition(xPos, zPos, isHorizontal: false))
            {
                CreateWall(xPos, zPos, -90f);
            }
        }
    }

    private bool CheckDoorPosition(int wallXPos, int wallZPos, bool isHorizontal)
    {
        for (int i = 0; i < _roomNode.DoorInfos.Count; i++)
        {
            Vector3 doorPos = _roomNode.DoorInfos[i]._doorPosition;

            int doorXMin = Mathf.CeilToInt(doorPos.x - 3);
            int doorXMax = Mathf.FloorToInt(doorPos.x + 3);
            int doorZMin = Mathf.CeilToInt(doorPos.z - 3);
            int doorZMax = Mathf.FloorToInt(doorPos.z + 3);

            if (isHorizontal)
            {
                if ((doorXMin < wallXPos && wallXPos < doorXMax) && ((int)doorPos.z == wallZPos))
                {
                    return true;
                }
            }
            else
            {
                if ((doorZMin < wallZPos && wallZPos < doorZMax) && ((int)doorPos.x == wallXPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CreateWall(int xPos, int zPos, float angle)
    {
        GameObject instance = Instantiate(_wallPrefab, transform);
        instance.transform.position = new Vector3(xPos, 0, zPos);
        instance.transform.eulerAngles = new Vector3(0, angle, 0);
        instance.transform.localScale = Vector3.one;
    }

    private void CreatePath()
    {
        foreach (DoorInfo doorInfo in _roomNode.DoorInfos)
        {
            //복도 문 생성
            GameObject instance = Instantiate(_doorwayPrefab, transform);
            instance.gameObject.name = doorInfo._name;
            instance.transform.position = doorInfo._doorPosition;
            instance.transform.rotation = doorInfo._doorRotation;
            instance.transform.localScale = Vector3.one;

            if (doorInfo._hasCorridor)
            {
                Vector3 newPos = doorInfo._doorPosition;
                for (int i = 0; i < doorInfo._corridorLength; i++)
                {
                    GameObject corridorInstance = Instantiate(_corridor1mPrefab, transform);
                    switch (doorInfo._doorDirection)
                    {
                        case eRelativeRectDirection.LEFT:
                            newPos.x -= 1;
                            break;
                        case eRelativeRectDirection.RIGHT:
                            newPos.x += 1;
                            break;
                        case eRelativeRectDirection.DOWN:
                            newPos.z -= 1;
                            break;
                        case eRelativeRectDirection.UP:
                            newPos.z += 1;
                            break;
                        case eRelativeRectDirection.NONE:
                        default:
                            break;
                    }

                    corridorInstance.transform.position = newPos;
                    corridorInstance.transform.rotation = doorInfo._doorRotation;
                    corridorInstance.transform.localScale = Vector3.one;
                }
            }
        }
    }
}