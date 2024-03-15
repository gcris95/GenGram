using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public Chromosome[] population;
    public int generations = 1;
    private int tournamentSize = 5;
    private Chromosome[] matingPool;       // Indici dei cromosomi tra cui fare il crossover
    private int elitism;

    public Population(GenerationSettings settings, int chromosomeLength)
    {
        population = new Chromosome[settings.populationSize];
        matingPool = new Chromosome[settings.populationSize];
        tournamentSize = settings.tournamentSize;
        elitism = settings.elitism;

        for (int i = 0; i < settings.populationSize; i++)
            population[i] = new Chromosome(chromosomeLength, settings.mutationRate);
    }

    public void newGeneration()
    {
        selection();
        crossover();
        mutation();
        generations++;
    }

    private void selection()
    {
        List<Chromosome> populationCopy = new List<Chromosome>(population);
        System.Random rnd = new System.Random();

        Chromosome winner = null;
        int index = 0;

        for (int i = 0; i < population.Length; i++)
        {
            for (int j = 0; j < tournamentSize; j++)
            {
                index = rnd.Next(populationCopy.Count);
                if (winner == null || populationCopy[index].fitness > winner.fitness)
                    winner = populationCopy[index];
                populationCopy.RemoveAt(index);
            }
            matingPool[i] = winner;
            populationCopy = new List<Chromosome>(population);
            winner = null;
        }

        Debug.Log("---------------------------------------SELECTION---------------------------------------");

        for (int i = 0; i < matingPool.Length; i++)
        {
            Debug.Log("Selection: " + string.Join(", ", matingPool[i].genes));
        }
    }

    public void crossover()
    {
        int i = 0;
        Chromosome[] children = new Chromosome[2];
        while (i < population.Length)
        {
            children = matingPool[i].crossover(matingPool[i + 1]);
            population[i] = children[0];
            population[i + 1] = children[1];
            i += 2;
        }

    }

    public void mutation()
    {
        for (int i = 0; i < population.Length; i++)
            population[i].mutate();
    }

    public void orderPopulation()
    {
        Array.Sort(population, delegate (Chromosome x, Chromosome y) { return x.fitness.CompareTo(y.fitness); });

        //for (int i = 0; i < population.Length; ++i)
        //    Debug.Log("Cromosomi dopo ordinamento: " + string.Join(", ", population[i].fitness));

    }
}
