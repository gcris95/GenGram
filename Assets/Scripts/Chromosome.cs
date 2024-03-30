using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chromosome
{
    public int[] genes;
    public float fitness;
    public float mutationRate;
    public int mutations = 0;


    public Chromosome(int length, float mutationRate, bool initialize = true)
    {
        genes = new int[length];
        this.mutationRate = mutationRate;

        if (initialize)
            randomGenes();
    }

    private void randomGenes()
    {
        //Debug.Log("--------------CREAZIONE--------------");

        genes[0] = Random.Range(0, 2);
        genes[1] = Random.Range(0, 2);
        genes[2] = Random.Range(0, 2);
        genes[3] = Random.Range(0, 2);

        for (int i = 4; i < genes.Length; i += 6)
        {
            genes[i] = Random.Range(0, 6);
            genes[i + 1] = Random.Range(0, 4);
            //genes[i + 1] = 1;
            genes[i + 2] = Random.Range(0, 2);
            genes[i + 3] = Random.Range(0, 2);
            genes[i + 4] = Random.Range(0, 2);
            genes[i + 5] = Random.Range(0, 2);
        }

        //Debug.Log("GENES: " + string.Join(", ", genes));
    }

    public Chromosome[] crossover(Chromosome other)
    {
        //Debug.Log("--------------CROSSOVER--------------");
        //Debug.Log("Cromosoma 1: " + string.Join(", ", genes));
        //Debug.Log("Cromosoma 2: " + string.Join(", ", other.genes));

        int point = Random.Range(1, genes.Length - 1);

        Chromosome child1 = new Chromosome(genes.Length, mutationRate, false);
        Chromosome child2 = new Chromosome(genes.Length, mutationRate, false);

        int i = 0;

        for (i = 0; i < point; i++)
        {
            child1.genes[i] = genes[i];
            child2.genes[i] = other.genes[i];
        }

        for (i = point; i < genes.Length; i++)
        {
            child1.genes[i] = other.genes[i];
            child2.genes[i] = genes[i];
        }

        //Debug.Log("Child 1: " + string.Join(", ", child1.genes));
        //Debug.Log("Child 2: " + string.Join(", ", child2.genes));

        return new Chromosome[2] { child1, child2 };
    }

    public void mutate()
    {
        //Debug.Log("--------------MUTATION--------------");
        //Debug.Log("Cromosoma prima della mutation: " + string.Join(", ", genes));

        if (Random.Range(0, 100) < mutationRate)
        {
            genes[0] = Random.Range(0, 2);
            mutations++;
        }
        if (Random.Range(0, 100) < mutationRate)
        {
            genes[1] = Random.Range(0, 2);
            mutations++;
        }
        if (Random.Range(0, 100) < mutationRate)
        {
            genes[2] = Random.Range(0, 2);
            mutations++;
        }
        if (Random.Range(0, 100) < mutationRate)
        {
            genes[3] = Random.Range(0, 2);
            mutations++;
        }

        for (int i = 4; i < genes.Length; i += 6)
        {
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i] = Random.Range(0, 6);
                mutations++;
            }
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i + 1] = Random.Range(0, 4);
                mutations++;
            }
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i + 2] = Random.Range(0, 2);
                mutations++;
            }
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i + 3] = Random.Range(0, 2);
                mutations++;
            }
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i + 4] = Random.Range(0, 2);
                mutations++;
            }
            if (Random.Range(0, 100) < mutationRate)
            {
                genes[i + 5] = Random.Range(0, 2);
                mutations++;
            }
        }

        //Debug.Log("Cromosoma dopo la mutation: " + string.Join(", ", genes));
    }

    public void calcFitness(Graph g, GenerationSettings settings, FitnessData fitData, MapData mapData)
    {
        Room[] rooms = g.rooms;

        float malus = 0;
        float mean;
        float diff;

        #region First-Last distance

        mean = (settings.maxDistance + settings.minDistance) / 2;
        diff = (settings.maxDistance - mean);

        int lastRoom = g.findLast(mean);
        int dist = lastRoom == 0 ? settings.maxDistance * 10 : g.distances[lastRoom];

        malus = Mathf.Max(Mathf.Abs(dist - mean) - diff, 0);

        float distanceFitness = 1 / (malus + 1);

        fitData.FirstLastDistancePoints = malus;
        mapData.firstLastDistance = dist;

        #endregion

        #region Rooms size

        mean = (settings.maxSize + settings.minSize) / 2;
        diff = (settings.maxSize - mean);

        #region mean method
        //float[] areas = new float[rooms.Length - 1];

        //for (int i = 0; i < areas.Length; i++)
        //    if (i != lastRoom)
        //        areas[i] = rooms[i].area;

        //float areaMean = calculateMean(areas);
        //float vIndex = calculateVariabilityIndex(areas, areaMean);

        //if (vIndex > 0.1)
        //    points += (vIndex - 0.1f) * 100;
        //points = Mathf.Max(Mathf.Abs(areaMean - mean) - diff, 0);
        #endregion

        malus = 0;
        for (int i = 0; i < rooms.Length; i++)
        {
            //if (i != lastRoom)
            malus += Mathf.Max(Mathf.Abs(rooms[i].area - mean) - diff, 0);
        }


        malus /= (rooms.Length);

        float sizeFitness = 1 / (malus + 1);

        fitData.roomSizePoints = malus;

        //Debug.Log("Points " + points + " rooms: " + rooms.Length + " calcolo: " + (Mathf.Abs(rooms.Length - mean) - diff));

        #endregion

        #region Rooms number

        mean = (settings.maxRooms + settings.minRooms) / 2;
        diff = (settings.maxRooms - mean);

        malus = Mathf.Max(Mathf.Abs(rooms.Length - mean) - diff, 0);

        float roomsFitness = 1 / (malus + 1);

        fitData.roomNumberPoints = malus;
        mapData.roomsNumber = rooms.Length;

        //Debug.Log("Points " + malus + " rooms: " + rooms.Length + " calcolo: " + (Mathf.Abs(rooms.Length - mean) - diff));

        #endregion

        #region Height/Width ratio         

        malus = 0;
        foreach (Room room in rooms)
            malus += Mathf.Abs(room.height / room.width - settings.hwRatio);

        malus /= rooms.Length;

        float hwFitness = 1 / (malus + 1);

        fitData.HeightWidthPoints = malus;

        #endregion

        #region Connection per room

        //Rapporto #righe #colonne

        // + 1 per considerare l'indice 0
        float rows = g.maxY + Mathf.Abs(g.minY) + 1;
        float cols = g.maxX + Mathf.Abs(g.minX) + 1;

        float roomPerRow = rooms.Length / rows;
        float roomPerCol = rooms.Length / cols;

        float ratio = rows > cols ? rows / cols : cols / rows;

        // Considero il grado di pienezza delle righe, ovvero COLS, che è il numero massimo di stanze presenti per ogni riga, meno LA MEDIA DI STANZE PER RIGA
        // Sommo 1 in modo che il risultato non possa essere 0 e peso per la grandezza del grafo, in modo che un grafo 2x10 pieno sia svantaggiato rispetto a un grafo 4x5 (prendo il rateo maggiore, quindi ad esempio 10/2 e non 2/10)
        // Stessa cosa per la pienezza delle colonne
        // Poichè il denominatore non può essere <1, non c'è bisogno di sommare 1 al denominatore nel calcolo della fitness, e il risultato ottimale per il malus sarà 1
        //malus = (ratio * (cols - roomPerRow + 1)) + (ratio * (rows - roomPerCol + 1)) / 2;


        malus = (Mathf.Max((cols * 3 / 5) - roomPerRow, 0) + Mathf.Max(((rows * 3 / 5) - roomPerCol), 0)) / 2;
        malus = malus == 0 ? ratio : malus * ratio;


        g.malus = malus;

        //for (int i = 0; i < rooms.Length; i++)
        //    malus += g.getConnections(i).Count;

        //malus = Mathf.Abs((malus / rooms.Length) - settings.connections);

        float connectionFitness = 1 / (malus + 1);

        fitData.ConnectionPerRoomPoints = connectionFitness;

        #endregion


        //#region Final room size

        //malus = Mathf.Abs(settings.finalRoomSize - rooms[lastRoom].area);

        //float finalRoomFitness = 1 / (malus + 1);

        //fitData.FinalRoomSizePoints = finalRoomFitness;

        //#endregion        

        g.roomsFitness = roomsFitness;
        g.sizeFitness = sizeFitness;
        g.distanceFitness = distanceFitness;
        g.hwFitness = hwFitness;
        g.connectionFitness = connectionFitness;
        g.roomPerRow = roomPerRow;
        g.roomPerCol = roomPerCol;
        g.rows = rows;
        g.cols = cols;
        g.ratio = ratio;

        roomsFitness *= /*settings.roomNumberWeight*/ 0.33f;
        sizeFitness *= /*settings.sizeWeight*/ 0.34f;
        distanceFitness *= /*settings.distanceWeight*/ 0.33f;
        connectionFitness *= /*settings.connectionWeight*/ 0.25f;
        hwFitness *= /*settings.hwRatioWeight*/ 0.2f;

        fitness = roomsFitness + distanceFitness + sizeFitness /*+*/ /*connectionFitness*/ /*+ hwFitness*/;

        //#region Extra room number

        //mean = (settings.maxExtraRooms + settings.minExtraRooms) / 2;
        //diff = (settings.maxExtraRooms - mean);

        //points = Mathf.Max(Mathf.Abs((rooms.Length - dist) - mean) - diff, 0);

        //antiFitness += points * settings.extraRoomWeight;

        //fitData.ExtraRoomPoints = points;
        //mapData.extraRoomsNumber = rooms.Length - dist;

        //#endregion

        #region Shape Variance TODO

        //Distanza di Hausdolff oppure Turning function?
        //antiFitness += points * settings.varianceWeight;

        fitData.ShapeVariancePoints = malus;
        mapData.shapeVariance = 0; // TODO
        #endregion


        RoomData roomData;

        foreach (Room r in rooms)
        {
            roomData = new RoomData();
            roomData.id = r.id;
            roomData.height = r.height;
            roomData.width = r.width;
            roomData.hwRatio = r.height / r.width;
            roomData.area = r.area;
            roomData.connections = g.getConnections(r.id).Count;
            //roomData.lastRoom = lastRoom == r.id;
            roomData.shapeValue = 0; // TODO
            mapData.rooms.Add(roomData);
        }

        fitData.fitness = fitness;
        g.fitnessData = fitData;
        g.mapData = mapData;
    }

    private float calculateMean(float[] values)
    {
        float mean = 0;

        foreach (float value in values)
            mean += value;

        return mean / values.Length;
    }

    private float calculateVariabilityIndex(float[] values, float mean = -1)
    {

        if (mean == -1)
            mean = calculateMean(values);
        float stdMax = mean * Mathf.Sqrt(values.Length - 1);
        float std;
        float sum = 0;

        foreach (float value in values)
            sum += Mathf.Pow(value - mean, 2);

        std = Mathf.Sqrt((sum) / values.Length);

        return std / stdMax;
    }
}
