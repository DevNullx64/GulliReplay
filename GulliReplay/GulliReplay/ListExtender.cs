using System;
using System.Collections.Generic;
using System.Text;

namespace Extender
{
    public static class IListExtender
    {
        public static int BinarySearchIndexOf<T>(this IList<T> list, T value) where T: IComparable<T>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + ((upper - lower) / 2);
                int comparisonResult = value.CompareTo(list[middle]);
                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return ~lower;
        }
    }
}
