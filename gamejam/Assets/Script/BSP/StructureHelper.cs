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
        //���ᰡ�� ��ȿ�� üũ. �ʺ�, ���� �� �� �ϳ��� ���̸� ��
        int xMin = Mathf.Min(from.xMin, to.xMin);
        int xMax = Mathf.Max(from.xMax, to.xMax);
        int yMin = Mathf.Min(from.yMin, to.yMin);
        int yMax = Mathf.Max(from.yMax, to.yMax);

        bool isOverlapped = (xMax - xMin < from.width + to.width) || (yMax - yMin < from.height + to.height);
        return isOverlapped;
    }

    /// <summary>
    /// <para>target�� ���� orig�� ��� ��ġ�� �ִ��� ��ȯ</para>
    /// <para>�����¿츸 ���ϹǷ� ��ġ�� ����(���� or ����)�� ���� ���� ����</para>
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
                //��, �Ʒ� �Ǻ�
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
                //��, �� �Ǻ�
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