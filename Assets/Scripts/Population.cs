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
    public float averagefitness = 0;
    private int tournamentSize = 5;
    private Chromosome[] matingPool;       // Indici dei cromosomi tra cui fare il crossover


    public Population(GenerationSettings settings, int chromosomeLength)
    {
        population = new Chromosome[settings.populationSize];
        offspring = new Chromosome[settings.populationSize];
        matingPool = new Chromosome[settings.populationSize];
        tournamentSize = settings.tournamentSize;

        for (int i = 0; i < settings.populationSize; i++)
            population[i] = new Chromosome(chromosomeLength, settings.mutationRate);
    }

    public void generateOffspring()
    {
        selection();
        crossover();
        mutation();
    }

    /// <summary>
    /// Get the next population performing NSGA-II
    /// </summary>
    public void getNextPopulation()
    {
        generations++;
        List<List<Chromosome>> frontiers = new List<List<Chromosome>>();
        List<Chromosome> R = new List<Chromosome>(population);
        R.AddRange(offspring);

        int i;

        //for (i = 0; i < population.Length; i++)
        //{
        //    Debug.Log("Pop: " + string.Join(", ", population[i].fitness));
        //}

        //for (i = 0; i < offspring.Length; i++)
        //{
        //    Debug.Log("Off: " + string.Join(", ", offspring[i].fitness));
        //}

        //for (i = 0; i < R.Count; i++)
        //{
        //    Debug.Log("R: " + string.Join(", ", R[i].fitness));
        //}

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
            if (c.dominated.Count > 0)
                c.dominated.Clear();

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
        i = 0;
        bool exit = false;
        List<Chromosome> nextFront;

        while (!exit)
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

        //for (i = 0; i < fronts.Count; i++)
        //{
        //    Debug.Log("----------------------------------------------------------");
        //    Debug.Log("Front " + i);
        //    for (int j = 0; j < fronts[i].Count; j++)
        //    {
        //        Debug.Log("Elemento " + j);
        //        for (int k = 0; k < fronts[i][j].fitnesses.Length; k++)
        //        {
        //            Debug.Log(fronts[i][j].fitnesses[k]);
        //        }

        //    }
        //}


        i = 0;

        #region New Population

        Debug.Log("Fronts: " + fronts.Count);
        for (int j = 0; j < fronts.Count; j++)
        {
            Debug.Log("Front: " + j + ": " + fronts[j].Count);
        }

        foreach (List<Chromosome> front in fronts)
        {
            if (front.Count <= 2)
            {
                for (int j = 0; j < front.Count; j++)
                {
                    if (i >= population.Length)
                        break;
                    population[i] = front[j];
                    i++;
                }
            }
            else
            {
                crowdingDistance(front, R);
                for (int j = 0; j < front.Count; j++)
                {
                    if (i >= population.Length)
                        break;
                    population[i] = front[j];
                    i++;
                }
            }
        }

        Debug.Log("i: " + i);

        #endregion

        for (i = 0; i < population.Length; i++)
            Debug.Log("NUOVA POP: " + string.Join(", ", population[i].fitness));

        Debug.Log("---------------------------");
    }


    public void calcAverageFitness()
    {
        averagefitness = 0;
        foreach (Chromosome c in population)
        {
            averagefitness += c.fitness;
        }

        averagefitness /= population.Length;
    }

    public void reinit()
    {
        for (int i = 0; i < population.Length; i++)
        {
            if (i % 2 == 0)
                population[i] = new Chromosome(population[i].genes.Length, population[i].mutationRate);
        }
    }

    /// <summary>
    /// A method that confronts two chromosomes based on dominance
    /// </summary>
    /// <param name="a">First individual</param>
    /// <param name="b">Second individual</param>
    /// <returns>1 if A dominates B, -1 if B dominates A, 0 otherwise</returns>
    private int dominates(Chromosome a, Chromosome b)
    {
        bool better = false;
        bool worst = false;

        for (int i = 0; i < a.fitnesses.Length; i++)
        {
            if (a.fitnesses[i] < b.fitnesses[i])
                worst = true;

            if (a.fitnesses[i] > b.fitnesses[i])
                better = true;
        }

        if (better && !worst) return 1;

        if (worst && !better) return -1;

        return 0;



        //if (a.fitness1 < b.fitness1 && a.fitness2 < b.fitness2)
        //    return 1;

        //if (a.fitness1 > b.fitness1 && a.fitness2 > b.fitness2)
        //    return -1;

        //return 0;
    }

    /// <summary>
    /// Calculate crowding distance in a certain pareto front and sort it based on it  
    /// </summary>
    /// <param name="front">The front of which calculate the crowding distance to</param>
    public void crowdingDistance(List<Chromosome> front, List<Chromosome> R)
    {
        if (front.Count == 0)
            return;

        foreach (Chromosome c in front)
            c.crowdingDistance = 0;

        float min = -1;
        float max = -1;

        for (int i = 0; i < front[0].fitnesses.Length; i++)
        {
            min = -1;
            max = -1;
            front.Sort((a, b) => a.fitnesses[i].CompareTo(b.fitnesses[i]));

            front[0].crowdingDistance = front[front.Count - 1].crowdingDistance = float.MaxValue;

            for (int k = 0; k < R.Count; k++)
            {
                if (min == -1 || R[k].fitnesses[i] < min)
                {
                    min = R[k].fitnesses[i];
                }
                if (max == -1 || R[k].fitnesses[i] > max)
                {
                    max = R[k].fitnesses[i];
                }
            }

            for (int j = 1; j < front.Count - 1; j++)
            {
                if (front[j].crowdingDistance < float.MaxValue)
                    front[j].crowdingDistance += (front[j + 1].fitnesses[i] - front[j - 1].fitnesses[i]) / (max - min); // Possibile miglioramento utilizzando j invece di j-1: https://arxiv.org/ftp/arxiv/papers/1811/1811.12667.pdf
            }
        }

        front.Sort((a, b) => a.crowdingDistance.CompareTo(b.crowdingDistance));
    }

    private void selection()
    {
        List<Chromosome> populationCopy = new List<Chromosome>(population);
        System.Random rnd = new System.Random();

        int winner = -1;
        int index = 0;

        for (int i = 0; i < population.Length; i++)
        {
            for (int j = 0; j < tournamentSize; j++)
            {
                index = rnd.Next(populationCopy.Count);
                if (winner == -1 || index < winner)
                    winner = index;
                populationCopy.RemoveAt(index);
            }
            matingPool[i] = population[winner];
            populationCopy = new List<Chromosome>(population);
            winner = -1;
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
}
