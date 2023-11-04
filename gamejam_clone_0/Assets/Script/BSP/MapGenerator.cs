using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //������ �������� �ּ� ����
    [SerializeField] float maximumDivideRate; //������ �������� �ִ� ����
    [SerializeField] int roomPadding;
    [SerializeField] private GameObject line; //lineRenderer�� ����ؼ� ������ �������� ���������� �����ֱ� ����
    [SerializeField] private GameObject map; //lineRenderer�� ����ؼ� ù ���� ����� �����ֱ� ����
    [SerializeField] private GameObject roomLine; //lineRenderer�� ����ؼ� ���� ����� �����ֱ� ����
    [SerializeField] private int maximumDepth; //Ʈ���� ����, ���� ���� ���� �� �ڼ��� ������ ��
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tilemap wallMap;
    
    [SerializeField] Tile roomTile; //���� �����ϴ� Ÿ��
    [SerializeField] Tile roomTile_up;
    [SerializeField] Tile roomTile_down; 
    [SerializeField] Tile roomTile_left;
    [SerializeField] Tile roomTile_right; 
    [SerializeField] Tile roomTile_leftdownConner; 
    [SerializeField] Tile roomTile_leftupConner; 
    [SerializeField] Tile roomTile_rightdownConner;
    [SerializeField] Tile roomTile_rightupConner; 
    [SerializeField] Tile wallTile; //��� �ܺθ� ���������� �� Ÿ��
    [SerializeField] Tile wallTile_left;
    [SerializeField] Tile wallTile_right;
    [SerializeField] Tile corridor_x_up; //������ ���� Ÿ�� ��
    [SerializeField] Tile corridor_x_down; //������ ���� Ÿ�� ��
    [SerializeField] Tile corridor_y_left; //������ ���� Ÿ�� ��
    [SerializeField] Tile corridor_y_right; //������ ���� Ÿ�� ��
    [SerializeField] Tile outTile; //�� �ܺ��� Ÿ��

    void Start()
    {
        FillBackground();//�� �ε� �� ���δ� �ٱ�Ÿ�Ϸ� ����
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall();


    }

    void Divide(Node tree, int n)
    {
        if (n == maximumDepth) return; //���� ���ϴ� ���̿� �����ϸ� �� �������� �ʴ´�.
                                       //�� ���� ��쿡��

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //���ο� ������ �� ����� ������, ���ΰ� ��ٸ� �� ��, ��� ���ΰ� �� ��ٸ� ��, �Ʒ��� �����ְ� �� ���̴�.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //���� �� �ִ� �ִ� ���̿� �ּ� �����߿��� �������� �� ���� ����
        if (tree.nodeRect.width >= tree.nodeRect.height) //���ΰ� �� ����� ��쿡�� �� ��� ������ �� ���̸�, �� ��쿡�� ���� ���̴� ������ �ʴ´�.
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴ� �����̹Ƿ� ������ ������, ���� ���̴� ������ ���� �������� �־��ش�.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴܿ��� ���������� ���� ���̸�ŭ �̵��� ��ġ�̸�, ���� ���̴� ���� ���α��̿��� ���� ���� ���ΰ��� �� ������ �κ��� �ȴ�. 
        }
        else
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
        }
        tree.leftNode.parNode = tree; //�ڽĳ����� �θ��带 �������� ���� ����
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //����, ������ �ڽ� ���鵵 �����ش�.
        Divide(tree.rightNode, n + 1);//����, ������ �ڽ� ���鵵 �����ش�.
    }
    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) //�ش� ��尡 ��������� ���� ����� �� ���̴�.
        {
            rect = tree.nodeRect;
            int width = rect.width - roomPadding;
            int height = rect.height - roomPadding;
            int x = rect.x + roomPadding-1;
            int y = rect.y + roomPadding-1;
            rect = new RectInt(x, y, width, height);
            FillRoom(rect);
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }
    private void GenerateLoad(Node tree, int n)
    {
        if (n == maximumDepth) //���� ����� ���� �ڽ��� ����.
            return;
        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
        {
            if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2))!=roomTile)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y + 1 - mapSize.y / 2, 0), wallTile);
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0), corridor_x_up);
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - 1 - mapSize.y / 2, 0), corridor_x_down);
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - 2 - mapSize.y / 2, 0), wallTile);
            }
        }

        for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
        {
            if (tileMap.GetTile(new Vector3Int(j - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2)) != roomTile)
            {
                tileMap.SetTile(new Vector3Int(rightNodeCenter.x + 2 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                tileMap.SetTile(new Vector3Int(rightNodeCenter.x + 1 - mapSize.x / 2, j - mapSize.y / 2, 0), corridor_y_right);
                tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), corridor_y_left);
                tileMap.SetTile(new Vector3Int(rightNodeCenter.x - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
            }
        }
        GenerateLoad(tree.leftNode, n + 1); //�ڽ� ���鵵 Ž��
        GenerateLoad(tree.rightNode, n + 1);
    }

    void FillBackground() //����� ä��� �Լ�, �� load�� ���� ���� ���ش�.
    {
        for (int i = -10; i < mapSize.x + 10; i++) //�ٱ�Ÿ���� �� �����ڸ��� ���� ������� �ʰ�
        //�� ũ�⺸�� �а� ä���ش�.
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }
    void FillWall() //�� Ÿ�ϰ� �ٱ� Ÿ���� ������ �κ�
    {
        for (int i = 0; i < mapSize.x; i++) //Ÿ�� ��ü�� ��ȸ
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == wallTile)
                {
                    wallMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                }
                else if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == wallTile_left)
                {
                    wallMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                }
                else if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == wallTile_right)
                {
                    wallMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                }
            }
        }
    }
    private void FillRoom(RectInt rect)
    { //room�� rect������ �޾Ƽ� tile�� set���ִ� �Լ�
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                if (i == rect.x && j == rect.y)     //���ϴ�
                {
                    tileMap.SetTile(new Vector3Int(rect.x - 1 - mapSize.x / 2, rect.y - 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_leftdownConner);
                }
                else if (i == rect.x + rect.width - 1 && j == rect.y)      // ���ϴ�
                {
                    tileMap.SetTile(new Vector3Int(rect.x + rect.width - mapSize.x / 2, rect.y - 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i + 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_rightdownConner);
                }
                else if (i == rect.x && j == rect.y + rect.height - 1)      //�»��
                {
                    tileMap.SetTile(new Vector3Int(rect.x - 1 - mapSize.x / 2, rect.y + rect.height - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j + 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_leftupConner); 
                }
                else if (i == rect.x + rect.width - 1 && j == rect.y + rect.height - 1)     //����
                {
                    tileMap.SetTile(new Vector3Int(rect.x + rect.width - mapSize.x / 2, rect.y + rect.height - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i + 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j + 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_rightupConner); 
                }
                else if (i == rect.x)
                {
                    tileMap.SetTile(new Vector3Int(i - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_left);
                }
                else if (i == rect.x + rect.width - 1)
                {
                    tileMap.SetTile(new Vector3Int(i + 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_right);
                }
                else if (j == rect.y + rect.height - 1)
                {
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j + 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_up);
                }
                else if (j == rect.y)
                {
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_down);
                }
                else
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
            }
        }
        
       
        
        
    }

}