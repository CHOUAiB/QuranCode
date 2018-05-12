// Yorye Nathan on 03-May-2015
// http://stackoverflow.com/questions/30006497/find-all-k-size-subsets-with-sum-s-of-an-n-size-bag-of-duplicate-unsorted-positi/30012781#30012781
// 100 times faster than Numbers.CountSubsets

// Tests
//int size = 7;
//int sum = 313;
//Subsets found: 122131009
//00:00:30.2087779

//int size = 9;
//int sum = 911;
//Subsets found: 766732123
//00:04:08.2344704

//int size = 10;
//int sum = 1000;
//Subsets found: 6323560004
//00:33:38.4460587

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Subsets
{
    public sealed class Integer
    {
        public readonly int Value;
        public readonly int Index;

        public Integer(int value, int index)
        {
            this.Value = value;
            this.Index = index;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Index + 1, this.Value);
        }
    }
    public readonly Integer[] Numbers;

    private readonly int length;
    private readonly int[] tail;
    public Subsets(int[] numbers)
    {
        // Set length and make Numbers array
        // (to remember the original index of each value)
        this.length = numbers.Length;
        this.Numbers = new Integer[this.length];

        for (var i = 0; i < this.length; i++)
        {
            this.Numbers[i] = new Integer(numbers[i], i);
        }

        // Sort Numbers descendingly by value
        Array.Sort(this.Numbers, (a, b) => b.Value.CompareTo(a.Value));

        this.tail = new int[this.length + 1];
        for (var i = this.length - 1; i >= 0; i--)
            this.tail[i] = this.tail[i + 1] + this.Numbers[i].Value;
    }

    /// <summary>
    /// Find all subsets with value sum equals to target sum
    /// </summary>
    /// <param name="sum">target sum</param>
    /// <param name="callback">method to call everytime a new subset is found</param>
    public void Find(int sum, Action<Integer[]> callback)
    {
        for (var size = 1; size <= this.length; size++)
        {
            this.Find(size, sum, callback);
        }
    }
    /// <summary>
    /// Find all subsets with specified size with value sum equals to target sum
    /// </summary>
    /// <param name="size">size of subsets t be found</param>
    /// <param name="sum">target sum</param>
    /// <param name="callback">method to call everytime a new subset is found</param>
    public void Find(int size, int sum, Action<Integer[]> callback)
    {
        // Size of subsets t be found cannot be larger than whole set
        if (size > this.length) return;

        this.Scan(0, size, sum, new List<Integer>(), callback);
    }
    /// <summary>
    /// Search for all subsets with specified size with value sum equals to target sum
    /// </summary>
    /// <param name="startIndex">start index in whole set</param>
    /// <param name="size">size of subsets t be found</param>
    /// <param name="sum">target sum</param>
    /// <param name="subset">subset to evalate</param>
    /// <param name="callback">method to call everytime a new subset is found</param>
    private void Scan(int startIndex, int size, int sum, List<Integer> subset, Action<Integer[]> callback)
    {
        // Subset cannot be larger than the Numbers set
        if (size > this.length)
        {
            return;
        }

        // No more Numbers to add, and current subset is guranteed to be valid
        if (size == 0)
        {
            // Callback with current subset
            callback(subset.ToArray());
            return;
        }

        // Sum the smallest size values
        var minSubsetStartIndex = this.length - size;
        var minSum = this.tail[minSubsetStartIndex];

        // Smallest possible sum is greater than target sum,
        // so a valid subset cannot be found
        if (minSum > sum)
        {
            return;
        }

        // Find largest value that satisfies the condition
        // that a valid subset can be found
        minSum -= this.Numbers[minSubsetStartIndex].Value;

        // But remember the last index that satisfies the condition
        var minSubsetEndIndex = minSubsetStartIndex;

        while (minSubsetStartIndex > startIndex &&
               minSum + this.Numbers[minSubsetStartIndex - 1].Value <= sum)
        {
            minSubsetStartIndex--;
        }

        // Find the first value in the sorted whole set that is
        // the largest value we just found (in case of duplicates)
        while (minSubsetStartIndex > startIndex &&
               Numbers[minSubsetStartIndex] == Numbers[minSubsetStartIndex - 1])
        {
            minSubsetStartIndex--;
        }

        // [minSubsetStartIndex .. maxSubsetEndIndex] is the
        // full range we must check in recursion
        for (var subsetStartIndex = minSubsetStartIndex;
             subsetStartIndex <= minSubsetEndIndex;
             subsetStartIndex++)
        {
            var maxSum = this.tail[subsetStartIndex] -
                         this.tail[subsetStartIndex + size];

            // The largest possible sum is less than the target sum,
            // so a valid subset cannot be found
            if (maxSum < sum)
            {
                return;
            }

            // Add current Number to the subset
            var integer = this.Numbers[subsetStartIndex];
            subset.Add(integer);

            // Recurse through the sub-problem to the right
            this.Scan(subsetStartIndex + 1, size - 1, sum - integer.Value, subset, callback);

            // Remove current Numbers and continue loop
            subset.RemoveAt(subset.Count - 1);
        }
    }

    // Test method
    private static long count;
    private static void OnSubsetFound(Integer[] subset)
    {
        count++;
        //return; // Skip prints when speed-testing

        foreach (var integer in subset)
        {
            Console.Write(integer.ToString());
            Console.Write(" ");
        }
        Console.WriteLine();
    }
    private static void MainTest(string[] args)
    {
        // Quran verses per chapter
        int[] values = new int[]
        {
            7, 286, 200, 176, 120, 165, 206, 75, 129, 109,
            123, 111, 43, 52, 99, 128, 111, 110, 98, 135,
            112, 78, 118, 64, 77, 227, 93, 88, 69, 60,
            34, 30, 73, 54, 45, 83, 182, 88, 75, 85,
            54, 53, 89, 59, 37, 35, 38, 29, 18, 45,
            60, 49, 62, 55, 78, 96, 29, 22, 24, 13,
            14, 11, 11, 18, 12, 12, 30, 52, 52, 44,
            28, 28, 20, 56, 40, 31, 50, 40, 46, 42,
            29, 19, 36, 25, 22, 17, 19, 26, 30, 20,
            15, 21, 11, 8, 8, 19, 5, 8, 8, 11,
            11, 8, 3, 9, 5, 4, 7, 3, 6, 3,
            5, 4, 5, 6
        };
        var subsets = new Subsets(values);

        int size = 7;
        int sum = 313;
        if (args.Length == 2)
        {
            int.TryParse(args[0], out size);
            int.TryParse(args[1], out sum);
        }

        var sw = Stopwatch.StartNew();
        count = 0L;
        subsets.Find(size, sum, OnSubsetFound);
        sw.Stop();

        Console.WriteLine("Subsets found: " + count);
        Console.WriteLine(sw.Elapsed);

        Console.ReadKey();
    }
}
