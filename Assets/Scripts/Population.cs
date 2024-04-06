using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Population
{
    public Chromosome[] population;
    public Chromosome[] offspring;
    public int generations = 1;
    private int tournamentSize = 5;
    private Chromosome[] matingPool;       // Indici dei cromosomi tra cui fare il crossover
    private int elitism;

    public Population(GenerationSettings settings, int chromosomeLength)
    {
        population = new Chromosome[settings.populationSize];
        offspring = new Chromosome[settings.populationSize];
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
        nonDominatedSorting();

        generations++;
    }

    /// <summary>
    /// Perform a non dominated sorting calculating pareto frontiers of the population R = P U Q
    /// where P is the Population at time t and Q is the offspring after the crossover
    /// </summary>
    public void nonDominatedSorting()
    {
        List<List<Chromosome>> frontiers = new List<List<Chromosome>>();
        List<Chromosome> R = new List<Chromosome>(population);
        R.AddRange(offspring);

        #region sorting
        //int cont = 0; // Numero di cromosomi inseriti
        //int i = 0;    // Numero di frontiera


        //bool dominante = false;
        //int domination;

        //List<Chromosome> dominated = new List<Chromosome>();

        //while (cont < population.Length)
        //{
        //    frontiers.Add(new List<Chromosome>());      // Aggiungi una nuova frontiera
        //    frontiers[i].Add(R[0]);                     // Aggiungi primo elemento della popolazione nella frontiera
        //    R.RemoveAt(0);                              // Rimuovi il primo elemento dalla popolazione
        //    cont++;

        //    for (int j = 1; j < R.Count; j++)            // Per ogni individuo rimanente nella popolazione
        //    {
        //        for (int k = 0; k < frontiers[i].Count; k++)     // Per ogni individuo nella frontiera
        //        {
        //            domination = dominates(R[j], frontiers[i][k]);
        //            if (domination == 1)                // Se l'individuo nella popolazione domina
        //            {
        //                dominated.Add(frontiers[i][k]);     // Aggiungi l'individuo della frontiera alla lista dei dominati
        //                frontiers[i].RemoveAt(k);           // Rimuovi l'individuo dalla frontiera
        //                k--;                            // decrementa l'indice k
        //                dominante = true;               // Setta l'individuo della popolazione come candidato
        //            }
        //            else if (domination == 0)           // Se l'individuo nella popolazione non domina su questo individuo della frontiera ma non è neanche dominato
        //                dominante = true;               // Setta l'individuo della popolazione come candidato
        //            else                                // Altrimenti, se l'individuo nella frontiera domina l'individuo della popolazione e l'individuo della popolazione è candidato
        //            {
        //                if (dominante)
        //                    dominante = false;          // Annulla la sua candidatura
        //                break;                          // Esci dal for, non può entrare in frontiera
        //            }
        //        }

        //        if (dominante)          // Se l'individuo della popolazione risulta alla fine risulta dominante
        //        {
        //            frontiers[i].Add(R[j]);     // Aggiungi l'individuo in frontiera
        //            R.RemoveAt(j);              // Rimuovilo dalla popolazione
        //            j--;                        // Decrementa l'indice j
        //            dominante = false;          // Resetta la candidatura
        //            cont++;
        //        }
        //    }

        //    if (dominated.Count > 0)
        //    {
        //        R.AddRange(dominated);      // Riaggiungi gli elementi della frontiera dominati alla popolazione
        //        cont -= dominated.Count;    // Diminuisci il numero di individui aggiunti rispetto a quelli tolti dalla frontiera
        //        dominated.Clear();          // Svuota la lista di individui tolti dalla frontiera
        //    }

        //    i++;                        // Incrementa l'indice della frontiera
        //}

        #endregion

        #region sorting NSGA-II

        List<List<Chromosome>> fronts = new List<List<Chromosome>>();
        List<Chromosome> firstFront = new List<Chromosome>();
        int domination;
        int cont = 0;

        foreach (Chromosome c in R)
        {
            c.dominationCount = 0;
            c.dominated = new List<Chromosome>();

            foreach (Chromosome q in R)
            {
                domination = dominates(c, q);
                if (domination == 1)
                    c.dominated.Add(q);
                else if (domination == -1)
                    c.dominationCount++;
            }

            if (c.dominationCount == 0)
            {
                cont++;
                firstFront.Add(c);
            }
        }

        fronts.Add(firstFront);
        int i = 0;
        bool exit = false;
        List<Chromosome> nextFront;

        while (!exit && cont < population.Length)
        {
            nextFront = new List<Chromosome>();

            foreach (Chromosome p in fronts[i])
            {
                foreach (Chromosome q in p.dominated)
                {
                    q.dominationCount--;

                    if (q.dominationCount == 0)
                    {
                        cont++;
                        nextFront.Add(q);
                    }
                }
            }

            if (nextFront.Count != 0)
            {
                i++;
                fronts.Add(nextFront);
            }
            else
                exit = true;
        }

        #endregion

        i = 0;

        #region Crowding Distance

        foreach (List<Chromosome> front in fronts)
        {
            if (front.Count == 1)
            {
                population[i] = front[0];
                i++;
            }
            else if (front.Count == 2)
            {
                population[i] = front[0];
                if (i < population.Length - 1)
                    population[i + 1] = front[1];

                i += 2;
            }
            else
            {
                crowdingDistance(front);
                for (int j = 0; j < front.Count; j++)
                {
                    population[i] = front[j];
                    i++;
                    if (i >= population.Length)
                        break;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// A method that confronts two chromosomes based on dominance
    /// </summary>
    /// <param name="a">First individual</param>
    /// <param name="b">Second individual</param>
    /// <returns>1 if A dominates B, -1 if B dominates A, 0 otherwise</returns>
    private int dominates(Chromosome a, Chromosome b)
    {
        if (a.fitness1 < b.fitness1 && a.fitness2 < b.fitness2)
            return 1;

        if (a.fitness1 > b.fitness1 && a.fitness2 > b.fitness2)
            return -1;

        return 0;
    }

    /// <summary>
    /// Calculate crowding distance in a certain pareto frontier and sort it based on it  
    /// </summary>
    /// <param name="frontier">The front of which calculate the crowding distance to</param>
    public void crowdingDistance(List<Chromosome> frontier)
    {
        for (int i = 0; i < frontier[0].Objectives.Length; i++)
        {
            frontier.Sort((a, b) => a.fitness.CompareTo(b.fitness));

            frontier[0].CrowdingDistance = frontier[size - 1].CrowdingDistance = Double.PositiveInfinity;

            for (int j = 1; j < size - 1; j++)
            {
                front[j].CrowdingDistance += (front[j + 1].Objectives[i] - front[j - 1].Objectives[i]);
            }
        }
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

        //Debug.Log("---------------------------------------SELECTION---------------------------------------");

        //for (int i = 0; i < matingPool.Length; i++)
        //{
        //    Debug.Log("Selection: " + string.Join(", ", matingPool[i].genes));
        //}
    }

    public void crossover()
    {
        int i = 0;
        Chromosome[] children = new Chromosome[2];
        while (i < population.Length)
        {
            children = matingPool[i].crossover(matingPool[i + 1]);
            offspring[i] = children[0];
            offspring[i + 1] = children[1];
            i += 2;
        }
    }

    public void mutation()
    {
        for (int i = 0; i < offspring.Length; i++)
            offspring[i].mutate();
    }

    public void orderPopulation()
    {
        Array.Sort(population, delegate (Chromosome x, Chromosome y) { return x.fitness.CompareTo(y.fitness); });

        //for (int i = 0; i < population.Length; ++i)
        //    Debug.Log("Cromosomi dopo ordinamento: " + string.Join(", ", population[i].fitness));

    }
}
