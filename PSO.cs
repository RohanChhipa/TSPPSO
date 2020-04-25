using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MersenneTwister;

namespace TSPPSO
{
    public class PSO
    {
        public void Run()
        {
            var problem = LoadProblem("problems/48.txt");

            var swarmSize = 50;
            var dimensions = problem.Length;
            var maxIterations = 2000;

            var swarm = InitSwarm(swarmSize, dimensions);
            var personalBests = new Particle[swarmSize];
            var globalBest = new Particle(dimensions);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var iteration = 0; iteration < maxIterations; iteration++)
            {
                swarm = CalculateFitness(swarm, problem);

                personalBests = GetPersonalBests(swarm, personalBests);
                globalBest = GetGlobalBest(personalBests, globalBest);

                CalculateNewVelocity(swarm, personalBests, globalBest);
                CalculateNewPosition(swarm);
            }

            stopwatch.Stop();
            Log($"Elapsed runtime (ms): {stopwatch.ElapsedMilliseconds}");

            Log($"Best fitness: {globalBest.Fitness}");
            Log($"Best found path: {string.Join(", ", RankPosition(globalBest))}");
        }

        private double[][] LoadProblem(string problemFile)
        {
            return File.ReadAllLines(problemFile)
                .Select(s => s.Split(','))
                .Select(s => s.Select(d => double.Parse(d)).ToArray())
                .ToArray();
        }

        private Particle[] InitSwarm(int swarmSize, int dimensions) 
        {
            return Enumerable.Range(0, swarmSize)
                .Select(i => new Particle(dimensions))
                .ToArray();
        }

        private Particle[] CalculateFitness(Particle[] swarm, double[][] problem)
        {
            foreach (var particle in swarm)
            {   
                var order = RankPosition(particle);
                particle.Fitness = 0;

                var current = order.First();
                for (var k = 1; k < order.Length; k++)
                {
                    particle.Fitness += problem[current][order[k]];
                    current = order[k];
                }
            }

            return swarm;
        }

        private int[] RankPosition(Particle particle)
        {
            var ranks = Enumerable.Range(0, particle.Position.Length)
                    .OrderBy(i => particle.Position[i]);

            return ranks.Append(ranks.First())
                .ToArray();
        }

        private Particle[] GetPersonalBests(Particle[] swarm, Particle[] personalBests) 
        {
            for (var k = 0; k < swarm.Length; k++)
            {
                personalBests[k] = swarm[k].CompareTo(personalBests[k]) < 0 ? new Particle(swarm[k]) : personalBests[k];
            }

            return personalBests;
        }

        private Particle GetGlobalBest(Particle[] personalBests, Particle globalBest)
        {
            var min = personalBests.Min();
            return min.CompareTo(globalBest) < 0 ? new Particle(min) : globalBest;
        }

        private void CalculateNewVelocity(Particle[] swarm, Particle[] personalBests, Particle globalBest)
        {
            var w = 0.729844;
            var c = 1.49618;
            
            for (var k = 0; k < swarm.Length; k++)
            {
                var congnitive = personalBests[k].Position
                    .Zip(swarm[k].Position, (pb, cb) => pb - cb)
                    .Select(v => v * c * Randoms.NextDouble());

                var social = globalBest.Position
                    .Zip(swarm[k].Position, (gb, cb) => gb - cb)
                    .Select(v => v * c * Randoms.NextDouble());

                var inertia = swarm[k].Velocity.Select(v => v * w);

                swarm[k].Velocity = congnitive.Zip(social, (c, s) => c + s)
                    .Zip(inertia, (i, c) => i + c)
                    .ToArray();
            }
        }

        private void CalculateNewPosition(Particle[] swarm)
        {
            for (var k = 0; k < swarm.Length; k++)
            {
                swarm[k].Position = swarm[k].Position
                    .Zip(swarm[k].Velocity, (p, v) => p + v)
                    .ToArray();
            }
        }

        private static void Log<T>(T t)
        {
            Console.WriteLine(t);
        }
    }
}