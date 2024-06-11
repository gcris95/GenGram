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
    public List<Quad> quads = new List<Quad>(1);
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

    public void calcSize()
    {
        calcArea();

        if (quads.Count == 1)
        {
            float quadSize = quads[0].size;
            height = quadSize;
            width = quadSize;
        }
        else
        {
            for (int i = 0; i < quads.Count; i++)
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
        if (quads.Count == 1)
        {
            quads[0].tileType = TileType.soloTile;
            return;
        }

        List<Quad> visited = new List<Quad>();

        Quad current = quads[0];
        while (current.up != null)
            current = current.up;

        Quad first = current;

        int direction = 1;
        bool exit = false;

        //Debug.Log("Room: " + id);
        //Debug.Log("First: " + first.x + " " + first.y);

        while (!exit)
        {
            if (direction == 0)
            {
                if (current.left != null)
                {
                    direction = 3;

                    switch (current.tileType)
                    {
                        // Se la tile è vuota
                        case TileType.floorTile:
                            current.tileType = TileType.exBotSxangleTile;                   // Assegna solo l'angolo in alto a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra
                        case TileType.exBotDxangleTile:
                            current.tileType = TileType.exDoubleBotAngleTile;               // Assegna l'angolo in alto a sinistra e a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra
                        case TileType.exTopDxangleTile:
                            current.tileType = TileType.exDoubleBotRightAngleTile;              // Assegna angolo in alto a sinistra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in basso a destra 
                        case TileType.exTopSxangleTile:
                            current.tileType = TileType.exDoubleLeftAngleTile;          // Assegna angolo in alto a sinistra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a sinistra
                        case TileType.exDoubleRightAngleTile:
                            current.tileType = TileType.exTripleBopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a destra
                        case TileType.exDoubleTopAngleTile:
                            current.tileType = TileType.exTripleTopSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra e in basso a destra
                        case TileType.exDoubleTopRightAngleTile:
                            current.tileType = TileType.exTripleBotSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha gli altri 3 angoli
                        case TileType.exTripleTopDxAngleTile:
                            current.tileType = TileType.exAllAngleTile;                     // Assegna tutti e 4
                            break;
                        // Se la tile ha il bordo destro
                        case TileType.dxBorderTile:
                            current.tileType = TileType.rightBorderBotAngleTile;
                            break;
                        // Se la tile ha il bordo basso
                        case TileType.topBorderTile:
                            current.tileType = TileType.topBorderLeftAngleTile;
                            break;
                        //Se la tile ha il bordo destro e l'angolo basso
                        case TileType.rightBorderTopAngleTile:
                            current.tileType = TileType.rightBorderLeftAnglesTile;
                            break;
                        // Se la tile ha il bordo basso e l'angolo destro
                        case TileType.topBorderRightAngleTile:
                            current.tileType = TileType.topBorderBotAnglesTile;
                            break;
                        // Se la tile ha bordo basso e destro
                        case TileType.topDxangleTile:
                            current.tileType = TileType.topDxangleAndAngleTile;
                            break;
                    }

                    if (!visited.Contains(current))
                        visited.Add(current);

                    current = current.left;
                    continue;
                }

                switch (current.tileType)
                {
                    case TileType.floorTile:
                        current.tileType = TileType.sxBorderTile;
                        break;
                    case TileType.topBorderTile:
                        current.tileType = TileType.topSxangleTile;
                        break;
                    case TileType.botBorderTile:
                        current.tileType = TileType.botSxangleTile;
                        break;
                    case TileType.topbotborder:
                        current.tileType = TileType.uRightTile;
                        break;
                    case TileType.botDxangleTile:
                        current.tileType = TileType.uUpTile;
                        break;
                    case TileType.topDxangleTile:
                        current.tileType = TileType.uBotTile;
                        break;
                    case TileType.exTopDxangleTile:
                        current.tileType = TileType.leftBorderTopAngleTile;
                        break;
                    case TileType.exBotDxangleTile:
                        current.tileType = TileType.leftBorderBotAngleTile;
                        break;
                    case TileType.exDoubleRightAngleTile:
                        current.tileType = TileType.leftBorderRightAnglesTile;
                        break;
                    case TileType.botBorderRightAngleTile:
                        current.tileType = TileType.botSxangleAndAngleTile;
                        break;
                    case TileType.topBorderRightAngleTile:
                        current.tileType = TileType.topSxangleAndAngleTile;
                        break;
                    case TileType.dxBorderTile:
                        current.tileType = TileType.leftrightborder;
                        break;
                }

                if (current.up == null)
                {
                    direction = 1;
                    continue;
                }

                if (current == first && visited.Contains(current.up))
                    exit = true;

                if (!visited.Contains(current))
                    visited.Add(current);

                current = current.up;
            }
            else if (direction == 1)
            {
                if (current.up != null)
                {
                    direction = 0;

                    switch (current.tileType)
                    {
                        // Se la tile è vuota
                        case TileType.floorTile:
                            current.tileType = TileType.exTopSxangleTile;                   // Assegna solo l'angolo in alto a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra
                        case TileType.exTopDxangleTile:
                            current.tileType = TileType.exDoubleTopAngleTile;               // Assegna l'angolo in alto a sinistra e a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra
                        case TileType.exBotSxangleTile:
                            current.tileType = TileType.exDoubleLeftAngleTile;              // Assegna angolo in alto a sinistra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in basso a destra 
                        case TileType.exBotDxangleTile:
                            current.tileType = TileType.exDoubleTopRightAngleTile;          // Assegna angolo in alto a sinistra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a sinistra
                        case TileType.exDoubleBotRightAngleTile:
                            current.tileType = TileType.exTripleTopSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a destra
                        case TileType.exDoubleRightAngleTile:
                            current.tileType = TileType.exTripleTopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra e in basso a destra
                        case TileType.exDoubleBotAngleTile:
                            current.tileType = TileType.exTripleBotSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha gli altri 3 angoli
                        case TileType.exTripleBopDxAngleTile:
                            current.tileType = TileType.exAllAngleTile;                     // Assegna tutti e 4
                            break;
                        // Se la tile ha il bordo destro
                        case TileType.dxBorderTile:
                            current.tileType = TileType.rightBorderTopAngleTile;
                            break;
                        // Se la tile ha il bordo basso
                        case TileType.botBorderTile:
                            current.tileType = TileType.botBorderLeftAngleTile;
                            break;
                        //Se la tile ha il bordo destro e l'angolo basso
                        case TileType.rightBorderBotAngleTile:
                            current.tileType = TileType.rightBorderLeftAnglesTile;
                            break;
                        // Se la tile ha il bordo basso e l'angolo destro
                        case TileType.botBorderRightAngleTile:
                            current.tileType = TileType.botBorderTopAnglesTile;
                            break;
                        // Se la tile ha bordo basso e destro
                        case TileType.botDxangleTile:
                            current.tileType = TileType.botDxangleAndAngleTile;
                            break;
                    }

                    if (!visited.Contains(current))
                        visited.Add(current);
                    current = current.up;
                    continue;
                }

                switch (current.tileType)
                {
                    case TileType.floorTile:
                        current.tileType = TileType.topBorderTile;
                        break;
                    case TileType.sxBorderTile:
                        current.tileType = TileType.topSxangleTile;
                        break;
                    case TileType.botSxangleTile:
                        current.tileType = TileType.uRightTile;
                        break;
                    case TileType.exTopSxangleTile:
                        current.tileType = TileType.rightBorderTopAngleTile;
                        break;
                    case TileType.exBotSxangleTile:
                        current.tileType = TileType.topBorderLeftAngleTile;
                        break;
                    case TileType.exDoubleLeftAngleTile:
                        current.tileType = TileType.rightBorderLeftAnglesTile;
                        break;
                    case TileType.leftBorderBotAngleTile:
                        current.tileType = TileType.topSxangleAndAngleTile;
                        break;
                    case TileType.rightBorderBotAngleTile:
                        current.tileType = TileType.topDxangleAndAngleTile;
                        break;
                    case TileType.botBorderTile:
                        current.tileType = TileType.topbotborder;
                        break;
                }

                if (current.right == null)
                {
                    direction = 2;
                    continue;
                }

                if (current == first && visited.Contains(current.right))
                    exit = true;

                if (!visited.Contains(current))
                    visited.Add(current);
                current = current.right;

            }
            else if (direction == 2)
            {
                if (current.right != null)
                {
                    direction = 1;

                    switch (current.tileType)
                    {
                        // Se la tile è vuota
                        case TileType.floorTile:
                            current.tileType = TileType.exTopDxangleTile;                   // Assegna solo l'angolo in alto a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra
                        case TileType.exTopSxangleTile:
                            current.tileType = TileType.exDoubleTopAngleTile;               // Assegna l'angolo in alto a sinistra e a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra
                        case TileType.exBotSxangleTile:
                            current.tileType = TileType.exDoubleBotRightAngleTile;              // Assegna angolo in alto a sinistra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in basso a destra 
                        case TileType.exBotDxangleTile:
                            current.tileType = TileType.exDoubleRightAngleTile;          // Assegna angolo in alto a sinistra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a sinistra
                        case TileType.exDoubleLeftAngleTile:
                            current.tileType = TileType.exTripleTopSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a destra
                        case TileType.exDoubleBotAngleTile:
                            current.tileType = TileType.exTripleBopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra e in basso a destra
                        case TileType.exDoubleTopRightAngleTile:
                            current.tileType = TileType.exTripleTopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha gli altri 3 angoli
                        case TileType.exTripleBotSxAngleTile:
                            current.tileType = TileType.exAllAngleTile;                     // Assegna tutti e 4
                            break;
                        // Se la tile ha il bordo destro
                        case TileType.sxBorderTile:
                            current.tileType = TileType.leftBorderTopAngleTile;
                            break;
                        // Se la tile ha il bordo basso
                        case TileType.botBorderTile:
                            current.tileType = TileType.botBorderRightAngleTile;
                            break;
                        //Se la tile ha il bordo destro e l'angolo basso
                        case TileType.leftBorderBotAngleTile:
                            current.tileType = TileType.leftBorderRightAnglesTile;
                            break;
                        // Se la tile ha il bordo basso e l'angolo destro
                        case TileType.botBorderLeftAngleTile:
                            current.tileType = TileType.botBorderTopAnglesTile;
                            break;
                        // Se la tile ha bordo basso e destro
                        case TileType.botSxangleTile:
                            current.tileType = TileType.botSxangleAndAngleTile;
                            break;
                    }

                    if (!visited.Contains(current))
                        visited.Add(current);
                    current = current.right;
                    continue;
                }

                switch (current.tileType)
                {
                    case TileType.floorTile:
                        current.tileType = TileType.dxBorderTile;
                        break;
                    case TileType.topBorderTile:
                        current.tileType = TileType.topDxangleTile;
                        break;
                    case TileType.topSxangleTile:
                        current.tileType = TileType.uBotTile;
                        break;
                    case TileType.exTopSxangleTile:
                        current.tileType = TileType.rightBorderTopAngleTile;
                        break;
                    case TileType.exBotSxangleTile:
                        current.tileType = TileType.rightBorderBotAngleTile;
                        break;
                    case TileType.exDoubleLeftAngleTile:
                        current.tileType = TileType.rightBorderLeftAnglesTile;
                        break;
                    case TileType.topBorderLeftAngleTile:
                        current.tileType = TileType.topDxangleAndAngleTile;
                        break;
                    case TileType.sxBorderTile:
                        current.tileType = TileType.leftrightborder;
                        break;
                }

                if (current.down == null)
                {
                    direction = 3;
                    continue;
                }

                if (current == first && visited.Contains(current.down))
                    exit = true;

                if (!visited.Contains(current))
                    visited.Add(current);
                current = current.down;
            }
            else
            {
                if (current.down != null)
                {
                    direction = 2;

                    switch (current.tileType)
                    {
                        // Se la tile è vuota
                        case TileType.floorTile:
                            current.tileType = TileType.exBotDxangleTile;                   // Assegna solo l'angolo in alto a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra
                        case TileType.exBotSxangleTile:
                            current.tileType = TileType.exDoubleBotAngleTile;               // Assegna l'angolo in alto a sinistra e a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra
                        case TileType.exTopDxangleTile:
                            current.tileType = TileType.exDoubleRightAngleTile;              // Assegna angolo in alto a sinistra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in basso a destra 
                        case TileType.exTopSxangleTile:
                            current.tileType = TileType.exDoubleTopRightAngleTile;          // Assegna angolo in alto a sinistra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a sinistra
                        case TileType.exDoubleLeftAngleTile:
                            current.tileType = TileType.exTripleBotSxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a sinistra
                            break;
                        // Se la tile ha già l'angolo in alto a destra e in basso a destra
                        case TileType.exDoubleTopAngleTile:
                            current.tileType = TileType.exTripleTopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha già l'angolo in basso a sinistra e in basso a destra
                        case TileType.exDoubleBotRightAngleTile:
                            current.tileType = TileType.exTripleBopDxAngleTile;             // Assegna angolo in alto a sinistra, in alto a destra e in basso a destra
                            break;
                        // Se la tile ha gli altri 3 angoli
                        case TileType.exTripleTopSxAngleTile:
                            current.tileType = TileType.exAllAngleTile;                     // Assegna tutti e 4
                            break;
                        // Se la tile ha il bordo destro
                        case TileType.sxBorderTile:
                            current.tileType = TileType.leftBorderBotAngleTile;
                            break;
                        // Se la tile ha il bordo basso
                        case TileType.topBorderTile:
                            current.tileType = TileType.topBorderRightAngleTile;
                            break;
                        //Se la tile ha il bordo destro e l'angolo basso
                        case TileType.leftBorderTopAngleTile:
                            current.tileType = TileType.leftBorderRightAnglesTile;
                            break;
                        // Se la tile ha il bordo basso e l'angolo destro
                        case TileType.topBorderLeftAngleTile:
                            current.tileType = TileType.topBorderBotAnglesTile;
                            break;
                        // Se la tile ha bordo basso e destro
                        case TileType.topSxangleTile:
                            current.tileType = TileType.topSxangleAndAngleTile;
                            break;
                    }

                    if (!visited.Contains(current))
                        visited.Add(current);
                    current = current.down;
                    continue;
                }

                switch (current.tileType)
                {
                    case TileType.floorTile:
                        current.tileType = TileType.botBorderTile;
                        break;
                    case TileType.dxBorderTile:
                        current.tileType = TileType.botDxangleTile;
                        break;
                    case TileType.topDxangleTile:
                        current.tileType = TileType.uLeftTile;
                        break;
                    case TileType.exTopSxangleTile:
                        current.tileType = TileType.botBorderLeftAngleTile;
                        break;
                    case TileType.exTopDxangleTile:
                        current.tileType = TileType.botBorderRightAngleTile;
                        break;
                    case TileType.exDoubleTopAngleTile:
                        current.tileType = TileType.botBorderTopAnglesTile;
                        break;
                    case TileType.rightBorderTopAngleTile:
                        current.tileType = TileType.botDxangleAndAngleTile;
                        break;
                    case TileType.topBorderTile:
                        current.tileType = TileType.topbotborder;
                        break;
                }

                if (current.left == null)
                {
                    direction = 0;
                    continue;
                }

                if (current == first && visited.Contains(current.left))
                    exit = true;

                if (!visited.Contains(current))
                    visited.Add(current);
                current = current.left;
            }
        }


    }

    public void calcArea()
    {
        area = quads.Count * quads[0].size * quads[0].size;
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
        GameObject room = new GameObject("Room " + id);
        RoomInfoDisplay r = room.AddComponent<RoomInfoDisplay>();

        r.id = id;
        r.width = width;
        r.height = height;
        r.hwRatio = height / width;

        r.connections = g.getConnections(id).Count;
        r.area = area;
        r.x = x; r.y = y;

        r.minX = minX; r.maxX = maxX;
        r.minY = minY; r.maxY = maxY;

        setTiles();

        foreach (Quad q in quads)
        {
            q.shift(position);
            ts.setTiles(q.pivot, q.tileType);
            q.show(room);
        }
    }

    public Vector2 calcVerticalBorder(int x, bool bot)
    {
        Quad obj = null;

        for (int i = 0; i < quads.Count; i++)
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

        for (int i = 0; i < quads.Count; i++)
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