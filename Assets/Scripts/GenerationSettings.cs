using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "GenerationSettings")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class GenerationSettings : ScriptableObject
{
    [Header("GA Settings")]
    public float fitnessThreshold;
    public int rulesNumber;
    public int populationSize;
    public int tournamentSize;
    public float mutationRate;
    public int elitism;
    public int maxGenerations;
    [Range(1, 4)]
    public int quadSize;

    [Space(20)]

    [Header("Weights")]
    public float sizeWeight;
    public float finalSizeWeight; //TODO
    public float roomNumberWeight;
    public float hwRatioWeight;
    public float connectionWeight;
    public float extraRoomWeight;
    public float distanceWeight;
    public float varianceWeight; // TODO

    [Space(20)]

    [Header("Map Settings")]
    [Header("Rooms Number")]
    public int minRooms;
    public int maxRooms;
    [Header("Rooms size")]
    public int minSize;
    public int maxSize;
    public int finalRoomSize;
    [Header("Height-Width room ratio")]
    [Range(0.25f, 4)]
    public float hwRatio = 1;
    [Header("Extra Rooms")]
    public int minExtraRooms;
    public int maxExtraRooms;
    [Header("First-Last Room distance")]
    public int minDistance;
    public int maxDistance;
    [Header("Connection per room")]
    [Range(2, 4)]
    public float connections = 3;



}