using System.Collections.Generic;
using System.Text;

namespace GameBrains.Extensions.Lists
{
	// Extensions to the generic list class to give it an interface like a Deque (combination Stack/Queue).
	public static class ListExtensions
    {
		public static void Push<T>(this List<T> list, T item)
        {
            list.Add(item);
        }

        public static void Pop<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public static T Peek<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static void Enqueue<T>(this List<T> list, T item)
        {
            list.Insert(0, item);
        }

		public static bool IsEmpty<T>(this List<T> list)
		{
			return list.Count == 0;
		}

        public static T Dequeue<T>(this List<T> list)
        {
            if (list.IsEmpty())
            {
				throw new System.Exception("Trying to remove first element of empty list.");
			}

			T first = list[0];
            list.RemoveAt(0);
			return first;
        }

        public static string ToNumberedItemsString<T>(this List<T> list)
        {
	        var sb = new StringBuilder();
	        for (var i = 0; i < list.Count; i++)
	        {
		        T item = list[i];
		        sb.AppendLine(i + ": " + item);
	        }

	        return sb.ToString();
        }
	}
}