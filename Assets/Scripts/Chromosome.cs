
using UnityEngine;

public class Chromosome
{
    public int[] genes;
    public float mutationRate;
    public int mutations = 0;

    public float fitness;
    public float paretoFitness;
    public float hwFitness, roomsFitness, sizeFitness, distanceFitness, connectionFitness, bottleneckFitness;

    float malus;
    float mean;
    float diff;

    public Chromosome(int length, float mutationRate, bool initialize = true)
    {
        genes = new int[length];
        this.mutationRate = mutationRate;

        if (initialize)
            randomGenes();
    }

    public void randomGenes()
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

        //if (Random.Range(0, 100) < mutationRate)
        //{
        //    genes[0] = Random.Range(0, 2);
        //    mutations++;
        //}
        //if (Random.Range(0, 100) < mutationRate)
        //{
        //    genes[1] = Random.Range(0, 2);
        //    mutations++;
        //}
        //if (Random.Range(0, 100) < mutationRate)
        //{
        //    genes[2] = Random.Range(0, 2);
        //    mutations++;
        //}
        //if (Random.Range(0, 100) < mutationRate)
        //{
        //    genes[3] = Random.Range(0, 2);
        //    mutations++;
        //}

        System.Random rand = new System.Random();
        for (int i = 4; i < genes.Length; i += 6)
        {           
            if (rand.Next(0, 100) < mutationRate)
            {
                genes[i] = rand.Next(0, 6);
                mutations++;
            }
            //if (Random.Range(0, 100) < mutationRate)
            //{
            //    genes[i + 1] = Random.Range(0, 4);
            //    mutations++;
            //}
            //if (Random.Range(0, 100) < mutationRate)
            //{
            //    genes[i + 2] = Random.Range(0, 2);
            //    mutations++;
            //}
            //if (Random.Range(0, 100) < mutationRate)
            //{
            //    genes[i + 3] = Random.Range(0, 2);
            //    mutations++;
            //}
            //if (Random.Range(0, 100) < mutationRate)
            //{
            //    genes[i + 4] = Random.Range(0, 2);
            //    mutations++;
            //}
            //if (Random.Range(0, 100) < mutationRate)
            //{
            //    genes[i + 5] = Random.Range(0, 2);
            //    mutations++;
            //}
        }

        //Debug.Log("Cromosoma dopo la mutation: " + string.Join(", ", genes));
    }

    public void calcFitness(Graph g, GenerationSettings settings/*, FitnessData fitData, MapData mapData*/)
    {
        Room[] rooms = g.rooms;


        if (settings.checkDistance)
        {
            checkDistance(settings, g);
        }

        if (settings.checkRoomsSize)
        {
            checkRoomsSize(settings, rooms);
        }

        if (settings.checkRoomsCount)
        {
            checkRoomsCount(settings, rooms);
        }

        if (settings.checkGridCover)
        {
            checkGridCover(settings, rooms, g);
        }

        if (settings.checkHeightRatio)
        {
            checkHeightRatio(settings, rooms);
        }

        if (settings.checkBottlenecks)
        {
            checkBottlenecks(settings, rooms);
        }

        fitness = (roomsFitness + sizeFitness + distanceFitness + connectionFitness + hwFitness + bottleneckFitness) / 6;

        fitness = 0;
        int cont = 0;

        if (settings.checkDistance)
        {
            cont++;
            fitness += distanceFitness;
        }

        if (settings.checkRoomsSize)
        {
            cont++;
            fitness += sizeFitness;
        }

        if (settings.checkRoomsCount)
        {
            cont++;
            fitness += roomsFitness;
        }

        if (settings.checkGridCover)
        {
            cont++;
            fitness += connectionFitness;
        }

        if (settings.checkHeightRatio)
        {
            cont++;
            fitness += hwFitness;
        }

        if (settings.checkBottlenecks)
        {
            cont++;
            fitness += bottleneckFitness;
        }

        fitness = cont == 0 ? fitness : fitness / cont;

    }

    public void checkDistance(GenerationSettings settings, Graph g)
    {
        mean = (settings.firstLastDistance.y + settings.firstLastDistance.x) / 2;
        diff = (settings.firstLastDistance.y - mean);

        int lastRoom = g.findLast(mean);
        int dist = lastRoom == 0 ? settings.firstLastDistance.y * 10 : g.distances[lastRoom];

        malus = Mathf.Max(Mathf.Abs(dist - mean) - diff, 0);

        distanceFitness = 1 / (malus + 1);
    }

    public void checkRoomsSize(GenerationSettings settings, Room[] rooms)
    {
        float averageSize = 0;

        mean = (settings.quadPerRoom.y + settings.quadPerRoom.x) / 2;
        diff = (settings.quadPerRoom.y - mean);

        malus = 0;
        for (int i = 0; i < rooms.Length; i++)
        {
            averageSize += rooms[i].quadsCount;
            
            malus += Mathf.Max(Mathf.Abs(rooms[i].quadsCount - mean) - diff, 0)/4;
            if (rooms[i].quadsCount == 1)
                malus *= 2;
        }

        malus /= rooms.Length;

        sizeFitness = 1 / (malus + 1);
    }

    public void checkRoomsCount(GenerationSettings settings, Room[] rooms)
    {
        mean = (settings.roomsNumber.y + settings.roomsNumber.x) / 2;
        diff = (settings.roomsNumber.y - mean);

        malus = Mathf.Max(Mathf.Abs(rooms.Length - mean) - diff, 0);

        roomsFitness = 1 / (malus + 1);
    }

    public void checkGridCover(GenerationSettings settings, Room[] rooms, Graph g)
    {
        //Rapporto #righe #colonne 

        // + 1 per considerare l'indice 0 
        float rows = g.maxY + Mathf.Abs(g.minY) + 1;
        float cols = g.maxX + Mathf.Abs(g.minX) + 1;

        float roomPerRow = rooms.Length / rows;
        float roomPerCol = rooms.Length / cols;

        float ratio = rows > cols ? rows / cols : cols / rows;

        if (settings.minMaxCover)
            malus = (Mathf.Max((cols * settings.coverPercentage / 100) - roomPerRow, 0) + Mathf.Max(((rows * settings.coverPercentage / 100) - roomPerCol), 0)) / 2;
        else
            malus = (Mathf.Max(roomPerRow - (cols * settings.coverPercentage / 100), 0) + Mathf.Max((roomPerCol - (rows * settings.coverPercentage / 100)), 0)) / 2;

        malus = malus == 0 ? ratio : malus * ratio;

        connectionFitness = 1 / (malus + 1);
    }

    public void checkHeightRatio(GenerationSettings settings, Room[] rooms)
    {
        malus = 0;
        foreach (Room room in rooms)
        {
            malus += Mathf.Abs(room.height / room.width - settings.hwRatio);

        }

        malus /= rooms.Length;

        hwFitness = 1 / (malus + 1);
    }

    public int checkBottlenecks(GenerationSettings settings, Room[] rooms)
    {
        malus = 0;
        bool up;
        bool down;
        bool left;
        bool right;

        foreach (Room r in rooms)
        {
            for (int i = 0; i < r.quadsCount; i++)
            {
                up = r.quads[i].up != null;
                down = r.quads[i].down != null;
                left = r.quads[i].left != null;
                right = r.quads[i].right != null;

                if (((up || down) && (!left && !right)) || ((left || right) && (!up && !down)))
                    malus += 0.33f;
            }
        }
        bottleneckFitness = 1 / (malus + 1);

        return Mathf.RoundToInt(malus);
    }
}
