using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Class dealing with pseudo-randomness.
    /// </summary>
    public static class RandomGlobals
    {
        /// <summary>
        /// The value that is used in random conditions. If this is less than the result, it returns true.
        /// </summary>
        public const int RandomConditionVal = 100;

        /// <summary>
        /// Random reference for generating pseudo-random numbers.
        /// </summary>
        public static readonly Random Randomizer = new Random();
        public static int GenerateRandomInt() => Randomizer.Next(RandomConditionVal);

        /// <summary>
        /// Tests a random condition with two values.
        /// This can be used when calculating a total percentage of something happening.
        /// <para>Two values are multiplied by each other then divided by <paramref name="randCondVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number greater than or equal to 0.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number greater than or equal to 0.</param>
        /// <param name="randCondVal">The value used for the random condition. If this is greater than the result, it returns true.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(in double value1, in double value2, in double randCondVal)
        {
            double value = Randomizer.NextDouble(0, randCondVal);

            double percentageResult = ((value1 * value2) / randCondVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with two values. An int overload.
        /// This is commonly used when calculating a total percentage of something happening.
        /// <para>Two values are multiplied by each other then divided by <paramref name="randCondVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number greater than or equal to 0.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number greater than or equal to 0.</param>
        /// <param name="randCondVal">The value used for the random condition. If this is greater than the result, it returns true.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(in int value1, in int value2, in int randCondVal)
        {
            int value = Randomizer.Next(randCondVal);

            int percentageResult = ((value1 * value2) / randCondVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Chooses a random index in a list of percentages.
        /// </summary>
        /// <param name="percentages">The container of percentages, each with positive values, with the sum adding up to 1.</param>
        /// <returns>The index in the container of percentages that was chosen.</returns>
        public static int ChoosePercentage(in IList<double> percentages)
        {
            double randomVal = Randomizer.NextDouble();
            double value = 0d;

            for (int i = 0; i < percentages.Count; i++)
            {
                value += percentages[i];
                if (value > randomVal)
                {
                    return i;
                }
            }

            //Return the last one if it goes through
            return percentages.Count - 1;
        }
    }
}
