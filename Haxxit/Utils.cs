using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit
{
    class Primes
    {
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

        public static IEnumerable<int> longsfrom(int i, int n = 1)
        {
            while (true)
            {
                yield return i;
                i += n;
            }
        }

        public static IEnumerable<int> exclude_multiples(int n, IEnumerable<int> the_iterator)
        {
            foreach (int i in the_iterator)
                if (i % n != 0)
                    yield return i;
        }

        public static IEnumerable<int> exclude_multiples(int n, IEnumerator<int> the_iterator)
        {
            while (the_iterator.MoveNext())
                if (the_iterator.Current % n != 0)
                    yield return the_iterator.Current;
        }

        public static IEnumerable<int> sieve()
        {
            return sieve(longsfrom(2).GetEnumerator());
        }

        public static IEnumerable<int> sieve(int iterations)
        {
            return firstn(iterations, sieve(longsfrom(2).GetEnumerator()));
        }

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
