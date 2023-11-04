using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eRelativeRectDirection
{
    NONE = 0,
    LEFT = 1,
    RIGHT,
    DOWN,
    UP,
}

public static class StructureHelper
{
    public static bool CheckOverlapRange(this RectInt from, RectInt to)
    {
        //연결가능 유효성 체크. 너비, 높이 둘 중 하나만 참이면 됨
        int xMin = Mathf.Min(from.xMin, to.xMin);
        int xMax = Mathf.Max(from.xMax, to.xMax);
        int yMin = Mathf.Min(from.yMin, to.yMin);
        int yMax = Mathf.Max(from.yMax, to.yMax);

        bool isOverlapped = (xMax - xMin < from.width + to.width) || (yMax - yMin < from.height + to.height);
        return isOverlapped;
    }

    /// <summary>
    /// <para>target과 비교해 orig이 어느 위치에 있는지 반환</para>
    /// <para>상하좌우만 비교하므로 겹치는 길이(수평 or 수직)가 없는 경우는 제외</para>
    /// </summary>
    public static eRelativeRectDirection DistinguishRectPosition(this RectInt orig, RectInt compareTarget)
    {
        if (orig.CheckOverlapRange(compareTarget))
        {
            int xMin = Mathf.Min(orig.xMin, compareTarget.xMin);
            int xMax = Mathf.Max(orig.xMax, compareTarget.xMax);
            int yMin = Mathf.Min(orig.yMin, compareTarget.yMin);
            int yMax = Mathf.Max(orig.yMax, compareTarget.yMax);

            if ((xMax - xMin < orig.width + compareTarget.width))
            {
                //위, 아래 판별
                if (orig.yMax < compareTarget.yMin)
                {
                    return eRelativeRectDirection.DOWN;
                }
                else if (orig.yMin > compareTarget.yMax)
                {
                    return eRelativeRectDirection.UP;
                }
            }
            else if (yMax - yMin < orig.height + compareTarget.height)
            {
                //좌, 우 판별
                if (orig.xMax < compareTarget.xMin)
                {
                    return eRelativeRectDirection.LEFT;
                }
                else if (orig.xMin > compareTarget.xMax)
                {
                    return eRelativeRectDirection.RIGHT;
                }
            }
        }

        return eRelativeRectDirection.NONE;
    }
}