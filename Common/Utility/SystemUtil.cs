using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemUtil {
    public static int[] AllIndexesOf(this string str, string substr, bool ignoreCase = false) {
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substr)) {
            throw new ArgumentException("String or substring is not specified.");
        }

        List<int> indexes = new List<int>();
        int index = 0;

        while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1) {
            indexes.Add(index++);
        }

        return indexes.ToArray();
    }
}