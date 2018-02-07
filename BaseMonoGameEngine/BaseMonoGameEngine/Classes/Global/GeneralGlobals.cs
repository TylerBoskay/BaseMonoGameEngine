using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Class for general global values and references
    /// </summary>
    public static class GeneralGlobals
    {
        /// <summary>
        /// The value that is used in random conditions. If this is less than the result, it returns true.
        /// </summary>
        public const int RandomConditionVal = 100;

        /// <summary>
        /// Random reference for generating pseudo-random numbers.
        /// </summary>
        public static readonly Random Randomizer = new Random();

        public static double GenerateRandomDouble() => (Randomizer.NextDouble() * RandomConditionVal);
        public static int GenerateRandomInt() => Randomizer.Next(RandomConditionVal);
    }
}
