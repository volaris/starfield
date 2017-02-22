using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.DataStructures
{
    /**
     * <summary>    Generic Priority Queue. </summary>
     *
     * <typeparam name="T"> Generic type parameter. </typeparam>
     */

    public class PriorityQueue<T>
    {

        private List<T> data;
        private Comparison<T> comparison;

        /**
         * <summary>    Constructor. </summary>
         *
         * <exception cref="ArgumentException"> Thrown when comparison is null and T is not an IComparable&lt;T&gt;. </exception>
         *
         * <param name="comparison">    (Optional) The comparison. </param>
         */

        public PriorityQueue(Comparison<T> comparison = null)
        {
            this.data = new List<T>();
            this.comparison = comparison;

            if(this.comparison == null)
            {
                if(typeof(T).GetInterfaces().Any(x => x == typeof(IComparable<T>)))
                {
                    this.comparison = delegate(T one, T two)
                    {
                        return ((IComparable<T>)one).CompareTo(two);
                    };
                }

                if (this.comparison == null)
                {
                    throw new ArgumentException("comparison must not be null or T must implement IComparable<T>");
                }
            }
        }

        /**
         * <summary>    Adds an object onto the end of this queue. </summary>
         *
         * <param name="item">  The item. </param>
         */

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (comparison(data[ci], data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        /**
         * <summary>    Removes the head object from this queue. </summary>
         *
         * <returns>    The head object from this queue. </returns>
         */

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            T frontItem = data[0];   // fetch the front
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && comparison(data[rc], data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (comparison(data[pi], data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        /**
         * <summary>    Returns the top-of-stack object without removing it. </summary>
         *
         * <returns>    The current top-of-stack object. </returns>
         */

        public T Peek()
        {
            T frontItem = data[0];
            return frontItem;
        }

        /**
         * <summary>    Gets the count. </summary>
         *
         * <returns>    An int. </returns>
         */

        public int Count()
        {
            return data.Count;
        }

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data[i].ToString() + " ";
            s += "count = " + data.Count;
            return s;
        }

        /**
         * <summary>    Query if this object is consistent. </summary>
         *
         * <returns>    True if consistent, false if not. </returns>
         */

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (data.Count == 0) return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && comparison(data[pi], data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && comparison(data[pi], data[rci]) > 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
    }
}
