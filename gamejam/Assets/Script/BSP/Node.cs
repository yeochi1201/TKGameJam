using UnityEngine;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; //�и��� ������ rect����
    public RectInt roomRect; //�и��� ���� �� ���� rect����
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
        //���� ��� ��. ��� ���� ���� �� ���
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}