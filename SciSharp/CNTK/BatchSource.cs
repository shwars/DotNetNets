using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciSharp.CNTK
{
    public class BatchSource<T> : IEnumerator<IEnumerable<T>>, IEnumerable<IEnumerable<T>>
    {
        public IEnumerable<T> TheEnumerable;
        public IEnumerator<T> TheEnumerator;

        protected T[] Buffer;
        public int BatchSize { get; private set; }

        public IEnumerable<T> Current => Buffer;

        object IEnumerator.Current => Current;

        public BatchSource(IEnumerable<T> I, int BatchSize)
        {
            this.TheEnumerable = I;
            this.BatchSize = BatchSize;
            TheEnumerator = I.Loop().GetEnumerator();
            Buffer = new T[BatchSize];
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            for (int i = 0; i < BatchSize; i++)
            {
                if (!TheEnumerator.MoveNext()) return false; // should not happen!
                Buffer[i] = TheEnumerator.Current;
            }
            return true;
        }

        public void Reset()
        {
            TheEnumerator.Reset();
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }

}
