using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Quad
{
    public int x, y;
    public int size;
    public Vector2 pivot = new Vector2(0, 0);
    public Tile tile;

    public Quad up, right, down, left;

    // vertices
    public Vector2 upLeft, upRight, downRight, downLeft;

    public Quad(int x, int y, int size)
    {
        this.x = x;
        this.y = y;
        this.size = size;
    }

    public void createVertices(bool upL, bool upR, bool downR, bool downL)
    {
        float halfSize = size / 2;

        if (upL)
            upLeft = new Vector2(pivot.x - halfSize, pivot.y + halfSize);
        if (upR)
            upRight = new Vector2(pivot.x + halfSize, pivot.y + halfSize);
        if (downR)
            downRight = new Vector2(pivot.x + halfSize, pivot.y - halfSize);
        if (downL)
            downLeft = new Vector2(pivot.x - halfSize, pivot.y - halfSize);
    }

    public void shiftQuad()
    {
        pivot = new Vector2(x * size, y * size);
    }

    public void shiftAgain(Vector2 shiftAmount)
    {
        pivot += shiftAmount;
    }

    public void show(GameObject room)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.transform.position = pivot;
        go.transform.localScale = Vector2.one * size;
        go.transform.parent = room.transform;

        QuadMono qm = go.AddComponent<QuadMono>();

        qm.x = x;
        qm.y = y;
    }

    public Quad addUpNeighbour(Quad newQ)
    {
        if (up == null)
        {
            up = newQ;
            newQ.down = this;
            return newQ;
        }

        newQ.up = up;
        up.down = newQ;
        newQ.down = this;
        up = newQ;

        Quad lastQuad = newQ;

        while (lastQuad.up != null)
        {
            lastQuad.right = lastQuad.up.right;
            lastQuad.left = lastQuad.up.left;
            if (lastQuad.right != null)
                lastQuad.right.left = lastQuad;
            if (lastQuad.left != null)
                lastQuad.left.right = lastQuad;

            lastQuad = lastQuad.up;
            lastQuad.right = null;
            lastQuad.left = null;
            lastQuad.y++;
        }

        return lastQuad;
    }

    public Quad addRightNeighbour(Quad newQ)
    {
        if (right == null)
        {
            right = newQ;
            newQ.left = this;
            return newQ;
        }

        newQ.right = right;
        right.left = newQ;
        newQ.left = this;
        right = newQ;

        Quad lastQuad = newQ;

        while (lastQuad.right != null)
        {
            lastQuad.up = lastQuad.right.up;
            lastQuad.down = lastQuad.right.down;
            if (lastQuad.up != null)
                lastQuad.up.down = lastQuad;
            if (lastQuad.down != null)
                lastQuad.down.up = lastQuad;

            lastQuad = lastQuad.right;
            lastQuad.down = null;
            lastQuad.up = null;
            lastQuad.x++;
        }

        return lastQuad;
    }

    public Quad addDownNeighbour(Quad newQ)
    {
        if (down == null)
        {
            down = newQ;
            newQ.up = this;
            return newQ;
        }

        newQ.down = down;
        down.up = newQ;
        newQ.up = this;
        down = newQ;

        Quad lastQuad = newQ;

        while (lastQuad.down != null)
        {
            lastQuad.right = lastQuad.down.right;
            lastQuad.left = lastQuad.down.left;
            if (lastQuad.right != null)
                lastQuad.right.left = lastQuad;
            if (lastQuad.left != null)
                lastQuad.left.right = lastQuad;

            lastQuad = lastQuad.down;
            lastQuad.right = null;
            lastQuad.left = null;
            lastQuad.y--;
        }

        return lastQuad;
    }

    public Quad addLeftNeighbour(Quad newQ)
    {
        if (left == null)
        {
            left = newQ;
            newQ.right = this;
            return newQ;
        }

        newQ.left = left;
        left.right = newQ;
        newQ.right = this;
        left = newQ;

        Quad lastQuad = newQ;

        while (lastQuad.left != null)
        {
            lastQuad.up = lastQuad.left.up;
            lastQuad.down = lastQuad.left.down;
            if (lastQuad.up != null)
                lastQuad.up.down = lastQuad;
            if (lastQuad.down != null)
                lastQuad.down.up = lastQuad;

            lastQuad = lastQuad.left;
            lastQuad.down = null;
            lastQuad.up = null;
            lastQuad.x--;
        }

        return lastQuad;
    }
}

public class QuadMono : MonoBehaviour
{
    public int x, y;
}
