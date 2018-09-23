using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Enum utility class.
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// Gets the values for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetValues<T> where T: Enum
        {
            /// <summary>
            /// The cached enum array containing all the values for the Enum type.
            /// </summary>
            public static T[] EnumValues { get; private set; } = null;

            static GetValues()
            {
                EnumValues = (T[])Enum.GetValues(typeof(T));
            }
        }

        /// <summary>
        /// Gets the names for an Enum of a particular type in an array and caches it.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public static class GetNames<T> where T: Enum
        {
            /// <summary>
            /// The cached string array containing all the names in the Enum type.
            /// </summary>
            public static string[] EnumNames { get; private set; } = null;

            static GetNames()
            {
                EnumNames = Enum.GetNames(typeof(T));
            }
        }

        /* Adding flags: flag1 |= flag2            ; 10 | 01 = 11
         * Checking flags: (flag1 & flag2) != 0    ; 11 & 10 = 10
         * Removing flags: (flag1 & (~flag2))      ; 1111 & (~0010) = 1111 & 1101 = 1101
         * */

        /// <summary>
        /// Tells if one set of Enum values contains another set of Enum values. This is for Enums that represent a collection of flags.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        /// <param name="flags">The set of values.</param>
        /// <param name="checkFlags">The set of values to test against.</param>
        /// <returns>true if <paramref name="flags"/> contains <paramref name="checkFlags"/>, otherwise false.</returns>
        public static bool HasEnumVal<T>(in T flags, in T checkFlags) where T : Enum
        {
            long val = Convert.ToInt64(flags);
            long val2 = Convert.ToInt64(checkFlags);

            long check = (val & val2);

            return (check != 0);
        }

        /// <summary>
        /// Adds Enum values onto a set of Enum values. This is for Enums that represent a collection of flags.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        /// <param name="flags">The set of values.</param>
        /// <param name="newFlags">The new values to add.</param>
        /// <returns>An enum value of type <typeparamref name="T"/> with the new values added.</returns>
        public static T AddEnumVal<T>(in T flags, in T newFlags) where T : Enum
        {
            long val = Convert.ToInt64(flags);
            long val2 = Convert.ToInt64(newFlags);

            long add = (val | val2);

            return (T)Enum.ToObject(typeof(T), add);
        }

        /// <summary>
        /// Removes Enum values from a set of Enum values. This is for Enums that represent a collection of flags.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        /// <param name="flags">The set of values.</param>
        /// <param name="flagsToRemove">The values to remove.</param>
        /// <returns>An enum value of type <typeparamref name="T"/> with the values removed.</returns>
        public static T RemoveEnumVal<T>(in T flags, in T flagsToRemove) where T : Enum
        {
            long val = Convert.ToInt64(flags);
            long val2 = Convert.ToInt64(flagsToRemove);

            long remove = (val & (~val2));

            return (T)Enum.ToObject(typeof(T), remove);
        }
    }
}
