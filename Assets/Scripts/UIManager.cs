using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GenerationSettings settings;
    public Generator generator;
    public CameraMovement cameraMovement;

    public TMP_InputField fitness;
    public TMP_InputField population;
    public TMP_InputField tournament;
    public TMP_InputField mutation;
    public TMP_InputField maxGen;

    public Toggle count;
    public Toggle size;
    public Toggle distance;
    public Toggle filling;
    public Toggle ratio;
    public Toggle bottlneck;

    public TMP_InputField minCount;
    public TMP_InputField maxCount;
    public TMP_InputField minSize;
    public TMP_InputField maxSize;
    public TMP_InputField minDistance;
    public TMP_InputField maxDistance;
    public TMP_InputField fillinginput;
    public Toggle fillingtoggle;
    public TMP_InputField ratioField;

    public TMP_InputField fileName;
    public TMP_InputField maps;
    public GameObject showMap;
    public GameObject settingCanvas;
    public GameObject showCanvas;
    public GameObject generating;

    public TMP_Text totalFitness;
    public TMP_Text countFitness;
    public TMP_Text sizeFitness;
    public TMP_Text ratioFitness;
    public TMP_Text fillingFitness;
    public TMP_Text bottleneckFitness;

    // Start is called before the first frame update
    void Start()
    {
        maps.text = "1";
        fileName.text = "log.json";
        fitness.text = settings.fitnessThreshold.ToString();
        population.text = settings.populationSize.ToString();
        tournament.text = settings.tournamentSize.ToString();
        mutation.text = settings.mutationRate.ToString();
        maxGen.text = settings.maxGenerations.ToString();

        count.isOn = settings.checkRoomsCount;
        size.isOn = settings.checkRoomsSize;
        distance.isOn = settings.checkDistance;
        filling.isOn = settings.checkGridCover;
        ratio.isOn = settings.checkHeightRatio;
        bottlneck.isOn = settings.checkBottlenecks;

        minCount.text = settings.roomsNumber.x.ToString();
        maxCount.text = settings.roomsNumber.y.ToString();
        minSize.text = settings.quadPerRoom.x.ToString();
        maxSize.text = settings.quadPerRoom.y.ToString();
        minDistance.text = settings.firstLastDistance.x.ToString();
        maxDistance.text = settings.firstLastDistance.y.ToString();
        fillinginput.text = settings.coverPercentage.ToString();
        fillingtoggle.isOn = settings.minMaxCover;
        ratioField.text = settings.hwRatio.ToString();
    }

    public void generate()
    {
        showMap.SetActive(false);
        generating.SetActive(true);
        settings.fitnessThreshold = float.Parse(fitness.text);
        settings.populationSize = Int32.Parse(population.text);
        settings.tournamentSize = Int32.Parse(tournament.text);
        settings.mutationRate = Int32.Parse(mutation.text);
        settings.maxGenerations = Int32.Parse(maxGen.text);

        settings.checkRoomsCount = count.isOn;
        settings.checkRoomsSize = size.isOn;
        settings.checkDistance = distance.isOn;
        settings.checkGridCover = filling.isOn;
        settings.checkHeightRatio = ratio.isOn;
        settings.checkBottlenecks = bottlneck.isOn;

        settings.roomsNumber.x = Int32.Parse(minCount.text);
        settings.roomsNumber.y = Int32.Parse(maxCount.text);
        settings.quadPerRoom.x = Int32.Parse(minSize.text);
        settings.quadPerRoom.y = Int32.Parse(maxSize.text);
        settings.firstLastDistance.x = Int32.Parse(minDistance.text);
        settings.firstLastDistance.y = Int32.Parse(maxDistance.text);
        settings.coverPercentage = Int32.Parse(fillinginput.text);
        settings.minMaxCover = fillingtoggle.isOn;
        settings.hwRatio = Int32.Parse(ratioField.text);

        generator.filename = fileName.text;
        generator.mapsToGenerate = Int32.Parse(maps.text);

        generator.generate();
    }

    public void finished()
    {
        showMap.SetActive(true);
        generating.SetActive(false);
    }

    public void show()
    {
        settingCanvas.SetActive(false);
        showCanvas.SetActive(true);
        cameraMovement.inViewer = true;

        totalFitness.text = generator.bestGraph.chromosome.fitness.ToString("F");
        countFitness.text = generator.bestGraph.chromosome.roomsFitness.ToString("F");
        sizeFitness.text = generator.bestGraph.chromosome.sizeFitness.ToString("F");
        ratioFitness.text = generator.bestGraph.chromosome.hwFitness.ToString("F");
        fillingFitness.text = generator.bestGraph.chromosome.connectionFitness.ToString("F");
        bottleneckFitness.text = generator.bestGraph.chromosome.bottleneckFitness.ToString("F");
    }

    public void returnSettings()
    {
        cameraMovement.inViewer = false;
        showCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }
}
