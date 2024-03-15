using System;
using System.Collections.Generic;
using UnityEngine;

public class DataLogger
{
    public static void log(GenerationSettings settings, Graph g, float time, int generations)
    {
        MapData mapData = g.mapData;
        FitnessData fitnessData = g.fitnessData;

        Log log = new Log(settings, mapData, fitnessData, time, generations, g.chromosome.mutations);

        string json = JsonUtility.ToJson(log);
        System.IO.File.WriteAllText("/Log" + "" + ".json", json);
    }
}

[Serializable]
public class Log
{
    GenerationSettings settings;
    MapData mapData;
    FitnessData fitnessData;
    float executionTime;
    int generationsNumber;
    int mutationsNumber;

    public Log(GenerationSettings settings, MapData mapData, FitnessData fitnessData, float time, int generations, int mutations)
    {
        this.settings = settings;
        this.mapData = mapData;
        this.fitnessData = fitnessData;
        this.executionTime = time;
        this.generationsNumber = generations;
        this.mutationsNumber = mutations;
    }
}

[Serializable]
public class MapData
{
    public int roomsNumber;
    public int firstLastDistance;
    public int extraRoomsNumber;
    public float shapeVariance;
    public List<RoomData> rooms = new List<RoomData>();
}

[Serializable]
public class FitnessData
{
    public float fitness;
    public float roomSizePoints;
    public float roomNumberPoints;
    public float FirstLastDistancePoints;
    public float FinalRoomSizePoints;
    public float HeightWidthPoints;
    public float ConnectionPerRoomPoints;
    public float ExtraRoomPoints;
    public float ShapeVariancePoints;
}

[Serializable]
public class RoomData
{
    public int id;
    public int quadsNumber;
    public float height;
    public float width;
    public float hwRatio;
    public float area;
    public int connections;
    public float shapeValue;
    public bool lastRoom;
}

