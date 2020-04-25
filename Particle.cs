using System;
using System.Linq;
using MersenneTwister;

namespace TSPPSO
{
    public class Particle : IComparable
    {
        public double[] Position { get; set; }

        public double[] Velocity { get; set; }

        public double Fitness { get; set; }

        public Particle(int n)
        {
            Position = Enumerable.Range(0, n).Select(i => Randoms.NextDouble()).ToArray();
            Velocity = Enumerable.Repeat(0.0, n).ToArray();
            Fitness = double.MaxValue;
        }

        public Particle(Particle p)
        {
            this.Position = p.Position.ToArray();
            this.Velocity = p.Velocity.ToArray();
            this.Fitness = p.Fitness;
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                return -1;
            }

            if (obj is Particle p)
            {
                if (Fitness < p.Fitness)
                    return -1;

                if (Fitness > p.Fitness)
                    return 1;

                return 0;
            }

            throw new Exception("Invalid type");
        }

        public override string ToString()
        {
            return $"{Fitness}";
        }
    }
}