using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GenerationSettings settings;
    public bool testing;
    Graph g;
    Graph bestGraph;
    Population maps;


    // Start is called before the first frame update
    void Start()
    {
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

        foreach (Room r in bestGraph.rooms)
            r.show(bestGraph.shiftAmount);

        time = Time.time - time;

        DataLogger.log(settings, bestGraph, time, maps.generations);
    }

    public void OnDrawGizmos()
    {
        if (bestGraph == null)
            return;
        bool[][] matrix;

        matrix = bestGraph.matrix;

        Room[] rooms = bestGraph.rooms;

        Color[] colors = new Color[] { Color.white, Color.blue, Color.red, Color.green, Color.cyan, Color.yellow, Color.magenta };

        foreach (Room r in rooms)
        {
            Gizmos.color = Color.green;
            // Draw rooms
            for (int i = 0; i < r.vertices.Count - 1; i++)
                Gizmos.DrawLine(r.vertices[i], r.vertices[i + 1]);
            Gizmos.DrawLine(r.vertices[r.vertices.Count - 1], r.vertices[0]);
            Gizmos.color = colors[bestGraph.distances[r.id] % 7];
            Gizmos.DrawCube(r.pivot, Vector3.one * 3f);
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < rooms.Length; i++)
            for (int j = 0; j < rooms.Length; j++)
                if (matrix[i][j])
                    Gizmos.DrawLine(rooms[i].pivot, rooms[j].pivot);

    }
}