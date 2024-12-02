using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperInnovaLib
{
    public static class iAPIExtension
    {
        public static string URLQuery(this string original, params string[] inputs)
        {
            original += "?";
            for(int i = 0; i < inputs.Length; i++)
            {
                original += (i == 0 ? "" : "&");
                original += inputs[i];
            }

            return original;
        }


        public static string Format(this string original, params string[] inputs)
        {
            return string.Format(original, inputs);
        }
    }
}
