using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

public class Generator : MonoBehaviour
{
    public GenerationSettings settings;
    public bool testing;
    Graph g;
    Graph bestGraph;
    Population maps;
    TileSetting ts;

    // Start is called before the first frame update
    void Start()
    {
        ts = GetComponent<TileSetting>();
        StartCoroutine(generation());
    }

    private IEnumerator generation()
    {
        float time = Time.time;

        int chromosomeLength = 4 + settings.rulesNumber * 6;
        maps = new Population(settings, chromosomeLength);

        FitnessData fitnessData = new FitnessData();
        MapData mapData = new MapData();

        do
        {
            bestGraph = null;

            for (int i = 0; i < maps.population.Length; i++)
            {
                g = ShapeGrammar.generate(maps.population[i], settings.quadSize);

                g.chromosome.calcFitness(g, settings, fitnessData, mapData);

                if (bestGraph == null || g.chromosome.fitness > bestGraph.chromosome.fitness)
                    bestGraph = g;
            }

            Debug.Log("Best Fitness: " + bestGraph.chromosome.fitness);

            if (testing || bestGraph.chromosome.fitness > 1 - settings.fitnessThreshold)
                break;

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


        foreach (Room r in bestGraph.rooms)
            r.show(bestGraph.shiftAmount, bestGraph, ts);
        createCorridors();

        Debug.Log("GENERAZIONI: " + maps.generations);
        Debug.Log("Punteggio rooms: " + bestGraph.roomsFitness);
        Debug.Log("Punteggio distance: " + bestGraph.distanceFitness);
        Debug.Log("Punteggio size: " + bestGraph.sizeFitness);
        Debug.Log("Punteggio HW: " + bestGraph.hwFitness);
        Debug.Log("Punteggio connection: " + bestGraph.connectionFitness);
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

    public void createCorridors()
    {
        Room[] rooms = bestGraph.rooms;
        bool[][] matrix = bestGraph.matrix;
        Vector2 pivot1;
        Vector2 pivot2;
        GameObject corridors = new GameObject("Corridors");
        GameObject go;

        int x = 0;
        int y = 0;
        bool[] visited = new bool[rooms.Length];

        foreach (Room room in rooms)
            room.calcBorderQuads();

        for (int i = 0; i < rooms.Length; i++)
        {
            Debug.Log(rooms[i].id + " Pivot: " + rooms[i].quads[0].pivot);
            y++;
            if (rooms[i].up != null && !visited[rooms[i].up.id] && matrix[i][rooms[i].up.id])
            {
                pivot1 = rooms[i].upperQuad.pivot + Vector2.up * 2.5f + Vector2.right * 0.5f;
                pivot2 = rooms[rooms[i].up.id].bottomQuad.pivot + Vector2.down * 2.5f + Vector2.right * 0.5f;

                while (pivot1 != pivot2)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot1;
                    go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot2;
                    go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    pivot1 += Vector2.up;
                    pivot2 += Vector2.down;
                    x++;
                }
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "" + y;
                go.transform.position = pivot1;
                go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                go.transform.parent = corridors.transform;

            }
            if (rooms[i].right != null && !visited[rooms[i].right.id] && matrix[i][rooms[i].right.id])
            {
                pivot1 = rooms[i].rightQuad.pivot + Vector2.right * 2.5f + Vector2.up * 0.5f;
                pivot2 = rooms[rooms[i].right.id].leftQuad.pivot + Vector2.left * 2.5f + Vector2.up * 0.5f;

                while (pivot1 != pivot2)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot1;
                    go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot2;
                    go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    pivot1 = pivot1 + Vector2.right;
                    pivot2 = pivot2 + Vector2.left;
                    x++;
                }
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "" + y;
                go.transform.position = pivot1;
                go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                go.transform.parent = corridors.transform;
            }
            if (rooms[i].down != null && !visited[rooms[i].down.id] && matrix[i][rooms[i].down.id])
            {
                pivot1 = rooms[i].bottomQuad.pivot + Vector2.down * 2.5f + Vector2.right * 0.5f;
                pivot2 = rooms[rooms[i].down.id].upperQuad.pivot + Vector2.up * 2.5f + Vector2.right * 0.5f;

                while (pivot1 != pivot2)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot1;
                    go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;


                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot2;
                    go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    pivot1 = pivot1 + Vector2.down;
                    pivot2 = pivot2 + Vector2.up;
                    x++;
                }
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "" + y;
                go.transform.position = pivot1;
                go.transform.localScale = Vector2.right * 3 + Vector2.up; //Vector2.one
                go.transform.parent = corridors.transform;
            }
            if (rooms[i].left != null && !visited[rooms[i].left.id] && matrix[i][rooms[i].left.id])
            {
                pivot1 = rooms[i].leftQuad.pivot + Vector2.left * 2.5f + Vector2.up * 0.5f;
                pivot2 = rooms[rooms[i].left.id].rightQuad.pivot + Vector2.right * 2.5f + Vector2.up * 0.5f;

                while (pivot1 != pivot2)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot1;
                    go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.position = pivot2;
                    go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                    go.transform.parent = corridors.transform;
                    go.name = "" + y;

                    pivot1 = pivot1 + Vector2.left;
                    pivot2 = pivot2 + Vector2.right;
                    x++;
                }
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = "" + y;
                go.transform.position = pivot1;
                go.transform.localScale = Vector2.up * 3 + Vector2.right; //Vector2.one
                go.transform.parent = corridors.transform;
            }
            x = 0;
            visited[i] = true;
        }
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
                Gizmos.color = Color.green;

            Gizmos.DrawCube(rooms[j].quads[0].pivot, Vector3.one * 3f);

            // Draw rooms
            //for (int i = 0; i < rooms[j].vertices.Count - 1; i++)
            //    Gizmos.DrawLine(rooms[j].vertices[i], rooms[j].vertices[i + 1]);
            //Gizmos.DrawLine(rooms[j].vertices[rooms[j].vertices.Count - 1], rooms[j].vertices[0]);
            //Gizmos.color = colors[bestGraph.distances[rooms[j].id] % 7];
        }

        //Gizmos.color = Color.white;

        for (int i = 0; i < rooms.Length; i++)
            for (int j = 0; j < rooms.Length; j++)
                if (matrix[i][j])
                    Gizmos.DrawLine(rooms[i].quads[0].pivot, rooms[j].quads[0].pivot);

    }
}