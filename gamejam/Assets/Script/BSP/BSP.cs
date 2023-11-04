using UnityEngine;
using System.Collections.Generic;

public class BSPNode
{
    public Rect Bounds { get; set; }
    public List<BSPNode> Children { get; set; }

    public BSPNode(Rect bounds)
    {
        Bounds = bounds;
        Children = new List<BSPNode>();
    }

    public void Split(int depth)
    {
        if (depth <= 0)
            return;

        // Split the node into two child nodes
        bool splitHorizontally = RandomSplit();
        int splitPoint = splitHorizontally
            ? UnityEngine.Random.Range((int)Bounds.x, (int)(Bounds.x + Bounds.width))
            : UnityEngine.Random.Range((int)Bounds.y, (int)(Bounds.y + Bounds.height));

        Rect leftChildBounds = new Rect(Bounds.x, Bounds.y, splitHorizontally ? splitPoint - Bounds.x : Bounds.width, splitHorizontally ? Bounds.height : splitPoint - Bounds.y);
        Rect rightChildBounds = new Rect(splitHorizontally ? splitPoint : Bounds.x, splitHorizontally ? Bounds.y : splitPoint, splitHorizontally ? Bounds.width - (splitPoint - Bounds.x) : Bounds.width, splitHorizontally ? Bounds.height : Bounds.height - (splitPoint - Bounds.y));

        Children.Add(new BSPNode(leftChildBounds));
        Children.Add(new BSPNode(rightChildBounds));

        // Recursively split child nodes
        foreach (BSPNode child in Children)
        {
            child.Split(depth - 1);
        }
    }

    private bool RandomSplit()
    {
        return UnityEngine.Random.value > 0.5f;
    }
}

public struct Rect
{
    public float x, y, width, height;

    public Rect(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}

public class BSP : MonoBehaviour
{

    [SerializeField]

    public BSPNode RootNode { get; private set; }

    public void Start()
    {
        Rect bounds = new Rect(0, 0, 100, 100); // Set your desired bounds here
        int depth = 3; // Set the desired depth of the BSP tree

        RootNode = new BSPNode(bounds);
        RootNode.Split(depth);
    }
}
