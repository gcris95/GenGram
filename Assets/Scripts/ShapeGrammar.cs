using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShapeGrammar
{
    private static List<Room> list = new List<Room>();
    private static int index = 0;
    private static int id = 1;
    private static int quadSize;

    public static Graph generate(Chromosome c, int qs)
    {
        quadSize = qs;

        int[] genes = c.genes;
        list.Add(new Room(0, 0, 0, quadSize, new bool[] { genes[0] == 1, genes[1] == 1, genes[2] == 1, genes[3] == 1 }));

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

        foreach (Room r in list)
        {
            r.createVertices();
            shiftamount = Mathf.Max(shiftamount, r.width, r.height);
        }

        foreach (Room r in list)
            r.shiftRoom(shiftamount + 1);


        Graph g = new Graph(list.ToArray(), c, shiftamount + 1);

        id = 1;
        list.Clear();
        index = 0;

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

            for (int i = 0; i < list.Count; i++)
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

            for (int i = 0; i < list.Count; i++)
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

            for (int i = 0; i < list.Count; i++)
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

            for (int i = 0; i < list.Count; i++)
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

        list.Insert(index, newR);
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

        foreach (Room r in list)
            r.shiftIndexUp();
    }

    private static void shiftIndexesDown()
    {
        index = Mathf.Min(list.Count - 1, index + 1);

        foreach (Room r in list)
            r.shiftIndexDown();
    }

    private static void resetIndexes()
    {
        index = list.Count - 1;

        foreach (Room r in list)
            r.resetIndex();
    }
}
