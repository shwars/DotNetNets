using static System.Console;
class Digit
{
    public int[] Image;
    public int Label;
}

var fn = @"c:\DEMO\Data\train.csv";
var f = File.ReadLines(fn);
var data = from z in f.Skip(1)
            let zz = z.Split(',').Select(int.Parse)
            select new Digit { Label = zz.First(),
                               Image = zz.Skip(1).ToArray() };

var train = data.Take(10000).ToArray();
var test = data.Skip(10000).Take(1000).ToArray();

// Func<int[],int[],int> dist = (a, b) =>
//    a.Zip(b, (x, y) => { return (x - y) * (x - y); } ).Sum();

int dist(int[] a, int[] b) => a.Zip(b, (x, y) => (x - y) * (x - y)).Sum();

#region MinBy Func
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
#endregion

Func<int[], int> classify = (im) =>
        train.MinBy(d => dist(d.Image, im)).Label;

int count = 0, correct = 0;

foreach(var z in test.Take(100))
{
    var n = classify(z.Image);
    if (z.Label == n) correct++;
    count++;
    WriteLine("{0} => {1}, {2}% correct", z.Label, n, correct * 100 / count);
}
