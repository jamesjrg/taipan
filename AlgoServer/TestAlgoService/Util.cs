using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAlgoService
{
    public class TreeAssertData
    {
        public TreeAssertData(int[] input)
        {
            this.input = input;
        }

        public int[] input;
        public List<KeyValuePair<int, int>> predecessorTests = new List<KeyValuePair<int, int>>();
        public List<KeyValuePair<int, int>> successorTests = new List<KeyValuePair<int, int>>();
    }

    class Util
    {
        static public List<TreeAssertData> CreateTreeAssertData()
        {
            List<TreeAssertData> assertData = new List<TreeAssertData>();

            int[] input;

            //empty
            assertData.Add(new TreeAssertData(new int[0]));

            //single entry
            assertData.Add(new TreeAssertData(new int[1] { 1 }));

            //some test sequences
            input = new int[] { 1, 2, 3, 4, 5 };
            assertData.Add(new TreeAssertData(input));

            input = new int[] { 5, 4, 3, 2, 1 };
            assertData.Add(new TreeAssertData(input));

            input = new int[] { 1, 3, 2, 4, 5 };
            assertData.Add(new TreeAssertData(input));

            foreach (var data in assertData)
            {
                data.predecessorTests.Add(new KeyValuePair<int, int>(5, 4));
                data.predecessorTests.Add(new KeyValuePair<int, int>(2, 1));
                data.predecessorTests.Add(new KeyValuePair<int, int>(1, -1));

                data.successorTests.Add(new KeyValuePair<int, int>(1, 2));
                data.successorTests.Add(new KeyValuePair<int, int>(4, 5));
                data.successorTests.Add(new KeyValuePair<int, int>(5, -1));
            }

            return assertData;
        }

        public static bool ArraysEqual(int[] a1, int[] a2)
        {
            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i != a1.Length; ++i)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }

        private static string ArrayToString(int[] array)
        {
            string[] tmp = new string[array.Length];
            for (int i = 0; i != array.Length; ++i)
                tmp[i] = array[i].ToString();
            return String.Join(", ", tmp);

        }

        public static string MakeErrorMessage(int[] expected, int[] actual)
        {
            return ArrayToString(expected) + " Actual: " + ArrayToString(actual);
        }
    }
}
