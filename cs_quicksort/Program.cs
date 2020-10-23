using System;
using System.Collections;
using System.Linq;

namespace cs_quicksort
{
    class Program
    {
        static void Main (string[] args)
        {
            // set some testing parameters
            int size = 50000;
            int max_possible_value = 10000;
            Console.WriteLine (string.Format ("Sorting array of size {0} and max possible value of {1}", size, max_possible_value));

            // fill initial array with random values
            int[] vals = new int[size];
            Random randNum = new Random ();
            for (int i = 0; i < vals.Length; vals[i++] = randNum.Next (0, max_possible_value)) ;

            // copy it twice so we can test our algorithm against standart one
            // (one copy would actually be sufficient, but symmetry is nice)
            int[] valscopy1 = new int[size];
            vals.CopyTo (valscopy1, 0);
            int[] valscopy2 = new int[size];
            vals.CopyTo (valscopy2, 0);

            // create two comparers to count comparisons independently
            MyComparer comp1 = new MyComparer ();
            MyComparer comp2 = new MyComparer ();

            // run two sorts
            Array.Sort (valscopy1, 0, valscopy1.Length, comp1);
            MySorter.QuickSort (valscopy2, 0, valscopy2.Length, comp2);

            // print array if parameters are small enough
            if (size <= 50 && max_possible_value <= 10)
            {
                Console.WriteLine (string.Format ("Initial:  {0}", string.Join (",", vals)));
                Console.WriteLine (string.Format ("Sorted:   {0}", string.Join (",", valscopy1)));
                Console.WriteLine (string.Format ("MySorted: {0}", string.Join (",", valscopy2)));
            }

            // print comparison counts
            Console.WriteLine (string.Format ("Expected comparison count:      {0}", Math.Ceiling (size * Math.Log (size, 2))));
            Console.WriteLine (string.Format ("Standart sort comparison count: {0}", comp1.GetCompareCount ()));
            Console.WriteLine (string.Format ("My sort comparison count:       {0}", comp2.GetCompareCount ()));

            // check if we actually sorted correctly
            if (valscopy1.SequenceEqual (valscopy2))
                Console.WriteLine ("Array was sorted correctly!");
            else
                Console.WriteLine ("My sort has an error, unfortunately");
        }
    }

    // we could use IComparer<int> here and get rid of type checking
    // but lets keep it more generic
    class MyComparer : IComparer
    {
        public int Compare (object a, object b)
        {
            if (a is int @inta && b is int @intb)
                return IntCompare (@inta, @intb);
            throw new NotImplementedException ();
        }
        int IntCompare (int a, int b)
        {
            MCompareCount++;
            if (a < b)
                return -1;
            if (a > b)
                return 1;
            return 0;
        }
        public int GetCompareCount () => MCompareCount;
        int MCompareCount = 0;
    }

    class MySorter
    {
        public static void QuickSort (Array array, int index, int length, IComparer comparer)
        {
            int start = index, end = index + length;
            while (end - start > 1)
            {
                int i = start;
                int j = end - 1;

                // in case of number sorting, we could just store the pivot value itself
                // but in general case it could be undesirable to copy the element, so lets keep track of its index instead
                // we cann do i++ and j-- here since GetPivotIndex swaps edge elements to correct order
                int pivot_index = GetPivotIndex (array, i++, j--, comparer);

                while (i <= j)
                {
                    while (comparer.Compare (array.GetValue (i), array.GetValue (pivot_index)) < 0) i++;
                    while (comparer.Compare (array.GetValue (j), array.GetValue (pivot_index)) > 0) j--;

                    if (i > j)
                        continue;

                    Swap (array, i, j);

                    // we need to keep track of pivot index in case it changes here
                    if (i == pivot_index)
                        pivot_index = j;
                    else if (j == pivot_index)
                        pivot_index = i;

                    i++;
                    j--;
                }

                // choose smaller part, pass it to recursive call
                if (end - i >= j - start + 1)
                {
                    QuickSort (array, start, j - start + 1, comparer);
                    start = i;
                }
                else
                {
                    QuickSort (array, i, end - i, comparer);
                    end = j + 1;
                }

            }
        }
        static int GetPivotIndex (Array array, int left, int right, IComparer comparer)
        {
            int mid = (left + right) / 2;

            bool c1;
            bool c2;

            // since we make comparisons here anyway, might as well also do some swapping to correct order
            if ((c1 = comparer.Compare (array.GetValue (right), array.GetValue (left)) < 0))
                Swap (array, left, right);
            if ((c2 = comparer.Compare (array.GetValue (mid), array.GetValue (left)) < 0))
                Swap (array, mid, left);

            // we dont need third comparison if we know that mid < left < right
            if ((!c1 && c2))
                return mid;

            if (comparer.Compare (array.GetValue (right), array.GetValue (mid)) < 0)
                Swap (array, right, mid);

            return mid;
        }
        static void Swap (Array array, int i, int j)
        {
            object swap;
            swap = array.GetValue (i);
            array.SetValue (array.GetValue (j), i);
            array.SetValue (swap, j);
        }
    }
}
