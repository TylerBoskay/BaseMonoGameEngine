using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
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

        public static double GenerateRandomDouble() => (Randomizer.NextDouble() * RandomConditionVal);
        public static int GenerateRandomInt() => Randomizer.Next(RandomConditionVal);

        /// <summary>
        /// Tests a random condition with two values.
        /// This is commonly used when calculating a total percentage of something happening.
        /// For example, this is used when testing whether a move will inflict a Status Effect on a BattleEntity.
        /// <para>Two values are multiplied by each other then divided by <see cref="RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value1, double value2)
        {
            double value = GenerateRandomDouble();

            double percentageResult = ((value1 * value2) / RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value)
        {
            return TestRandomCondition(value, RandomConditionVal);
        }

        /// <summary>
        /// Tests a random condition with two values. An int overload.
        /// This is commonly used when calculating a total percentage of something happening.
        /// <para>Two values are multiplied by each other then divided by <see cref="RandomGlobals.RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value1, int value2)
        {
            int value = GenerateRandomInt();

            int percentageResult = ((value1 * value2) / RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value. An int overload.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value)
        {
            return TestRandomCondition(value, RandomConditionVal);
        }

        /// <summary>
        /// Chooses a random index in a list of percentages
        /// </summary>
        /// <param name="percentages">The container of percentages, each with positive values, with the sum adding up to 1</param>
        /// <returns>The index in the container of percentages that was chosen</returns>
        public static int ChoosePercentage(IList<double> percentages)
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
