using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public Room[] rooms;
    public bool[][] matrix;
    public bool[][] mst;
    public int[] distances;
    public Chromosome chromosome;
    public float shiftAmount;

    public MapData mapData;
    public FitnessData fitnessData;

    public Graph(Room[] roomList, Chromosome c, float shiftAmount)
    {
        this.shiftAmount = shiftAmount;
        rooms = new Room[roomList.Length];
        matrix = new bool[roomList.Length][];
        mst = new bool[roomList.Length][];
        distances = new int[roomList.Length];

        chromosome = c;

        for (int i = 0; i < roomList.Length; i++)
            rooms[roomList[i].id] = roomList[i];

        for (int i = 0; i < roomList.Length; i++)
            matrix[i] = new bool[roomList.Length];

        for (int i = 0; i < roomList.Length; i++)
            mst[i] = new bool[roomList.Length];

        mstPrim();
        addExtraEdges();
        calcDistances();
    }

    /// <summary>
    /// Set mst using a Prim based maze generator
    /// This algorithm is biased toward many short dead ends
    /// </summary>
    public void mstPrim()
    {

        bool[] visited = new bool[rooms.Length];
        visited[0] = true;

        List<(int, int)> priorityqueue = new List<(int, int)>(rooms.Length);

        if (rooms[0].up != null)
            insertOrdered((0, rooms[0].up.id), priorityqueue);

        if (rooms[0].right != null)
            insertOrdered((0, rooms[0].right.id), priorityqueue);

        if (rooms[0].down != null)
            insertOrdered((0, rooms[0].down.id), priorityqueue);

        if (rooms[0].left != null)
            insertOrdered((0, rooms[0].left.id), priorityqueue);

        (int, int) currentEdge;
        int currentNode;

        while (priorityqueue.Count > 0)
        {
            currentEdge = priorityqueue[0];
            priorityqueue.RemoveAt(0);


            if (!visited[currentEdge.Item2])
            {
                currentNode = currentEdge.Item2;
                visited[currentNode] = true;
                mst[currentEdge.Item2][currentEdge.Item1] = true;
                mst[currentEdge.Item1][currentEdge.Item2] = true;
                addEdge(currentEdge.Item1, currentEdge.Item2);

                if (rooms[currentNode].up != null && !visited[rooms[currentNode].up.id])
                    insertOrdered((currentNode, rooms[currentNode].up.id), priorityqueue);
                if (rooms[currentNode].right != null && !visited[rooms[currentNode].right.id])
                    insertOrdered((currentNode, rooms[currentNode].right.id), priorityqueue);
                if (rooms[currentNode].down != null && !visited[rooms[currentNode].down.id])
                    insertOrdered((currentNode, rooms[currentNode].down.id), priorityqueue);
                if (rooms[currentNode].left != null && !visited[rooms[currentNode].left.id])
                    insertOrdered((currentNode, rooms[currentNode].left.id), priorityqueue);
            }
        }
    }

    /// <summary>
    /// Insert "value" into "list" in ascending order
    /// </summary>
    /// <param name="value"> The value to add </param>
    /// <param name="list"> The list where to put the value </param>
    public void insertOrdered((int, int) value, List<(int, int)> list)
    {
        if (list.Count == 0)
        {
            list.Add(value);
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Item2 > value.Item2)
            {
                list.Insert(i, value);
                return;
            }
        }

        list.Add(value);
    }

    /// <summary>
    /// Calculate the distances from the starting Room for each other Room using a BFS
    /// </summary>
    public void calcDistances()
    {
        Queue<int> roomsQueue = new Queue<int>();
        roomsQueue.Enqueue(0);

        bool[] visited = new bool[rooms.Length];
        visited[0] = true;

        int current;
        Room currentRoom;

        distances[0] = 0;
        while (roomsQueue.Count > 0)
        {
            current = roomsQueue.Dequeue();
            currentRoom = rooms[current];

            if (currentRoom.up != null && matrix[current][currentRoom.up.id] && !visited[currentRoom.up.id])
            {
                roomsQueue.Enqueue(currentRoom.up.id);
                visited[currentRoom.up.id] = true;
                distances[currentRoom.up.id] = distances[current] + 1;
            }
            if (currentRoom.right != null && matrix[current][currentRoom.right.id] && !visited[currentRoom.right.id])
            {
                roomsQueue.Enqueue(currentRoom.right.id);
                visited[currentRoom.right.id] = true;
                distances[currentRoom.right.id] = distances[current] + 1;
            }
            if (currentRoom.down != null && matrix[current][currentRoom.down.id] && !visited[currentRoom.down.id])
            {
                roomsQueue.Enqueue(currentRoom.down.id);
                visited[currentRoom.down.id] = true;
                distances[currentRoom.down.id] = distances[current] + 1;
            }
            if (currentRoom.left != null && matrix[current][currentRoom.left.id] && !visited[currentRoom.left.id])
            {
                roomsQueue.Enqueue(currentRoom.left.id);
                visited[currentRoom.left.id] = true;
                distances[currentRoom.left.id] = distances[current] + 1;
            }
            int a = roomsQueue.Count;
            distances[current] = distances[current];
        }

    }

    /// <summary>
    /// Find the last room of the map based on the preferred distance and on the number of connections (==1)
    /// </summary>
    /// <param name="preferredDistance"> The optimal distance we want for the room </param>
    /// <returns> The ID of the last room </returns>
    public int findLast(float preferredDistance)
    {
        float bestDiff = float.MaxValue;
        int lastRoom = 0;

        for (int i = 0; i < rooms.Length; i++)
        {
            if (getConnections(i).Count == 1 && Mathf.Abs(preferredDistance - distances[i]) < bestDiff)
            {
                lastRoom = i;
                bestDiff = Mathf.Abs(preferredDistance - distances[i]);
            }
        }

        return lastRoom;
    }

    /// <summary>
    /// Add extra edges to the matrix based on the room settings
    /// </summary>
    public void addExtraEdges()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].hasUp && rooms[i].up != null && rooms[i].up.hasDown)
                addEdge(rooms[i].id, rooms[i].up.id);

            if (rooms[i].hasRight && rooms[i].right != null && rooms[i].right.hasLeft)
                addEdge(rooms[i].id, rooms[i].right.id);

            if (rooms[i].hasDown && rooms[i].down != null && rooms[i].down.hasUp)
                addEdge(rooms[i].id, rooms[i].down.id);

            if (rooms[i].hasLeft && rooms[i].left != null && rooms[i].left.hasRight)
                addEdge(rooms[i].id, rooms[i].left.id);
        }
    }

    public void addEdge(int idFrom, int idTo)
    {
        if (idFrom < 0 || idTo < 0) return;
        if (idFrom >= rooms.Length || idTo >= rooms.Length) return;

        matrix[idFrom][idTo] = true;
        matrix[idTo][idFrom] = true;
    }

    public List<int> getConnections(int id)
    {
        List<int> connections = new List<int>(4);

        if (rooms[id].up != null && matrix[id][rooms[id].up.id])
            connections.Add(rooms[id].up.id);
        if (rooms[id].right != null && matrix[id][rooms[id].right.id])
            connections.Add(rooms[id].right.id);
        if (rooms[id].down != null && matrix[id][rooms[id].down.id])
            connections.Add(rooms[id].down.id);
        if (rooms[id].left != null && matrix[id][rooms[id].left.id])
            connections.Add(rooms[id].left.id);

        return connections;
    }
}