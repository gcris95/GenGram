using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Room
{
    // Room ID
    public int id;

    // Quads of the room
    //public List<Quad> quads = new List<Quad>(1);
    public Quad[] quads = new Quad[10];
    public int quadsCount = 0;
    public int quadIndex = 0;
    public Quad center;

    // Room Neighbours
    public bool hasUp, hasDown, hasLeft, hasRight;
    public Room up, down, left, right;
    public int rightID;

    // Position of the room in the world;
    public int x, y;

    public float area;
    public float height, width;
    public List<Vector2> vertices = new List<Vector2>(4);

    public Vector2 position = Vector2.zero;

    public int minX = 0;
    public int maxX = 0;
    public int minY = 0;
    public int maxY = 0;

    public Vector2 topCorridor;
    public Vector2 rightCorridor;
    public Vector2 leftCorridor;
    public Vector2 bottomCorridor;

    public Room(int id, int x, int y, int quadSize, bool[] neighbours)
    {
        this.id = id;
        this.x = x;
        this.y = y;

        hasUp = neighbours[0];
        hasRight = neighbours[1];
        hasDown = neighbours[2];
        hasLeft = neighbours[3];

        //quads.Add(new Quad(0, 0, quadSize));
        quads[0] = new Quad(0, 0, quadSize);
        quadsCount++;
    }

    public void shiftIndexUp()
    {
        quadIndex = Mathf.Min(quadIndex + 1, quadsCount - 1);
    }

    public void shiftIndexDown()
    {
        quadIndex = Mathf.Max(quadIndex - 1, 0);
    }

    public void resetIndex()
    {
        quadIndex = quadsCount - 1;
    }

    public Room addUpNeighbour(Room newR)
    {
        if (up == null)
        {
            up = newR;
            newR.down = this;
            return newR;
        }

        newR.up = up;
        up.down = newR;
        newR.down = this;
        up = newR;

        Room lastRoom = newR;

        while (lastRoom.up != null)
        {
            lastRoom.right = lastRoom.up.right;
            lastRoom.left = lastRoom.up.left;
            if (lastRoom.right != null)
                lastRoom.right.left = lastRoom;
            if (lastRoom.left != null)
                lastRoom.left.right = lastRoom;

            lastRoom = lastRoom.up;
            lastRoom.right = null;
            lastRoom.left = null;
            lastRoom.y++;
        }

        return lastRoom;
    }

    public Room addRightNeighbour(Room newR)
    {
        if (right == null)
        {
            right = newR;
            newR.left = this;
            return newR;
        }

        newR.right = right;

        newR.right.left = newR;

        newR.left = this;

        right = newR;

        Room lastRoom = newR;

        while (lastRoom.right != null)
        {
            lastRoom.up = lastRoom.right.up;
            lastRoom.down = lastRoom.right.down;
            if (lastRoom.up != null)
                lastRoom.up.down = lastRoom;
            if (lastRoom.down != null)
                lastRoom.down.up = lastRoom;

            lastRoom = lastRoom.right;
            lastRoom.down = null;
            lastRoom.up = null;
            lastRoom.x++;
        }

        return lastRoom;
    }

    public Room addDownNeighbour(Room newR)
    {
        if (down == null)
        {
            down = newR;
            newR.up = this;
            return newR;
        }

        newR.down = down;
        down.up = newR;
        newR.up = this;
        down = newR;

        Room lastRoom = newR;

        while (lastRoom.down != null)
        {
            lastRoom.right = lastRoom.down.right;
            lastRoom.left = lastRoom.down.left;
            if (lastRoom.right != null)
                lastRoom.right.left = lastRoom;
            if (lastRoom.left != null)
                lastRoom.left.right = lastRoom;

            lastRoom = lastRoom.down;
            lastRoom.right = null;
            lastRoom.left = null;
            lastRoom.y--;
        }

        return lastRoom;
    }

    public Room addLeftNeighbour(Room newR)
    {
        if (left == null)
        {
            left = newR;
            newR.right = this;
            return newR;
        }

        newR.left = left;
        left.right = newR;
        newR.right = this;
        left = newR;

        Room lastRoom = newR;

        while (lastRoom.left != null)
        {
            lastRoom.up = lastRoom.left.up;
            lastRoom.down = lastRoom.left.down;
            if (lastRoom.up != null)
                lastRoom.up.down = lastRoom;
            if (lastRoom.down != null)
                lastRoom.down.up = lastRoom;

            lastRoom = lastRoom.left;
            lastRoom.down = null;
            lastRoom.up = null;
            lastRoom.x--;
        }

        return lastRoom;
    }

    public void calcSize()
    {
        calcArea();

        if (quadsCount == 1)
        {
            float quadSize = quads[0].size;
            height = quadSize;
            width = quadSize;
        }
        else
        {
            for (int i = 0; i < quadsCount; i++)
            {
                minY = Mathf.Min(minY, quads[i].y);
                maxY = Mathf.Max(maxY, quads[i].y);
                minX = Mathf.Min(minX, quads[i].x);
                maxX = Mathf.Max(maxX, quads[i].x);
            }

            float quadSize = quads[0].size;
            height = (maxY * quadSize + quadSize / 2) - (minY * quads[0].size - quadSize / 2);
            width = (maxX * quads[0].size + quadSize / 2) - (minX * quads[0].size - quadSize / 2);
        }
    }

    public void setTiles()
    {
        if (quadsCount == 1)
        {
            quads[0].tileType = TileType.soloTile;
            return;
        }

        bool upNeighbour;
        bool downNeighbour;
        bool rightNeighbour;
        bool leftNeighbour;

        bool upLeftNeighbour;
        bool upRightNeighbour;
        bool downLeftNeighbour;
        bool downRightNeighbour;

        for(int i = 0; i < quadsCount; i++)
        {
            upNeighbour = quads[i].up != null;
            downNeighbour = quads[i].down != null;
            leftNeighbour = quads[i].left != null;
            rightNeighbour = quads[i].right != null;

            if (upNeighbour && downNeighbour && leftNeighbour && rightNeighbour)
            {
                upLeftNeighbour = quads[i].up.left != null;
                upRightNeighbour = quads[i].up.right != null;
                downLeftNeighbour = quads[i].down.left != null;
                downRightNeighbour = quads[i].down.right != null;

                if (upLeftNeighbour && upRightNeighbour && downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.floorTile;
                else if (!upLeftNeighbour && upRightNeighbour && downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exTopSxangleTile;
                else if (upLeftNeighbour && !upRightNeighbour && downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exTopDxangleTile;
                else if (upLeftNeighbour && upRightNeighbour && !downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exBotSxangleTile;
                else if (upLeftNeighbour && upRightNeighbour && downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exBotDxangleTile;
                else if (!upLeftNeighbour && !upRightNeighbour && downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleTopAngleTile;
                else if (!upLeftNeighbour && upRightNeighbour && !downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleLeftAngleTile;
                else if (upLeftNeighbour && !upRightNeighbour && downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleRightAngleTile;
                else if (upLeftNeighbour && upRightNeighbour && !downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleBotAngleTile;
                else if (!upLeftNeighbour && upRightNeighbour && downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleTopRightAngleTile;
                else if (upLeftNeighbour && !upRightNeighbour && !downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exDoubleBotRightAngleTile;
                else if (!upLeftNeighbour && !upRightNeighbour && !downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.exTripleTopSxAngleTile;
                else if (!upLeftNeighbour && !upRightNeighbour && downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exTripleTopDxAngleTile;
                else if (upLeftNeighbour && !upRightNeighbour && !downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exTripleBopDxAngleTile;
                else if (!upLeftNeighbour && upRightNeighbour && !downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.exTripleBotSxAngleTile;
                else
                    quads[i].tileType = TileType.exAllAngleTile;

            }
            else if (!upNeighbour && downNeighbour && leftNeighbour && rightNeighbour)
            {
                downLeftNeighbour = quads[i].down.left != null;
                downRightNeighbour = quads[i].down.right != null;

                if (downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.topBorderTile;
                else if (!downLeftNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.topBorderLeftAngleTile;
                else if (downLeftNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.topBorderRightAngleTile;
                else
                    quads[i].tileType = TileType.topBorderBotAnglesTile;
            }
            else if (upNeighbour && downNeighbour && leftNeighbour && !rightNeighbour)
            {
                upLeftNeighbour = quads[i].left.up != null;
                downLeftNeighbour = quads[i].left.down != null;

                if (upLeftNeighbour && downLeftNeighbour)
                    quads[i].tileType = TileType.dxBorderTile;
                else if (!upLeftNeighbour && downLeftNeighbour)
                    quads[i].tileType = TileType.rightBorderTopAngleTile;
                else if (upLeftNeighbour && !downLeftNeighbour)
                    quads[i].tileType = TileType.rightBorderBotAngleTile;
                else
                    quads[i].tileType = TileType.rightBorderLeftAnglesTile;
            }
            else if (upNeighbour && !downNeighbour && leftNeighbour && rightNeighbour)
            {
                upLeftNeighbour = quads[i].up.left != null;
                upRightNeighbour = quads[i].up.right != null;

                if (upLeftNeighbour && upRightNeighbour)
                    quads[i].tileType = TileType.botBorderTile;
                else if (!upLeftNeighbour && upRightNeighbour)
                    quads[i].tileType = TileType.botBorderLeftAngleTile;
                else if (upLeftNeighbour && !upRightNeighbour)
                    quads[i].tileType = TileType.botBorderRightAngleTile;
                else
                    quads[i].tileType = TileType.botBorderTopAnglesTile;
            }
            else if (upNeighbour && downNeighbour && !leftNeighbour && rightNeighbour)
            {
                upRightNeighbour = quads[i].right.up != null;
                downRightNeighbour = quads[i].right.down != null;

                if (upRightNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.sxBorderTile;
                else if (!upRightNeighbour && downRightNeighbour)
                    quads[i].tileType = TileType.leftBorderTopAngleTile;
                else if (upRightNeighbour && !downRightNeighbour)
                    quads[i].tileType = TileType.leftBorderBotAngleTile;
                else
                    quads[i].tileType = TileType.leftBorderRightAnglesTile;
            }
            else if (!upNeighbour && !downNeighbour && leftNeighbour && rightNeighbour)
            {
                quads[i].tileType = TileType.topbotborder;
            }
            else if (upNeighbour && downNeighbour && !leftNeighbour && !rightNeighbour)
            {
                quads[i].tileType = TileType.leftrightborder;
            }
            else if (!upNeighbour && downNeighbour && !leftNeighbour && rightNeighbour)
            {
                downRightNeighbour = quads[i].right.down != null;

                if (downRightNeighbour)
                    quads[i].tileType = TileType.topSxangleTile;
                else
                    quads[i].tileType = TileType.topSxangleAndAngleTile;
            }
            else if (!upNeighbour && downNeighbour && leftNeighbour && !rightNeighbour)
            {
                downLeftNeighbour = quads[i].left.down != null;

                if (downLeftNeighbour)
                    quads[i].tileType = TileType.topDxangleTile;
                else
                    quads[i].tileType = TileType.topDxangleAndAngleTile;
            }
            else if (upNeighbour && !downNeighbour && !leftNeighbour && rightNeighbour)
            {
                upRightNeighbour = quads[i].right.up != null;

                if (upRightNeighbour)
                    quads[i].tileType = TileType.botSxangleTile;
                else
                    quads[i].tileType = TileType.botSxangleAndAngleTile;
            }
            else if (upNeighbour && !downNeighbour && leftNeighbour && !rightNeighbour)
            {
                upLeftNeighbour = quads[i].left.up != null;

                if (upLeftNeighbour)
                    quads[i].tileType = TileType.botDxangleTile;
                else
                    quads[i].tileType = TileType.botDxangleAndAngleTile;
            }
            else if (!upNeighbour && !downNeighbour && !leftNeighbour && rightNeighbour)
            {
                quads[i].tileType = TileType.uRightTile;
            }
            else if (upNeighbour && !downNeighbour && !leftNeighbour && !rightNeighbour)
            {
                quads[i].tileType = TileType.uUpTile;
            }
            else if (!upNeighbour && downNeighbour && !leftNeighbour && !rightNeighbour)
            {
                quads[i].tileType = TileType.uBotTile;
            }
            else if (!upNeighbour && !downNeighbour && leftNeighbour && !rightNeighbour)
            {
                quads[i].tileType = TileType.uLeftTile;
            }
        }
    }



    public void calcArea()
    {
        area = quadsCount * quads[0].size * quads[0].size;
    }

    public void shiftRoom(float shiftAmount)
    {
        for (int i = 0; i < vertices.Count; i++)
            vertices[i] = new Vector2(vertices[i].x + (x * shiftAmount), vertices[i].y + (y * shiftAmount));

        //calcPivot();
    }

    public void addQuad(int direction)
    {
        Quad parent = quads[quadIndex];
        Quad newQ = null;
        Quad last;
        quadIndex++;

        if (direction == 0)
        {
            newQ = new Quad(parent.x, parent.y + 1, parent.size);
            last = parent.addUpNeighbour(newQ);

            for (int i = 0; i < quadsCount; i++)
            {
                if (quads[i].y == last.y)
                {
                    if (quads[i].x == last.x - 1)
                    {
                        last.left = quads[i];
                        quads[i].right = last;
                    }
                    else if (quads[i].x == last.x + 1)
                    {
                        last.right = quads[i];
                        quads[i].left = last;
                    }
                }
                else if (quads[i].x == last.x && last.y == quads[i].y - 1)
                {
                    last.up = quads[i];
                    quads[i].down = last;
                }
            }
        }
        else if (direction == 1)
        {
            newQ = new Quad(parent.x + 1, parent.y, parent.size);
            last = parent.addRightNeighbour(newQ);

            for (int i = 0; i < quadsCount; i++)
            {
                if (quads[i].x == last.x)
                {
                    if (quads[i].y == last.y - 1)
                    {
                        last.down = quads[i];
                        quads[i].up = last;
                    }
                    else if (quads[i].y == last.y + 1)
                    {
                        last.up = quads[i];
                        quads[i].down = last;
                    }
                }
                else if (quads[i].y == last.y && last.x == quads[i].x - 1)
                {
                    last.right = quads[i];
                    quads[i].left = last;
                }
            }

        }
        else if (direction == 2)
        {
            newQ = new Quad(parent.x, parent.y - 1, parent.size);
            last = parent.addDownNeighbour(newQ);

            for (int i = 0; i < quadsCount; i++)
            {
                if (quads[i].y == last.y)
                {
                    if (quads[i].x == last.x - 1)
                    {
                        last.left = quads[i];
                        quads[i].right = last;
                    }
                    else if (quads[i].x == last.x + 1)
                    {
                        last.right = quads[i];
                        quads[i].left = last;
                    }
                }
                else if (quads[i].x == last.x && last.y == quads[i].y + 1)
                {
                    last.down = quads[i];
                    quads[i].up = last;
                }
            }
        }
        else
        {
            newQ = new Quad(parent.x - 1, parent.y, parent.size);
            last = parent.addLeftNeighbour(newQ);

            for (int i = 0; i < quadsCount; i++)
            {
                if (quads[i].x == last.x)
                {
                    if (quads[i].y == last.y - 1)
                    {
                        last.down = quads[i];
                        quads[i].up = last;
                    }
                    else if (quads[i].y == last.y + 1)
                    {
                        last.up = quads[i];
                        quads[i].down = last;
                    }
                }
                else if (quads[i].y == last.y && last.x == quads[i].x + 1)
                {
                    last.left = quads[i];
                    quads[i].right = last;
                }
            }

        }

        //quads.Insert(quadIndex, newQ);


        if (quadsCount < quads.Length)
        {
            if (quadIndex < quadsCount)
            {
                Quad prev = quads[quadIndex];
                Quad curr;

                for (int i = quadIndex + 1; i <= quadsCount; i++)
                {
                    curr = quads[i];
                    quads[i] = prev;
                    prev = curr;
                }
                quads[quadIndex] = newQ;

            }
            else
            {
                quads[quadIndex] = newQ;
            }
        }
        else
        {
            Quad[] newList = new Quad[quadsCount * 2];

            for (int i = 0; i <= quadsCount; i++)
            {
                if (i < quadIndex)
                    newList[i] = quads[i];
                else if (i > quadIndex)
                    newList[i] = quads[i - 1];
                else
                    newList[i] = newQ;
            }

            quads = newList;
        }

        quadsCount++;


    }

    public void shift(Vector2 shiftAmount)
    {
        //foreach (Quad q in quads)
        //{
        //    q.shift(shiftAmount);
        //}

        position += shiftAmount;
    }

    public void show(Graph g, TileSetting ts)
    {
        setTiles();

        for(int i = 0; i < quadsCount; i++)
        {
            quads[i].shift(position);
            ts.setTiles(quads[i].pivot, quads[i].tileType);
        }
    }

    public Vector2 calcVerticalBorder(int x, bool bot)
    {
        Quad obj = null;

        for (int i = 0; i < quadsCount; i++)
        {
            if (quads[i].x == x)
            {
                if (obj == null)
                    obj = quads[i];
                else if (bot)
                {
                    if (quads[i].y < obj.y)
                        obj = quads[i];
                }
                else if (quads[i].y > obj.y)
                    obj = quads[i];
            }
        }
        return obj.pivot;
    }

    public Vector2 calcHorizontalBorder(int y, bool left)
    {
        Quad obj = null;

        for (int i = 0; i < quadsCount; i++)
        {
            if (quads[i].y == y)
            {
                if (obj == null)
                    obj = quads[i];
                else if (left)
                {
                    if (quads[i].x < obj.x)
                        obj = quads[i];
                }
                else if (quads[i].x > obj.x)
                    obj = quads[i];
            }
        }
        return obj.pivot;
    }

    public void initializeCorridors()
    {
        topCorridor = new Vector2(float.MaxValue, float.MaxValue);
        rightCorridor = new Vector2(float.MaxValue, float.MaxValue);
        leftCorridor = new Vector2(float.MaxValue, float.MaxValue);
        bottomCorridor = new Vector2(float.MaxValue, float.MaxValue);
    }
}