using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

public class Generator : MonoBehaviour
{
    public GenerationSettings settings;
    public bool testing;
    Graph g;
    Graph bestGraph;
    Population maps;
    TileSetting ts;



    bool started;
    float time;
    int cont = 0;
    int chromosomeLength;
    FitnessData fitnessData = new FitnessData();
    MapData mapData = new MapData();

    // Start is called before the first frame update
    void Start()
    {
        checkSettings();
        ts = GetComponent<TileSetting>();
        StartCoroutine(generation());

        time = Time.time;
        chromosomeLength = 4 + settings.rulesNumber * 6;
        maps = new Population(settings, chromosomeLength);
        //started = true;
    }

    private void Update()
    {
        if (!started)
            return;

        cont++;
        bestGraph = null;

        for (int i = 0; i < maps.population.Length; i++)
        {
            g = ShapeGrammar.generate(maps.population[i], settings.quadSize);

            g.chromosome.calcFitness(g, settings, fitnessData, mapData);

            if (bestGraph == null || g.chromosome.fitness > bestGraph.chromosome.fitness)
                bestGraph = g;
        }

        Debug.Log("Best Fitness: " + bestGraph.chromosome.fitness);

        if (bestGraph.chromosome.fitness < 1 - settings.fitnessThreshold && maps.generations < settings.maxGenerations)
        {
            if (bestGraph.chromosome.fitness < 0.6 && cont > 15)
            {
                maps.reinitialize();
                cont = 0;
            }
            else
                maps.newGeneration();
        }
        else
        {
            createRooms();
            StartCoroutine(createCorridors());
            started = false;
        }
    }

    private IEnumerator generation()
    {
        float time = Time.time;
        int cont = 0;
        int chromosomeLength = 4 + settings.rulesNumber * 6;
        maps = new Population(settings, chromosomeLength);

        FitnessData fitnessData = new FitnessData();
        MapData mapData = new MapData();

        do
        {
            cont++;

            bestGraph = null;

            for (int i = 0; i < maps.population.Length; i++)
            {
                g = ShapeGrammar.generate(maps.population[i], settings.quadSize);

                g.chromosome.calcFitness(g, settings, fitnessData, mapData);

                if (bestGraph == null || g.chromosome.fitness > bestGraph.chromosome.fitness)
                    bestGraph = g;
            }

            Debug.Log("Best Fitness: " + bestGraph.chromosome.fitness);

            if (bestGraph.chromosome.fitness > 1 - settings.fitnessThreshold)
                break;

            if (bestGraph.chromosome.fitness < 0.6 && cont > 40)
            {
                maps.reinitialize();
                cont = 0;
            }
            else
                maps.newGeneration();


            yield return null;
        }
        while (maps.generations < settings.maxGenerations);

        //Debug.Log("--------------MATRIX--------------");
        //for (int i = 0; i < bestGraph.rooms.Length; i++)
        //    Debug.Log(string.Join(", ", bestGraph.matrix[i]));

        //Debug.Log("Best last: " + bestGraph.lastRoomId);
        //Debug.Log("Best lastconnection: " + bestGraph.getConnections(bestGraph.lastRoomId).Count);



        time = Time.time - time;


        createRooms();
        StartCoroutine(createCorridors());

        Debug.Log("GENERAZIONI: " + maps.generations);
        Debug.Log("Punteggio rooms: " + bestGraph.roomsFitness);
        Debug.Log("Punteggio distance: " + bestGraph.distanceFitness);
        Debug.Log("Punteggio size: " + bestGraph.sizeFitness);
        Debug.Log("Punteggio HW: " + bestGraph.hwFitness);
        Debug.Log("Punteggio connection: " + bestGraph.connectionFitness);
        Debug.Log("Punteggio bottleneck: " + bestGraph.bottleneckFitness);
        //Debug.Log("minX: " + bestGraph.minX);
        //Debug.Log("maxX: " + bestGraph.maxX);
        //Debug.Log("minY: " + bestGraph.minY);
        //Debug.Log("maxY: " + bestGraph.maxY);
        //Debug.Log("ROW: " + bestGraph.rows);
        //Debug.Log("COLS: " + bestGraph.cols);
        //Debug.Log("ROOM PER ROW: " + bestGraph.roomPerRow);
        //Debug.Log("ROOM PER COL: " + bestGraph.roomPerCol);
        //Debug.Log("RATIO: " + bestGraph.ratio);
        //Debug.Log("malus connection: " + bestGraph.malus);

        DataLogger.log(settings, bestGraph, time, maps.generations);
    }

    public IEnumerator createCorridors()
    {
        Room[] rooms = bestGraph.rooms;
        bool[][] matrix = bestGraph.matrix;
        Vector2 pivot1;
        Vector2 pivot2;
        GameObject corridors = new GameObject("Corridors");

        int x = 0;
        int y = 0;
        bool[] visited = new bool[rooms.Length];

        Vector2 start;
        Vector2 end;
        int index;
        bool crossedStart = false;
        bool crossedEnd = false;

        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].initializeCorridors();
        }

        for (int i = 0; i < rooms.Length; i++)
        {
            y++;

            if (rooms[i].up != null && !visited[rooms[i].up.id] && matrix[i][rooms[i].up.id])
            {
                index = Random.Range(Mathf.Max(rooms[i].minX, rooms[rooms[i].up.id].minX), Mathf.Min(rooms[i].maxX, rooms[rooms[i].up.id].maxX));
                start = rooms[i].calcVerticalBorder(index, false);
                crossedStart = false;
                crossedEnd = false;

                if (rooms[i].leftCorridor.x != float.MaxValue && start.x < rooms[i].leftCorridor.x && start.y < rooms[i].leftCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(start.x, rooms[i].leftCorridor.y);
                }
                else if (rooms[i].rightCorridor.x != float.MaxValue && start.x > rooms[i].leftCorridor.x && start.y < rooms[i].leftCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(start.x, rooms[i].rightCorridor.y);
                }

                end = rooms[rooms[i].up.id].calcVerticalBorder(index, true);

                if (rooms[i].up.leftCorridor.x != float.MaxValue && end.x < rooms[i].up.leftCorridor.x && end.y > rooms[i].up.leftCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(end.x, rooms[i].up.leftCorridor.y);
                }
                else if (rooms[i].up.rightCorridor.x != float.MaxValue && end.x > rooms[i].up.leftCorridor.x && end.y > rooms[i].up.leftCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(end.x, rooms[i].up.rightCorridor.y);
                }

                rooms[i].topCorridor = start;
                rooms[i].up.bottomCorridor = end;

                if (!crossedStart)
                    ts.setDoorTile(start + Vector2.up * 1.5f, false);

                ts.changeDoorWalls(start + Vector2.up * 1.5f, 4);
                pivot1 = start + Vector2.up * 2.5f + Vector2.right * 0.5f;

                if (!crossedEnd)
                {
                    ts.setDoorTile(end - Vector2.up * 1.5f, false);
                    ts.changeDoorWalls(end - Vector2.up * 1.5f, 1);
                    pivot2 = end + Vector2.down * 2.5f + Vector2.right * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(end - Vector2.up * 0.5f, 1);
                    pivot2 = end + Vector2.down * 1.5f + Vector2.right * 0.5f;
                }


                while (pivot1 != pivot2 && pivot1.y < pivot2.y)
                {
                    ts.createCorridor(pivot1, true);
                    ts.createCorridor(pivot2, true);

                    pivot1 += Vector2.up;
                    pivot2 += Vector2.down;
                    x++;
                    yield return null;
                }

                ts.createCorridor(pivot1, true);
            }
            if (rooms[i].down != null && !visited[rooms[i].down.id] && matrix[i][rooms[i].down.id])
            {
                index = Random.Range(Mathf.Max(rooms[i].minX, rooms[rooms[i].down.id].minX), Mathf.Min(rooms[i].maxX, rooms[rooms[i].down.id].maxX));
                start = rooms[i].calcVerticalBorder(index, true);
                crossedStart = false;
                crossedEnd = false;

                if (rooms[i].leftCorridor.x != float.MaxValue && start.x < rooms[i].leftCorridor.x && start.y > rooms[i].leftCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(start.x, rooms[i].leftCorridor.y);
                }
                else if (rooms[i].rightCorridor.x != float.MaxValue && start.x > rooms[i].rightCorridor.x && start.y > rooms[i].rightCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(start.x, rooms[i].rightCorridor.y);
                }

                end = rooms[rooms[i].down.id].calcVerticalBorder(index, false);

                if (rooms[i].down.leftCorridor.x != float.MaxValue && end.x < rooms[i].down.leftCorridor.x && end.y < rooms[i].down.leftCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(end.x, rooms[i].down.leftCorridor.y);
                }
                else if (rooms[i].down.rightCorridor.x != float.MaxValue && end.x > rooms[i].down.rightCorridor.x && end.y < rooms[i].down.rightCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(end.x, rooms[i].down.rightCorridor.y);
                }

                rooms[i].bottomCorridor = start;
                rooms[i].down.topCorridor = end;

                if (!crossedStart)
                {
                    ts.setDoorTile(start + Vector2.down * 1.5f, false);
                    ts.changeDoorWalls(start + Vector2.down * 1.5f, 1);

                    pivot1 = start + Vector2.down * 2.5f + Vector2.right * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(start + Vector2.down * 0.5f, 1);
                    pivot1 = start + Vector2.down * 1.5f + Vector2.right * 0.5f;
                }

                if (!crossedEnd)
                    ts.setDoorTile(end + Vector2.up * 1.5f, false);

                ts.changeDoorWalls(end + Vector2.up * 1.5f, 4);
                pivot2 = end + Vector2.up * 2.5f + Vector2.right * 0.5f;

                while (pivot1 != pivot2 && pivot1.y > pivot2.y)
                {
                    ts.createCorridor(pivot1, true);
                    ts.createCorridor(pivot2, true);

                    pivot1 = pivot1 + Vector2.down;
                    pivot2 = pivot2 + Vector2.up;
                    x++;
                    yield return null;
                }

                ts.createCorridor(pivot1, true);
            }
            if (rooms[i].right != null && !visited[rooms[i].right.id] && matrix[i][rooms[i].right.id])
            {
                index = Random.Range(Mathf.Max(rooms[i].minY, rooms[rooms[i].right.id].minY), Mathf.Min(rooms[i].maxY, rooms[rooms[i].right.id].maxY));
                start = rooms[i].calcHorizontalBorder(index, false);

                crossedStart = false;
                crossedEnd = false;

                if (rooms[i].topCorridor.x != float.MaxValue && start.x < rooms[i].topCorridor.x && start.y > rooms[i].topCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(rooms[i].topCorridor.x, start.y);
                }
                else if (rooms[i].bottomCorridor.x != float.MaxValue && start.x < rooms[i].bottomCorridor.x && start.y < rooms[i].bottomCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(rooms[i].bottomCorridor.x, start.y);
                }

                end = rooms[rooms[i].right.id].calcHorizontalBorder(index, true);

                if (rooms[i].right.topCorridor.x != float.MaxValue && end.x > rooms[i].right.topCorridor.x && end.y > rooms[i].right.topCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(rooms[i].right.topCorridor.x, end.y);
                }
                else if (rooms[i].right.bottomCorridor.x != float.MaxValue && end.x > rooms[i].right.bottomCorridor.x && end.y < rooms[i].right.bottomCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(rooms[i].right.bottomCorridor.x, end.y);
                }

                rooms[i].rightCorridor = start;
                rooms[i].right.leftCorridor = end;

                if (!crossedStart)
                {
                    ts.setDoorTile(start + Vector2.right * 1.5f, true);
                    ts.changeDoorWalls(start + Vector2.right * 1.5f, 2);
                    pivot1 = start + Vector2.right * 2.5f + Vector2.up * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(start + Vector2.right * 0.5f, 2);
                    pivot1 = start + Vector2.right * 2.5f + Vector2.up * 0.5f;
                }


                if (!crossedEnd)
                {
                    ts.setDoorTile(end + Vector2.left * 1.5f, true);
                    ts.changeDoorWalls(end + Vector2.left * 1.5f, 3);
                    pivot2 = end + Vector2.left * 2.5f + Vector2.up * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(end + Vector2.left * 0.5f, 3);
                    pivot2 = end + Vector2.left * 1.5f + Vector2.up * 0.5f;
                }

                while (pivot1 != pivot2 && pivot1.x < pivot2.x)
                {
                    ts.createCorridor(pivot1, false);
                    ts.createCorridor(pivot2, false);

                    pivot1 = pivot1 + Vector2.right;
                    pivot2 = pivot2 + Vector2.left;
                    x++;
                    yield return null;
                }

                ts.createCorridor(pivot1, false);
            }
            if (rooms[i].left != null && !visited[rooms[i].left.id] && matrix[i][rooms[i].left.id])
            {
                index = Random.Range(Mathf.Max(rooms[i].minY, rooms[rooms[i].left.id].minY), Mathf.Min(rooms[i].maxY, rooms[rooms[i].left.id].maxY));
                start = rooms[i].calcHorizontalBorder(index, true);

                crossedStart = false;
                crossedEnd = false;

                if (rooms[i].topCorridor.x != float.MaxValue && start.x > rooms[i].topCorridor.x && start.y > rooms[i].topCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(rooms[i].topCorridor.x, start.y);
                }
                else if (rooms[i].bottomCorridor.x != float.MaxValue && start.x > rooms[i].bottomCorridor.x && start.y < rooms[i].bottomCorridor.y)
                {
                    crossedStart = true;
                    start = new Vector2(rooms[i].bottomCorridor.x, start.y);
                }

                end = rooms[rooms[i].left.id].calcHorizontalBorder(index, false);

                if (rooms[i].left.topCorridor.x != float.MaxValue && end.x < rooms[i].left.topCorridor.x && end.y > rooms[i].left.topCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(rooms[i].left.topCorridor.x, end.y);
                }
                else if (rooms[i].left.bottomCorridor.x != float.MaxValue && end.x < rooms[i].left.bottomCorridor.x && end.y < rooms[i].left.bottomCorridor.y)
                {
                    crossedEnd = true;
                    end = new Vector2(rooms[i].left.bottomCorridor.x, end.y);
                }

                rooms[i].leftCorridor = start;
                rooms[i].left.rightCorridor = end;

                if (!crossedStart)
                {
                    ts.setDoorTile(start + Vector2.left * 1.5f, true);
                    ts.changeDoorWalls(start + Vector2.left * 1.5f, 3);

                    pivot1 = start + Vector2.left * 2.5f + Vector2.up * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(start + Vector2.left * 0.5f, 3);
                    pivot1 = start + Vector2.left * 1.5f + Vector2.up * 0.5f;
                }


                if (!crossedEnd)
                {
                    ts.setDoorTile(end + Vector2.right * 1.5f, true);
                    ts.changeDoorWalls(end + Vector2.right * 1.5f, 2);
                    pivot2 = end + Vector2.right * 2.5f + Vector2.up * 0.5f;
                }
                else
                {
                    ts.changeDoorWalls(end + Vector2.right * 1.5f, 2);
                    pivot2 = end + Vector2.right * 2.5f + Vector2.up * 0.5f;
                }


                while (pivot1 != pivot2 && pivot1.x > pivot2.x)
                {
                    ts.createCorridor(pivot1, false);
                    ts.createCorridor(pivot2, false);

                    pivot1 = pivot1 + Vector2.left;
                    pivot2 = pivot2 + Vector2.right;
                    x++;
                    yield return null;
                }
                ts.createCorridor(pivot1, false);
            }

            x = 0;
            visited[i] = true;
        }

    }

    public void createRooms()
    {
        Room[] rooms = bestGraph.rooms;

        int currentRow = bestGraph.minY;
        int currentCol = bestGraph.minX;

        int endRow = bestGraph.maxY;
        int endCol = bestGraph.maxX;

        List<Room> currentLine = new List<Room>();

        float shiftValue;
        float totalShift = 0;
        int min = 0;

        while (currentRow < endRow)
        {
            shiftValue = 0;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].y == currentRow)
                {
                    if (rooms[i].maxY > shiftValue)
                        shiftValue = rooms[i].maxY;
                }
            }

            currentRow++;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].minY < min)
                {
                    min = rooms[i].minY;
                }
            }
            totalShift += (shiftValue - min + 1) * rooms[0].quads[0].size + 1;
            //Debug.Log("-----------------------");
            //Debug.Log("current X " + currentRow);
            //Debug.Log("SHIFTVALUE " + shiftValue);
            //Debug.Log("TOTALSHIFT " + totalShift);
            min = 0;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].y == currentRow)
                {
                    rooms[i].shift(Vector2.up * totalShift);
                }
            }
        }

        totalShift = 0;
        while (currentCol < endCol)
        {
            shiftValue = 0;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].x == currentCol)
                {
                    if (rooms[i].maxX > shiftValue)
                        shiftValue = rooms[i].maxX;
                }
            }

            currentCol++;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].minX < min)
                {
                    min = rooms[i].minX;
                }
            }

            totalShift += (shiftValue - min + 1) * rooms[0].quads[0].size + 1;
            //Debug.Log("-----------------------");
            //Debug.Log("current Y " + currentCol);
            //Debug.Log("SHIFTVALUE " + shiftValue);
            //Debug.Log("TOTALSHIFT " + totalShift);
            min = 0;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].x == currentCol)
                    rooms[i].shift(Vector2.right * totalShift);
            }
        }

        foreach (Room room in rooms)
            room.show(bestGraph, ts);
    }

    public void checkSettings()
    {
        float roomMean = (settings.roomsNumber.y + settings.roomsNumber.x) / 2;
        float sizeMean = (settings.quadPerRoom.y + settings.quadPerRoom.x) / 2;

        settings.rulesNumber = (int)(roomMean * sizeMean);
    }


    public void OnDrawGizmos()
    {
        if (bestGraph == null)
            return;
        bool[][] matrix;

        matrix = bestGraph.matrix;

        Room[] rooms = bestGraph.rooms;

        //Color[] colors = new Color[] { Color.white, Color.blue, Color.red, Color.green, Color.cyan, Color.yellow, Color.magenta };
        Gizmos.color = Color.green;

        for (int j = 0; j < rooms.Length; j++)
        {
            if (j == 0)
                Gizmos.color = Color.blue;
            else if (j == bestGraph.lastRoomId)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.clear;

            Gizmos.DrawCube(rooms[j].quads[0].pivot, Vector3.one * 3f);

            // Draw rooms
            //for (int i = 0; i < rooms[j].vertices.Count - 1; i++)
            //    Gizmos.DrawLine(rooms[j].vertices[i], rooms[j].vertices[i + 1]);
            //Gizmos.DrawLine(rooms[j].vertices[rooms[j].vertices.Count - 1], rooms[j].vertices[0]);
            //Gizmos.color = colors[bestGraph.distances[rooms[j].id] % 7];
        }

        //Gizmos.color = Color.white;

        //for (int i = 0; i < rooms.Length; i++)
        //    for (int j = 0; j < rooms.Length; j++)
        //        if (matrix[i][j])
        //            Gizmos.DrawLine(rooms[i].quads[0].pivot, rooms[j].quads[0].pivot);

    }
}