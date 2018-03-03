using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciSharp
{
    public static class ArrayUtils
    {
        public static T MinBy<T>(this IEnumerable<T> seq, Func<T, int> f)
        {
            int min = int.MaxValue;
            T el = default(T);
            foreach (var x in seq)
            {
                if (f(x) < min)
                {
                    min = f(x);
                    el = x;
                }
            }
            return el;
        }

        public static T[,] Reshape<T>(this IEnumerable<T> seq, int x, int y)
        {
            var a = new T[x, y];
            var en = seq.GetEnumerator();
            for (int i=0;i<x;i++)
                for (int j=0;j<y;j++)
                {
                    en.MoveNext();
                    a[i, j] = en.Current;
                }
            return a;
        }
        
        public static int[] ToOneHot10(this int x, int n)
        {
            var res = new int[n];
            for (int i = 0; i < n; i++) res[i] = (i == x) ? 1 : 0;
            return res;
        }

        public static double[] ToDoubleArray(this int[] arr)
        {
            return arr.Select(x => (double)x).ToArray();
        }

        public static float[] ToFloatArray(this int[] arr)
        {
            return arr.Select(x => (float)x).ToArray();
        }


        public static int MaxIndex<T>(this IEnumerable<T> seq) where T:IComparable
        {
            T max = default(T); // TODO
            int pos = 0, i = 0;
            foreach(var x in seq)
            {
                if (max.CompareTo(default(T))==0 || x.CompareTo(max) > 0)
                {
                    max = x;
                    pos = i;
                }
                i++;
            }
            return pos;
        }

    }
}
