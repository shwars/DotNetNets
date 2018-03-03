using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciSharp
{
    public class InfiniteEnumerator<T> : IEnumerator<T>
    {
        protected IEnumerator<T> TheEnumerator;

        public InfiniteEnumerator(IEnumerator<T> I)
        {
            this.TheEnumerator = I;
        }
        public T Current => TheEnumerator.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            TheEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            var b = TheEnumerator.MoveNext();
            if (!b)
            {
                TheEnumerator.Reset();
                return TheEnumerator.MoveNext();
            }
            return true;
        }

        public void Reset()
        {
            TheEnumerator.Reset();
        }
    }

    public class InfiniteEnumerable<T> : IEnumerable<T>
    {
        public IEnumerable<T> TheEnumerable;
        public InfiniteEnumerable(IEnumerable<T> I)
        {
            TheEnumerable = I;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new InfiniteEnumerator<T>(TheEnumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new InfiniteEnumerator<T>(TheEnumerable.GetEnumerator());
        }
    }

    public static class IterUtils
    {
        public static IEnumerable<T> Loop<T>(this IEnumerable<T> I) => new InfiniteEnumerable<T>(I);
    }

    public class PairEnumerator<U, V> : IEnumerator<Tuple<U, V>>
    {
        public IEnumerator<U> Enumerator1 { get; private set; }
        public IEnumerator<V> Enumerator2 { get; private set; }

        public Tuple<U, V> Current => new Tuple<U, V>(Enumerator1.Current, Enumerator2.Current);

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Enumerator1.Dispose(); Enumerator2.Dispose();
        }

        public bool MoveNext()
        {
            var b1 = Enumerator1.MoveNext();
            var b2 = Enumerator2.MoveNext();
            return b1 && b2;
        }

        public void Reset()
        {
            Enumerator1.Reset(); Enumerator2.Reset();
        }
    }
}
