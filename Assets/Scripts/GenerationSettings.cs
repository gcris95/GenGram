using GD.MinMaxSlider;
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

    //[Space(20)]

    //[Header("Weights")]
    //public float sizeWeight;
    //public float finalSizeWeight; //TODO
    //public float roomNumberWeight;
    //public float hwRatioWeight;
    //public float connectionWeight;
    //public float distanceWeight;
    //public float varianceWeight; // TODO

    [Space(20)]

    [Header("Map Settings")]

    public bool checkRoomsCount = true;
    public bool checkRoomsSize = true;
    public bool checkDistance = true;
    public bool checkGridCover = true;
    public bool checkHeightRatio = true;
    public bool checkBottlenecks = true;

    [Header("Rooms Number")]
    [MinMaxSlider(2, 100)]
    public Vector2Int roomsNumber = new Vector2Int(7, 15);
    [Header("Rooms size")]
    [MinMaxSlider(1, 100)]
    public Vector2Int quadPerRoom = new Vector2Int(2, 6);
    [Header("First Room-Last Room distance")]
    [MinMaxSlider(1,100)]
    public Vector2Int firstLastDistance = new Vector2Int(1, 10);
    [Header("Rooms distribution")]
    [Tooltip("Toggle this if the cover percentage of the grid must be considered as a minimum, as a maximum otherwise")]
    public bool minMaxCover = true;
    [Range(25f, 100f)]
    public float coverPercentage = 50;
    [Header("Height-Width room ratio")]
    [Range(0.25f, 4)]
    public float hwRatio = 1;
}