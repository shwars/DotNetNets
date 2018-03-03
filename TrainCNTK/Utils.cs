using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCNTK
{
    public static class Utils
    {
        public static float Next(this Random R, double l, double r)
        {
            return (float)(l + R.NextDouble() * (r - l));
        }

        public static T[] AsArray<T>(this T x)
        {
            return new T[] { x };
        }
    }
}
