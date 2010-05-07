using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAlgoService
{
    class Util
    {
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
