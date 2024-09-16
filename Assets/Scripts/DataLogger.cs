using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger
{
    public static void log(GenerationSettings settings, MapData m, FitnessData f, int mutations, float time, int generations, List<float> bestFitnessCurve, List<float> averageFitnessCurve)
    {
        List<Log> logs;
        if (File.Exists(Path.Combine(Application.persistentDataPath, "logfile.json"))) {
            string jsonToLoad = File.ReadAllText(Path.Combine(Application.persistentDataPath, "logfile.json"));
            logs = new List<Log>(JsonHelper.FromJson<Log>(jsonToLoad));
        }
        else
            logs = new List<Log>();

        Log log = new Log(settings, m, f, time, generations, mutations, bestFitnessCurve, averageFitnessCurve);
        logs.Add(log);
        string json = JsonHelper.ToJson(logs.ToArray(), true);

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "logfile.json"), json);

        //for (int i = 0; i < loadListData.Count; i++) 
        //{ 
        //    Debug.Log("Got: " + loadListData[i].name); 
        //} 

        Debug.Log(Application.persistentDataPath);
    }
}

[Serializable]
public class Log
{
    public GenerationSettings settings;
    public MapData mapData;
    public FitnessData fitnessData;

    public float time;
    public int generations;
    public int mutations;
    public List<float> bestFitnessCurve;
    public List<float> averageFitnessCurve;

    public Log(GenerationSettings settings, MapData mapData, FitnessData fitnessData, float time, int generations, int mutations, List<float> bestFitnessCurve, List<float> averageFitnessCurve)
    {
        this.settings = settings;
        this.mapData = mapData;
        this.fitnessData = fitnessData;
        this.time = time;
        this.generations = generations;
        this.mutations = mutations;
        this.averageFitnessCurve = averageFitnessCurve;
        this.bestFitnessCurve = bestFitnessCurve;
    }
}

[Serializable]
public class MapData
{
    public int roomsCount;
    public int firstLastDistance;
    public float averageRatio;
    public float distributionPercentage;
    public float averageArea;
    public int bottlenecks;
    public List<RoomData> rooms = new List<RoomData>();
}

[Serializable]
public class FitnessData
{
    public float fitness;
    public float sizeFitness;
    public float countFitness;
    public float distanceFitness;
    public float ratioFitness;
    public float distributionFitness;
    public float bottleneckFitness;
}

[Serializable]
public class RoomData
{
    public int id;
    public int quadsNumber;
    public float height;
    public float width;
    public float area;
    //public int connections; 
    public int x;
    public int y;
}