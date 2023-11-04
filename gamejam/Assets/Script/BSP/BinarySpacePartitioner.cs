using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum eSplitDirection
{
    NONE = 0,
    HORIZONTAL = 1,
    VERTICAL = 2,
}

public class BinarySpacePartitioner
{
    private RoomNode _rootNode;
    private int _maxIterations;

    private float _minDivideRatio;
    private float _maxDivideRatio;

    public BinarySpacePartitioner(RectInt size, float minDivideRatio, float maxDivideRatio)
    {
        _rootNode = new RoomNode(size);
        _minDivideRatio = minDivideRatio;
        _maxDivideRatio = maxDivideRatio;
    }

    public RoomNode CreateRoomTree(int maxIterations)
    {
        _maxIterations = maxIterations;
        DivideSpace(_rootNode, 0);

        return _rootNode;
    }

    private void DivideSpace(RoomNode node, int iterations)
    {
        if (iterations == _maxIterations)
            return;

        eSplitDirection splitDireciton = splitDireciton = (node.SpaceWidth > node.SpaceHeight) ? eSplitDirection.VERTICAL : eSplitDirection.HORIZONTAL;
        if (eSplitDirection.HORIZONTAL == splitDireciton)
        {
            //가로
            int dividedHeight = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceHeight * _minDivideRatio, node.SpaceHeight * _maxDivideRatio));

            RectInt topRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y + dividedHeight,
                node.SpaceWidth, node.SpaceHeight - dividedHeight
            );

            RectInt bottomRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y,
                node.SpaceWidth, dividedHeight
            );

            RoomNode topNode = new RoomNode(topRoomSize);
            RoomNode bottomNode = new RoomNode(bottomRoomSize);

            node.Left = bottomNode;
            node.Right = topNode;

        }
        else
        {
            //세로
            int dividedWidth = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceWidth * _minDivideRatio, node.SpaceWidth * _maxDivideRatio));

            RectInt leftRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y,
                dividedWidth, node.SpaceHeight
            );

            RectInt rightRoomSize = new RectInt(
                node.BottomLeftAnchor.x + dividedWidth, node.BottomLeftAnchor.y,
                node.SpaceWidth - dividedWidth, node.SpaceHeight
            );

            RoomNode leftNode = new RoomNode(leftRoomSize);
            RoomNode rightNode = new RoomNode(rightRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;

        }

        //문 생성을 위해 말단 노드만 가진 부모노드 표시
        if (iterations == _maxIterations - 1)
        {
            node.HasLeaf = true;
        }

        DivideSpace(node.Left, iterations + 1);
        DivideSpace(node.Right, iterations + 1);
    }
}