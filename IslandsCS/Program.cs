using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IslandsCS
{
    class FindIslands
    {
        private int[,] _matrix;
        private int _count = 0;
        private Stack<(int row, int column)> _elementStack;
        public FindIslands(int[,] matrix)
        {
            _matrix = matrix.Clone() as int[,];
            GetIslands();
        }

        public int Count => _count;

        // for the given element, if its value is 1, push it onto the stack, mark it as "visited" by setting it to zero then return true.
        // Otherwise return false.
        private bool VisitElement(int row, int column)
        {
            if (_matrix[row, column] != 0)
            {
                _elementStack.Push((row, column));
                _matrix[row, column] = 0;
                return true;
            }
            return false;
        }

        private void GetIslands()
        {
            _elementStack = new Stack<(int, int)>();

            var numberOfRows = _matrix.GetLength(0);
            var numberOfColumns = _matrix.GetLength(1);

            for (var i = 0; i < numberOfRows; ++i)
            {
                for (var j = 0; j < numberOfColumns; ++j)
                {
                    if (VisitElement(i, j))
                    {
                        _count++;
                    }

                    while (_elementStack.Any())
                    {
                        var (row, column) = _elementStack.First();
                        var neighbours = false;

                        if (row > 0)
                        {
                            neighbours |= VisitElement(row - 1, column);
                        }
                        if (column > 0)
                        {
                            neighbours |= VisitElement(row, column - 1);
                        }
                        if (row < numberOfRows - 1)
                        {
                            neighbours |= VisitElement(row + 1, column);
                        }
                        if (column < numberOfColumns - 1)
                        {
                            neighbours |= VisitElement(row, column + 1);
                        }
                        if (!neighbours)
                        {
                            _elementStack.Pop();
                        }
                    }
                }
            }

            foreach (var p in _matrix)
            {
                if (p != 0)
                {
                    Console.WriteLine($"p is {p}");
                }
            }
        }
    }

    internal class Program
    {
        /// <summary>
        /// Create a rectangular array - yes I stole this from Jon Skeet! :)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays"></param>
        /// <returns></returns>
        static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            T[,] ret;
            if (arrays.Count == 0)
            {
                ret = new T[,] { };
            }
            int minorLength = arrays[0].Length;
            ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    foreach (var filename in args)
                    {
                        var list = new List<int[]>();
                        foreach (var line in File.ReadLines(filename))
                        {
                            var elements = line.Split().Select(x=>x.Trim()).Where(y=>y.Length > 0).Select(x=>Convert.ToInt32(x));
                            if (elements.Any(x=>x < 0 || x > 1))
                            {
                                throw new Exception("Found bad element");
                            }
                            list.Add(elements.ToArray());
                        }
                        var arr = CreateRectangularArray(list);
                        var result = new FindIslands(arr).Count;
                        Console.WriteLine($"Matrix in {filename} has {result} islands");
                    }
                }
                else
                {
                    var i0 = new int[,] { };
                    var i1a = new int[,] { { 1 } };
                    var i1b = new int[,] { { 0 } };
                    var i2a = new int[,] { { 1, 1 }, { 1, 1 } };
                    var i2b = new int[,] { { 1, 0 }, { 0, 1 } };

                    var iA = new int[,] {
                        {1, 0, 1, 1, 0},
                        {0, 1, 0, 1, 0},
                        {0, 1, 0, 1, 0},
                        {0, 0, 0, 1, 0},
                        {0, 1, 0, 1, 0},
                        {0, 1, 1, 1, 0},
                    };

                    var big = new int[1000, 1000];
                    for (var i = 0u; i < big.GetLength(0); ++i)
                    {
                        for (var j = 0u; j < big.GetLength(1); ++j)
                        {
                            big[i, j] = 1;
                        }
                    }

                    // create a single 1 on an island of its own:
                    big[500, 500] = 0;
                    big[500, 502] = 0;
                    big[499, 501] = 0;
                    big[501, 501] = 0;

                    var rnd = new Random();

                    var bigRandom = new int[1000, 1000];
                    for (var i = 0u; i < bigRandom.GetLength(0); ++i)
                    {
                        for (var j = 0u; j < bigRandom.GetLength(1); ++j)
                        {
                            bigRandom[i, j] = rnd.Next() % 2;
                        }
                    }

                    using (var writer = new StreamWriter("big1.txt"))
                    {
                        for (var i = 0u; i < bigRandom.GetLength(0); ++i)
                        {
                            for (var j = 0u; j < bigRandom.GetLength(1); ++j)
                            {
                                writer.Write($"{bigRandom[i, j]} ");
                            }
                            writer.WriteLine();
                        }
                    }

                    Console.WriteLine($"i0 has {new FindIslands(i0).Count} islands.");
                    Console.WriteLine($"i1a has {new FindIslands(i1a).Count} islands.");
                    Console.WriteLine($"i1b has {new FindIslands(i1b).Count} islands.");
                    Console.WriteLine($"i2a has {new FindIslands(i2a).Count} islands.");
                    Console.WriteLine($"i2b has {new FindIslands(i2b).Count} islands.");
                    Console.WriteLine($"iA has {new FindIslands(iA).Count} islands.");
                    Console.WriteLine($"big has {new FindIslands(big).Count} islands.");
                    Console.WriteLine($"big has {new FindIslands(bigRandom).Count} islands.");
                }
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }
        }
    }
}
