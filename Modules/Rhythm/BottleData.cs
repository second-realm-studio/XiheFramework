using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleData {
    public bool autoClearBottle = false; //clear when full

    private List<Queue<int>> data = new List<Queue<int>>();

    private int debug = 0;

    public BottleData(int count) {
        for (int i = 0; i < count; i++) {
            data.Add(new Queue<int>());
        }
    }

    public void Distribute(int value) {
        if (autoClearBottle) {
            if (data[0].Count >= data.Count) {
                data.RemoveAt(0);
                data.Add(new Queue<int>());
            }
        }


        for (int i = 0; i < data.Count; i++) {
            var q = data[i];
            if (q.Count < data.Count - i) {
                //q.Enqueue(value);
                q.Enqueue(++debug);
            }
        }
    }

    public int[] GetResultArray() {
        // var length = 0;
        // for (int i = 0; i < data.Count; i++) {
        //     length += data.Count - i;
        // }

        List<int> output = new List<int>();

        for (int i = 0; i < data.Count; i++) {
            output.AddRange(data[i].ToArray());
        }

        return output.ToArray();
    }

    public int[][] GetResult2DArray() {
        List<int[]> result = new List<int[]>();
        foreach (var q in data) {
            result.Add(q.ToArray());
        }

        return result.ToArray();
    }

    /// <summary>
    /// shift left and return first bottle act like dequeue
    /// </summary>
    /// <returns></returns>
    public int[] Shift() {
        var result = data[0].ToArray();

        data.RemoveAt(0);
        data.Add(new Queue<int>());

        return result;
    }

    public int[] Peek() {
        return data[0].ToArray();
    }

    public int Count() {
        return data.Count;
    }
}