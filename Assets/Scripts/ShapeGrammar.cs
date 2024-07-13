using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShapeGrammar
{
    //private static List<Room> list = new List<Room>();
    private static Room[] list = new Room[20];
    private static int roomsCount = 0;
    private static int index = 0;
    private static int id = 1;
    private static int quadSize;
    private static int[] genes;
    private static int minX = 0, maxX = 0, minY = 0, maxY = 0;

    public static Graph generate(Chromosome c, int qs)
    {
        quadSize = qs;

        genes = c.genes;
        list[0] = new Room(0, 0, 0, quadSize, new bool[] { genes[0] == 1, genes[1] == 1, genes[2] == 1, genes[3] == 1 });
        roomsCount++;

        for (int i = 4; i < genes.Length; i += 6)
        {
            switch (genes[i])
            {
                case 0:
                    addRoom(genes[i + 1], new bool[] { genes[i + 2] == 1, genes[i + 3] == 1, genes[i + 4] == 1, genes[i + 5] == 1 });
                    break;
                case 1:
                    addQuad(genes[i + 1]);
                    break;
                case 2:
                    doubleQuad();
                    break;
                case 3:
                    shiftIndexesUp();
                    break;
                case 4:
                    shiftIndexesDown();
                    break;
                case 5:
                    resetIndexes();
                    break;
            }
        }

        float shiftamount = 0;

        for(int i = 0; i < roomsCount; i++)
        {
            list[i].calcSize();
            shiftamount = Mathf.Max(shiftamount, list[i].width, list[i].height);
        }

        //foreach (Room r in list)
        //    r.shiftRoom((int)shiftamount);

        Graph g = new Graph(list, roomsCount, c, shiftamount + 1, minX, maxX, minY, maxY);

        id = 1;
        //list = new Room[20];
        roomsCount = 0;
        index = 0;

        minX = 0;
        maxX = 0;
        minY = 0;
        maxY = 0;
        return g;
    }

    private static void addRoom(int direction, bool[] hasNeighbours)
    {
        Room parent = list[index];
        Room newR = null;
        Room last;

        index++;

        if (direction == 0)
        {
            newR = new Room(id, parent.x, parent.y + 1, quadSize, hasNeighbours);

            last = parent.addUpNeighbour(newR);

            if (last.y > maxY)
                maxY = last.y;
            else
                for (int i = 0; i < roomsCount; i++)
                {
                    if (list[i].y == last.y)
                    {
                        if (list[i].x == last.x - 1)
                        {
                            last.left = list[i];
                            list[i].right = last;
                        }
                        else if (list[i].x == last.x + 1)
                        {
                            last.right = list[i];
                            list[i].left = last;
                        }
                    }
                    else if (list[i].x == last.x && last.y == list[i].y - 1)
                    {
                        last.up = list[i];
                        list[i].down = last;
                    }
                }

        }
        else if (direction == 1)
        {
            newR = new Room(id, parent.x + 1, parent.y, quadSize, hasNeighbours);

            last = parent.addRightNeighbour(newR);

            if (last.x > maxX)
                maxX = last.x;
            else
                for (int i = 0; i < roomsCount; i++)
                {
                    if (list[i].x == last.x)
                    {
                        if (list[i].y == last.y - 1)
                        {
                            last.down = list[i];
                            list[i].up = last;
                        }
                        else if (list[i].y == last.y + 1)
                        {
                            last.up = list[i];
                            list[i].down = last;
                        }
                    }
                    else if (list[i].y == last.y && last.x == list[i].x - 1)
                    {
                        last.right = list[i];
                        list[i].left = last;
                    }
                }

        }
        else if (direction == 2)
        {
            newR = new Room(id, parent.x, parent.y - 1, quadSize, hasNeighbours);

            last = parent.addDownNeighbour(newR);

            if (last.y < minY)
                minY = last.y;
            else
                for (int i = 0; i < roomsCount; i++)
                {
                    if (list[i].y == last.y)
                    {
                        if (list[i].x == last.x - 1)
                        {
                            last.left = list[i];
                            list[i].right = last;
                        }
                        else if (list[i].x == last.x + 1)
                        {
                            last.right = list[i];
                            list[i].left = last;
                        }
                    }
                    else if (list[i].x == last.x && last.y == list[i].y + 1)
                    {
                        last.down = list[i];
                        list[i].up = last;
                    }
                }
        }
        else
        {
            newR = new Room(id, parent.x - 1, parent.y, quadSize, hasNeighbours);

            last = parent.addLeftNeighbour(newR);

            if (last.x < minX)
                minX = last.x;
            else
                for (int i = 0; i < roomsCount; i++)
                {
                    if (list[i].x == last.x)
                    {
                        if (list[i].y == last.y - 1)
                        {
                            last.down = list[i];
                            list[i].up = last;
                        }
                        else if (list[i].y == last.y + 1)
                        {
                            last.up = list[i];
                            list[i].down = last;
                        }
                    }
                    else if (list[i].y == last.y && last.x == list[i].x + 1)
                    {
                        last.left = list[i];
                        list[i].right = last;
                    }
                }
        }

        //list.Insert(index, newR);

        if (roomsCount < list.Length)
        {
            if (index < roomsCount)
            {
                Room prev = list[index];
                Room curr;

                for (int i = index + 1; i <= roomsCount; i++)
                {
                    curr = list[i];
                    list[i] = prev;
                    prev = curr;
                }
                list[index] = newR;

            }
            else
            {
                list[index] = newR;
            }
        }
        else
        {
            Room[] newList = new Room[roomsCount * 2];

            for (int i = 0; i <= roomsCount; i++)
            {
                if (i < index)
                    newList[i] = list[i];
                else if (i > index)
                    newList[i] = list[i - 1];
                else
                    newList[i] = newR;
            }

            list = newList;
        }

        roomsCount++;
        id++;
    }

    private static void addQuad(int direction)
    {
        list[index].addQuad(direction);
    }

    private static void doubleQuad()
    {
        addQuad(0);
        addQuad(1);
        addQuad(2);
    }

    private static void shiftIndexesUp()
    {
        index = Mathf.Max(0, index - 1);

        for (int i = 0; i < roomsCount; i++)
            list[i].shiftIndexUp();
    }

    private static void shiftIndexesDown()
    {
        index = Mathf.Min(roomsCount - 1, index + 1);


        for (int i = 0; i < roomsCount; i++)
            list[i].shiftIndexDown();
    }

    private static void resetIndexes()
    {
        index = roomsCount - 1;

        for (int i = 0; i < roomsCount; i++)
            list[i].resetIndex();
    }
}
