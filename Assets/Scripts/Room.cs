using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room
{
    // Room ID
    public int id;

    // Quads of the room
    public List<Quad> quads = new List<Quad>(1);
    public int quadIndex = 0;

    // Room Neighbours
    public bool hasUp, hasDown, hasLeft, hasRight;
    public Room up, down, left, right;
    public int rightID;

    // Position of the room in the world;
    public int x, y;

    public Vector2 pivot;
    public float area;
    public float height, width;
    public List<Vector2> vertices = new List<Vector2>(4);

    public Room(int id, int x, int y, int quadSize, bool[] neighbours)
    {
        this.id = id;
        this.x = x;
        this.y = y;

        hasUp = neighbours[0];
        hasRight = neighbours[1];
        hasDown = neighbours[2];
        hasLeft = neighbours[3];

        quads.Add(new Quad(0, 0, quadSize));
    }

    public void shiftIndexUp()
    {
        quadIndex = Mathf.Min(quadIndex + 1, quads.Count - 1);
    }

    public void shiftIndexDown()
    {
        quadIndex = Mathf.Max(quadIndex - 1, 0);
    }

    public void resetIndex()
    {
        quadIndex = quads.Count - 1;
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

    public void createVertices()
    {
        //Debug.Log("--------------CREAZIONE VERTICI--------------");
        if (quads.Count == 1)
        {
            quads[0].createVertices(true, true, true, true);

            vertices.Add(quads[0].upLeft);
            vertices.Add(quads[0].upRight);
            vertices.Add(quads[0].downRight);
            vertices.Add(quads[0].downLeft);

            height = quads[0].size;
            width = quads[0].size;

            return;
        }

        foreach (Quad q in quads)
            q.shiftQuad();

        Quad current = quads[0];
        while (current.up != null)
            current = current.up;

        int direction = 1;

        current.createVertices(true, false, false, false);

        Vector2 first = current.upLeft;

        vertices.Add(current.upLeft);

        float maxY = 0, minY = 0, maxX = 0, minX = 0;
        int c;

        while (true)
        {
            c = quads.Count;
            if (direction == 0)
            {
                if (current.left != null)
                {
                    direction = 3;
                    current = current.left;
                    continue;
                }

                current.createVertices(true, false, false, false);
                if (current.upLeft == first)
                    break;

                vertices.Add(current.upLeft);

                if (current.upLeft.y > maxY)
                    maxY = current.upLeft.y;
                if (current.upLeft.x < minX)
                    minX = current.upLeft.x;

                if (current.up == null)
                {
                    direction = 1;
                    continue;
                }

                current = current.up;
            }
            else if (direction == 1)
            {
                if (current.up != null)
                {
                    direction = 0;
                    current = current.up;
                    continue;
                }

                current.createVertices(false, true, false, false);
                if (current.upRight == first)
                    break;


                vertices.Add(current.upRight);

                if (current.upRight.y > maxY)
                    maxY = current.upRight.y;
                if (current.upRight.x > maxX)
                    maxX = current.upRight.x;

                if (current.right == null)
                {
                    direction = 2;
                    continue;
                }

                current = current.right;

            }

            else if (direction == 2)
            {
                if (current.right != null)
                {
                    direction = 1;
                    current = current.right;
                    continue;
                }

                current.createVertices(false, false, true, false);
                if (current.downRight == first)
                    break;

                vertices.Add(current.downRight);

                if (current.downRight.y < minY)
                    minY = current.downRight.y;
                if (current.downRight.x > maxX)
                    maxX = current.downRight.x;


                if (current.down == null)
                {
                    direction = 3;
                    continue;
                }

                current = current.down;
            }
            else
            {
                if (current.down != null)
                {
                    direction = 2;
                    current = current.down;
                    continue;
                }

                current.createVertices(false, false, false, true);
                if (current.downLeft == first)
                    break;

                vertices.Add(current.downLeft);

                if (current.downLeft.y < minY)
                    minY = current.downLeft.y;
                if (current.downLeft.x < minX)
                    minX = current.downLeft.x;

                if (current.left == null)
                {
                    direction = 0;
                    continue;
                }

                current = current.left;
            }
        }

        width = maxX - minX;
        height = maxY - minY;
    }

    public void calcArea()
    {
        area = quads.Count * quads[0].size * quads[0].size;
    }

    public void shiftRoom(float shiftAmount)
    {
        for (int i = 0; i < vertices.Count; i++)
            vertices[i] = new Vector2(vertices[i].x + (x * shiftAmount), vertices[i].y + (y * shiftAmount));

        calcPivot();
        calcArea();
    }

    public void simplify()
    {

    }

    public void calcPivot()
    {
        float sumX = 0, sumY = 0;
        foreach (Vector2 v in vertices)
        {
            sumX += v.x;
            sumY += v.y;
        }

        pivot = new Vector2(sumX / vertices.Count, sumY / vertices.Count);
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

            for (int i = 0; i < quads.Count; i++)
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

            for (int i = 0; i < quads.Count; i++)
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

            for (int i = 0; i < quads.Count; i++)
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

            for (int i = 0; i < quads.Count; i++)
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

        quads.Insert(quadIndex, newQ);
    }

    public void show(float shiftAmount, Graph g)
    {
        GameObject room = new GameObject("Room " + id);
        RoomMono r = room.AddComponent<RoomMono>();

        r.id = id;
        r.width = width;
        r.height = height;
        r.hwRatio = height / width;

        r.connections = g.getConnections(id).Count;
        r.area = area;
        r.x = x; r.y = y;

        Vector2 shift = new Vector2((x * shiftAmount), (y * shiftAmount));
        foreach (Quad q in quads)
        {
            q.shiftAgain(shift);
            q.show(room);
        }
    }

}
