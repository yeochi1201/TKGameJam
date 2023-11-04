using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] int roomPadding;
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private GameObject roomLine; //lineRenderer를 사용해서 방의 사이즈를 보여주기 위함
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tilemap wallMap;
    
    [SerializeField] Tile roomTile; //방을 구성하는 타일
    [SerializeField] Tile roomTile_up;
    [SerializeField] Tile roomTile_down; 
    [SerializeField] Tile roomTile_left;
    [SerializeField] Tile roomTile_right; 
    [SerializeField] Tile roomTile_leftdownConner; 
    [SerializeField] Tile roomTile_leftupConner; 
    [SerializeField] Tile roomTile_rightdownConner;
    [SerializeField] Tile roomTile_rightupConner; 
    [SerializeField] Tile wallTile; //방과 외부를 구분지어줄 벽 타일
    [SerializeField] Tile wallTile_left;
    [SerializeField] Tile wallTile_right;
    [SerializeField] Tile corridor_x_up; //가로축 복도 타일 상
    [SerializeField] Tile corridor_x_down; //가로축 복도 타일 하
    [SerializeField] Tile corridor_y_left; //가로축 복도 타일 좌
    [SerializeField] Tile corridor_y_right; //가로축 복도 타일 우
    [SerializeField] Tile outTile; //방 외부의 타일

    void Start()
    {
        FillBackground();//신 로드 시 전부다 바깥타일로 덮음
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall();


    }

    void Divide(Node tree, int n)
    {
        if (n == maximumDepth) return; //내가 원하는 높이에 도달하면 더 나눠주지 않는다.
                                       //그 외의 경우에는

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //가로와 세로중 더 긴것을 구한후, 가로가 길다면 위 좌, 우로 세로가 더 길다면 위, 아래로 나눠주게 될 것이다.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        if (tree.nodeRect.width >= tree.nodeRect.height) //가로가 더 길었던 경우에는 좌 우로 나누게 될 것이며, 이 경우에는 세로 길이는 변하지 않는다.
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //왼쪽 노드에 대한 정보다 
            //위치는 좌측 하단 기준이므로 변하지 않으며, 가로 길이는 위에서 구한 랜덤값을 넣어준다.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //우측 노드에 대한 정보다 
            //위치는 좌측 하단에서 오른쪽으로 가로 길이만큼 이동한 위치이며, 가로 길이는 기존 가로길이에서 새로 구한 가로값을 뺀 나머지 부분이 된다. 
        }
        else
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
        }
        tree.leftNode.parNode = tree; //자식노드들의 부모노드를 나누기전 노드로 설정
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //왼쪽, 오른쪽 자식 노드들도 나눠준다.
        Divide(tree.rightNode, n + 1);//왼쪽, 오른쪽 자식 노드들도 나눠준다.
    }
    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) //해당 노드가 리프노드라면 방을 만들어 줄 것이다.
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
        if (n == maximumDepth) //리프 노드라면 이을 자식이 없다.
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
        GenerateLoad(tree.leftNode, n + 1); //자식 노드들도 탐색
        GenerateLoad(tree.rightNode, n + 1);
    }

    void FillBackground() //배경을 채우는 함수, 씬 load시 가장 먼저 해준다.
    {
        for (int i = -10; i < mapSize.x + 10; i++) //바깥타일은 맵 가장자리에 가도 어색하지 않게
        //맵 크기보다 넓게 채워준다.
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }
    void FillWall() //룸 타일과 바깥 타일이 만나는 부분
    {
        for (int i = 0; i < mapSize.x; i++) //타일 전체를 순회
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
    { //room의 rect정보를 받아서 tile을 set해주는 함수
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                if (i == rect.x && j == rect.y)     //좌하단
                {
                    tileMap.SetTile(new Vector3Int(rect.x - 1 - mapSize.x / 2, rect.y - 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_leftdownConner);
                }
                else if (i == rect.x + rect.width - 1 && j == rect.y)      // 우하단
                {
                    tileMap.SetTile(new Vector3Int(rect.x + rect.width - mapSize.x / 2, rect.y - 1 - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i + 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_right);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_rightdownConner);
                }
                else if (i == rect.x && j == rect.y + rect.height - 1)      //좌상단
                {
                    tileMap.SetTile(new Vector3Int(rect.x - 1 - mapSize.x / 2, rect.y + rect.height - mapSize.y / 2, 0), wallTile);
                    tileMap.SetTile(new Vector3Int(i - 1 - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile_left);
                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j + 1 - mapSize.y / 2, 0), wallTile);

                    tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile_leftupConner); 
                }
                else if (i == rect.x + rect.width - 1 && j == rect.y + rect.height - 1)     //우상단
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