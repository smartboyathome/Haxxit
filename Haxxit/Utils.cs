using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit
{
    /// <summary>
    /// A general interface for deep-clonable objects.
    /// </summary>
    public interface IDeepCloneable
    {
        object DeepClone();
    }

    /// <summary>
    /// A type-specific interface for deep-clonable objects.
    /// </summary>
    /// <typeparam name="T">The type that is clonable.</typeparam>
    public interface IDeepCloneable<T>
    {
        T DeepClone();
    }

    /// <summary>
    /// A general interface for shallow-clonable objects.
    /// </summary>
    public interface IShallowCloneable
    {
        object ShallowClone();
    }

    /// <summary>
    /// A type-specific interface for shallow-clonable objects.
    /// </summary>
    /// <typeparam name="T">The type that is clonable.</typeparam>
    public interface IShallowCloneable<T>
    {
        T ShallowClone();
    }

    /// <summary>
    /// A (somewhat bad) prime number sequence generator based on iterators and recursion.
    /// </summary>
    class Primes
    {

        /// <summary>
        /// The first n values from an enumerable.
        /// </summary>
        /// <param name="n">The number of n to return.</param>
        /// <param name="the_iterator">The enumerable from which to return those n.</param>
        /// <returns>The new inumerable which iterates over the first n values of the old enumerable.</returns>
        public static IEnumerable<int> firstn(int n, IEnumerable<int> the_iterator)
        {
            int count = 0;
            foreach (int i in the_iterator)
            {
                if (count >= n)
                    break;
                count++;
                yield return i;
            }
        }

        /// <summary>
        /// The first n values from an enumerator.
        /// </summary>
        /// <param name="n">The number of n to return.</param>
        /// <param name="the_iterator">The enumerator from which to return those n.</param>
        /// <returns>A new enumerable which iterates through the old enumerator.</returns>
        public static IEnumerable<int> firstn(int n, IEnumerator<int> the_iterator)
        {
            int count = 0;
            while (the_iterator.MoveNext())
            {
                if (count >= n)
                    break;
                count++;
                yield return the_iterator.Current;
            }
        }

        /// <summary>
        /// Returns a sequence of ints starting at i, then continuing every n ints (in other words, return i, then i += n).
        /// </summary>
        /// <param name="i">The start of the sequence.</param>
        /// <param name="n">What to continue over.</param>
        /// <returns>The resulting sequence from this.</returns>
        public static IEnumerable<int> intsfrom(int i, int n = 1)
        {
            while (true)
            {
                yield return i;
                i += n;
            }
        }

        /// <summary>
        /// Excludes multiples of n from the passed-in enumerable.
        /// </summary>
        /// <param name="n">The number to exclude multiples of.</param>
        /// <param name="the_iterator">The enumerable to exclude multiples from.</param>
        /// <returns>A new enumerable which has multiples of n excluded from it.</returns>
        public static IEnumerable<int> exclude_multiples(int n, IEnumerable<int> the_iterator)
        {
            foreach (int i in the_iterator)
                if (i % n != 0)
                    yield return i;
        }


        /// <summary>
        /// Excludes multiples of n from the passed-in enumerator.
        /// </summary>
        /// <param name="n">The number to exclude multiples of.</param>
        /// <param name="the_iterator">The enumerator to exclude multiples from.</param>
        /// <returns>A new enumerator which has multiples of n excluded from it.</returns>
        public static IEnumerable<int> exclude_multiples(int n, IEnumerator<int> the_iterator)
        {
            while (the_iterator.MoveNext())
                if (the_iterator.Current % n != 0)
                    yield return the_iterator.Current;
        }

        /// <summary>
        /// Returns a standard sieve enumerable that goes on until you run out of stack space. ;)
        /// </summary>
        /// <returns>An enumerable that calculates the sieve.</returns>
        public static IEnumerable<int> sieve()
        {
            return sieve(intsfrom(2).GetEnumerator());
        }

        /// <summary>
        /// Returns a standard sieve enumerable that runs for n iterations.
        /// </summary>
        /// <param name="n">The number of iterations to run the sieve for.</param>
        /// <returns>The enumerable that calculates the first n primes from the sieve.</returns>
        public static IEnumerable<int> sieve(int n)
        {
            return firstn(n, sieve(intsfrom(2).GetEnumerator()));
        }

        /// <summary>
        /// Runs the sieve over the given enumerator. You don't normally need this. Really, you don't.
        /// </summary>
        /// <param name="the_iterator">The enumerator to iterate over.</param>
        /// <returns>The sieve of primes.</returns>
        public static IEnumerable<int> sieve(IEnumerator<int> the_iterator)
        {
            while (the_iterator.MoveNext())
            {
                yield return the_iterator.Current;
                the_iterator = exclude_multiples(the_iterator.Current, the_iterator).GetEnumerator();
            }
        }
    }
}
