﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public enum NumberType
{
    None,                   // not a number (eg infinity, -infinity)
    Unit,                   // indivisible by anything
    Prime,                  // divisible by self only (dividing by 1 doesn't divide into smaller parts and is misleading)
    AdditivePrime,          // prime with prime digit sum
    PurePrime,              // additive prime with prime digits
    Composite,              // divisible by slef and other some prime(s)
    AdditiveComposite,      // composite with composite digit sum
    PureComposite,          // additiver composite with composite digits
    MersennePrime,          // p of prime 2^p - 1
    Fibonacci,              // n2 = n0 + n1 and n2/n1 ~= the golden ratio
    Tetrahedral,            // n*(n+1)*(n+2)/6   = 1, 4, 10, 20, 35, 56, 84, 120, 165, 220, 286, 364, 455, 560, 680, 816, 969, 1140, 1330, 1540, 1771, 2024, 2300, 2600, 2925, 3276, 3654, 4060, 4495, 4960, 5456, 5984, 6545, 7140, 7770, 8436, 9139, 9880, 10660, 11480, 12341, 13244, 14190, 15180, ...
    Pyramidal,              // n*(n+1)*(2*n+1)/6 = 1, 5, 14, 30, 55, 91, 140, 204, 285, 385, 506, 650, 819, 1015, 1240, 1496, 1785, 2109, 2470, 2870, 3311, 3795, 4324, 4900, 5525, 6201, 6930, 7714, 8555, 9455, 10416, 11440, 12529, 13685, 14910, 16206, 17575, 19019, 20540, 22140, 23821, 25585, 27434, 29370, ...
    Cubic,                  // n^3               = 1, 8, 27, 64, 125, ...
    Collatz,                // 3n+1 for odd n, n/2 for even n, continue until n = 1. Squence of numbers with increasing steps to reach n = 1 are:
    //                      // 1, 2, 3, 6, 7, 9, 18, 25, 27, 54, 73, 97, 129, 171, 231, 313, 327, 649, 703, 871, 1161, 2223, 2463, 2919, 3711, 6171, 10971, 13255, 17647, 23529, 26623, 34239, 35655, 52527, 77031, 106239, 142587, 156159, 216367, 230631, 410011, 511935, 626331, 837799, ...
    Gematria,               // 1..10..100..1000..10000
    QuranNumber,            // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 30, 40, 50, 60, 70, 80, 99, 100, 200, 300, 1000, 2000, 3000, 5000, 50000, 100000
    Any                     // any natural number from 1 to MaxValue
};


//http://en.wikipedia.org/wiki/Polygon
//----------------------------------------------------
//Ones		        Tens			Final suffix
//----------------------------------------------------
//1 hen-		    10 deca-		-gon 
//2 do-		        20 -cosa- 
//3 tri-		    30 triaconta- 
//4 tetra-	        40 tetraconta- 
//5 penta-	        50 pentaconta- 
//6 hexa-		    60 hexaconta- 
//7 hepta-	        70 heptaconta- 
//8 octa-		    80 octaconta- 
//9 ennea-/nona-	90 enneaconta-/nonaconta- 
//----------------------------------------------------
//
//Examples:
//7-gon  = hepta-                   -gon		= heptagon
//19-gon = nona-    deca-           -gon 		= nonadecagon 
//42-gon = do-      tetraconta-     -gon 		= dotetracontagon 
//50-gon =          pentaconta-     -gon		= pentacontagon 
//
//N-gon (for N > 99)			                = N-gon
//100-gon					                    = 100-gon
//----------------------------------------------------
public enum PolygonType
{
    Triangular,
    Square,
    Pentagonal,
    Hexagonal,
    Heptagonal,
    Octagonal,
    Nonagonal,
    Decagonal,
    Hendecagonal,
    Dodecagonal,
    Tridecagonal,
    Tetradecagonal,
    Pentadecagonal,
    Hexadecagonal,
    Heptadecagonal,
    Octadecagonal,
    Nonadecagonal,
    Icosagonal,
    Icosihenagonal,
    Icosidigonal,
    Icositrigonal,
    Icositetragonal
};

// https://oeis.org  Chemical polyhex hydrocarbons with 19 hexagons
public enum PolyhexType
{
    C2hPolyhexHydrocarbon,
    C2vPolyhexHydrocarbon
};

// = ≠ ≡ < ≤ > ≥
public enum ComparisonOperator { Equal, NotEqual, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual, MultipleOf, DivisibleByWithRemainder, Reserved };

// + - * / % (remainder)
public enum ArithmeticOperator { Plus, Minus, Multiply, Divide, Modulus };

public static class Numbers
{
    // pi = circumference / diameter ~= 355/113
    // e = Euler's number = 0SUM∞(1/n!)
    // phi is golden ratio = (sqrt(5)+1)/2
    public const double PI = 3.141592653589793238462643383279D;
    public const double E = 2.718281828459045235360287471352D;
    public const double PHI = 1.618033988749894848204586834365D;

    public static string NUMBERS_FOLDER = "Numbers";
    private static int s_max_number_limit = int.MaxValue / 1024;

    static Numbers()
    {
        if (!Directory.Exists(NUMBERS_FOLDER))
        {
            Directory.CreateDirectory(NUMBERS_FOLDER);
        }

        //GeneratePrimes(s_max_number_limit);
        //GenerateAdditivePrimes(s_max_number_limit);
        //GeneratePurePrimes(s_max_number_limit);
        //GenerateComposites(s_max_number_limit);
        //GenerateAdditiveComposites(s_max_number_limit);
        //GeneratePureComposites(s_max_number_limit);

        //// >20Mb files. Too big for users to download
        //SavePrimes();
        //SaveAdditivePrimes();
        //SavePurePrimes();
        //SaveComposites();
        //SaveAdditiveComposites();
        //SavePureComposites();

        //LoadPrimes();
        //LoadAdditivePrimes();
        //LoadPurePrimes();
        //LoadComposites();
        //LoadAdditiveComposites();
        //LoadPureComposites();
    }

    public static bool IsDigitsOnly(string text)
    {
        foreach (char c in text)
        {
            if (!Char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsNumberType(long number, NumberType number_type)
    {
        switch (number_type)
        {
            case NumberType.Any:
                {
                    return true;
                }
            case NumberType.Prime:
                {
                    return (IsPrime(number));
                }
            case NumberType.AdditivePrime:
                {
                    return (IsAdditivePrime(number));
                }
            case NumberType.PurePrime:
                {
                    return (IsPurePrime(number));
                }
            case NumberType.Composite:
                {
                    return (IsComposite(number));
                }
            case NumberType.AdditiveComposite:
                {
                    return (IsAdditiveComposite(number));
                }
            case NumberType.PureComposite:
                {
                    return (IsPureComposite(number));
                }
            case NumberType.None:
            default:
                {
                    return false;
                }
        }
    }
    /// <summary>
    /// Compare two numbers
    /// </summary>
    /// <param name="number1">first number</param>
    /// <param name="number2">second number</param>
    /// <param name="comparison_operator">operator for comparing the two numbers</param>
    /// <param name="remainder">remainder for the % operator. -1 means any remainder</param>
    /// <returns>returns comparison result</returns>
    public static bool Compare(long number1, long number2, ComparisonOperator comparison_operator, int remainder)
    {
        switch (comparison_operator)
        {
            case ComparisonOperator.Equal:
                {
                    return (number1 == number2);
                }
            case ComparisonOperator.NotEqual:
                {
                    return (number1 != number2);
                }
            case ComparisonOperator.LessThan:
                {
                    return (number1 < number2);
                }
            case ComparisonOperator.LessThanOrEqual:
                {
                    return (number1 <= number2);
                }
            case ComparisonOperator.GreaterThan:
                {
                    return (number1 > number2);
                }
            case ComparisonOperator.GreaterThanOrEqual:
                {
                    return (number1 >= number2);
                }
            ////////////////////////////////////////////////////////////
            // General Use
            //case ComparisonOperator.MultipleOf:
            //    {
            //        return ((number1 % number2) == 0);
            //    }
            //case ComparisonOperator.DivisibleByWithRemainder:
            //    {
            //        return ((number1 % number2) == remainder);
            //    }
            ////////////////////////////////////////////////////////////
            // QuranCode Use
            ////////////////////////////////////////////////////////////
            case ComparisonOperator.MultipleOf:
                {
                    return ((number1 > 0) && ((number1 % number2) == 0));
                }
            case ComparisonOperator.DivisibleByWithRemainder:
                {
                    if (remainder == -1) // means any remainder
                    {
                        return ((number1 % number2) != 0);
                    }
                    else
                    {
                        return ((number1 > 0) && ((number1 % number2) == remainder));
                    }
                }
            ////////////////////////////////////////////////////////////
            case ComparisonOperator.Reserved:
            default:
                {
                    return false;
                }
        }
    }
    public static bool AreConsecutive(List<int> numbers)
    {
        if (numbers != null)
        {
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                if (numbers[i + 1] != numbers[i] + 1)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    // pi = circumference / diameter ~= 355/113
    private static string s_pi_filename = "pi.txt";
    private static int[] s_pi_digits;
    public static int[] PiDigits
    {
        get
        {
            if (s_pi_digits == null)
            {
                GeneratePiDigits();
            }
            return s_pi_digits;
        }
    }
    private static void GeneratePiDigits()
    {
        string filename = NUMBERS_FOLDER + "/" + s_pi_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                //s_pi_digits = new int[s_pi_limit];
                string content = reader.ReadToEnd();
                s_pi_digits = new int[content.Length - 1];
                s_pi_digits[0] = int.Parse(content[0] + "");
                for (int i = 1; i < content.Length - 1; i++)
                {
                    s_pi_digits[i] = int.Parse(content[i + 1] + "");
                }
            }
        }
    }

    // e = Euler's number = 0SUM∞(1/n!)
    private static string s_e_filename = "e.txt";
    private static int[] s_e_digits;
    public static int[] EDigits
    {
        get
        {
            if (s_e_digits == null)
            {
                GenerateEDigits();
            }
            return s_e_digits;
        }
    }
    private static void GenerateEDigits()
    {
        string filename = NUMBERS_FOLDER + "/" + s_e_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                //s_e_digits = new int[s_e_limit];
                string content = reader.ReadToEnd();
                s_e_digits = new int[content.Length - 1];
                s_e_digits[0] = int.Parse(content[0] + "");
                for (int i = 1; i < content.Length - 1; i++)
                {
                    s_e_digits[i] = int.Parse(content[i + 1] + "");
                }
            }
        }
    }

    // phi is golden ratio = (sqrt(5)+1)/2
    private static string s_phi_filename = "phi.txt";
    private static int[] s_phi_digits;
    public static int[] PhiDigits
    {
        get
        {
            if (s_phi_digits == null)
            {
                GeneratePhiDigits();
            }
            return s_phi_digits;
        }
    }
    private static void GeneratePhiDigits()
    {
        string filename = NUMBERS_FOLDER + "/" + s_phi_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                //s_phi_digits = new int[s_phi_limit];
                string content = reader.ReadToEnd();
                s_phi_digits = new int[content.Length - 1];
                s_phi_digits[0] = int.Parse(content[0] + "");
                for (int i = 1; i < content.Length - 1; i++)
                {
                    s_phi_digits[i] = int.Parse(content[i + 1] + "");
                }
            }
        }
    }

    public static bool IsUnit(long number)
    {
        return (number == 1L);
    }
    public static bool IsUnit(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsUnit(number);
    }
    // http://digitalbush.com/2010/02/26/sieve-of-eratosthenes-in-csharp/

    //IList<int> FindPrimes(int max)
    //{
    //    var result = new List<int>((int)(max / (Math.Log(max) - 1.08366)));
    //    var maxSquareRoot = Math.Sqrt(max);
    //    var eliminated = new System.Collections.BitArray(max + 1);
    //    result.Add(2);
    //    for (int i = 3; i <= max; i += 2)
    //    {
    //        if (!eliminated[i])
    //        {
    //            if (i < maxSquareRoot)
    //            {
    //                for (int j = i * i; j <= max; j += 2 * i)
    //                    eliminated[j] = true;
    //            }
    //            result.Add(i);
    //        }
    //    }
    //    return result;
    //}

    // Algorithm Optimizations
    // I cut my work in half by treating the special case of '2'.
    // We know that 2 is prime and all even numbers thereafter are not.
    // So, we'll add two immediately and then start looping at 3 only checking odd numbers from there forward.

    // After we've found a prime, we only need to eliminate numbers from it's square and forward.
    // Let's say we want to find all prime numbers up to 100 and we've just identified 7 as a prime.
    // Per the algorithm, I'll need to eliminate 2*7, 3*7 ,4*7, 5*7, 6*7, 7*7 ,8*7 ,9*7, 10*7 ,11*7, 12*7 ,13*7 and 14*7.
    // None of the even multiples matter (even times an odd is always even) and none of the multiples
    // up to the square of the prime matter since we've already done those multiples in previous loops.
    // So really we only have to eliminate 7*7, 9*7, 11*7 and 13*7.
    // That's a 9 fewer iterations and those savings become more fruitful the deeper you go!

    // The last optimization is the square root calculation and check.
    // We know from above that we only need to start eliminating beginning at the square of the current prime.
    // Therefore it also makes sense that we can stop even trying once we get past the to square root of the max.
    // This saves a bunch more iterations.

    // Language Optimizations
    // Originally I had started by returning an IEnumerable<int>.
    // I wasn't using the list you see above and instead I was using yield return i.
    // I really like that syntax, but once I got to the VB.net version (Coming Soon!),
    // I didn't have a direct translation for the yield keyword.
    // I took the lazy route in the VB version and just stuffed it all into a list and returned that.
    // To my surprise it was faster! I went back and changed the C# version above and it performed better.
    // I'm not sure why, but I'm going with it.

    // What do you think that you get when do a sizeof(bool) in C#?
    // I was surprised to find out that my trusty booleans actually take up a whole byte instead of a single bit.
    // I speculate that there is a performance benefit that all of your types fit into a byte level offset in memory.
    // I was thrilled to find out that we have a BitArray class that is useful for situations above
    // when you need to store a lot of booleans and you need them to only take up a bit in memory.
    // I'm not sure it helped anything, but I feel better knowing I'm using the least amount of memory possible.

    // Conclusion
    // Despite the fact that I know C# really well, I'm very thrilled that I was able to learn a few things about the language.
    // Also, I'm really happy with the performance of this reference implementation.
    // On my machine (2.66 GHz Core2 Duo and 2 GB of RAM) I can find all of the primes under 1,000,000 in 19ms.
    // I think I've squeezed all I can out of this version.
    // Please let me know if you see something I missed or did wrong and I'll make adjustments.

    // EDIT: I just added one more optimization that's worth noting.
    // Instead of constructing my list with an empty constructor, I can save a several milliseconds 
    // off the larger sets by specifying a start size of the internal array structure behind the list.
    // If I set this size at or slightly above the end count of prime numbers,
    // then I avoid a lot of costly array copying as the array bounds keep getting hit.
    // It turns out that there is quite a bit of math involved in accurately predicting the number of primes underneath a given number.
    // I chose to cheat and just use Legendre's constant with the Prime Number Theorem which is close enough for my purposes.
    // I can now calculate all primes under 1,000,000 in 10ms on my machine. Neat!
    //private static List<int> GeneratePrimesUsingSieveOfEratosthenes(int limit)
    //{
    //    // guard against parameter out of range
    //    if (limit < 2)
    //    {
    //        return new List<int>();
    //    }

    //    // Legendre's constant to approximate the number of primes below N
    //    int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
    //    if (max_primes < 1)
    //    {
    //        max_primes = 1;
    //    }
    //    List<int> primes = new List<int>(max_primes);

    //    // bit array to cross out multiples of primes successively
    //    BitArray candidates = new BitArray(limit + 1, true);

    //    // add number 2 as prime
    //    primes.Add(2);
    //    // and cross out all its multiples
    //    for (int j = 2 * 2; j <= limit; j += 2)
    //    {
    //        candidates[j] = false;
    //    }

    //    // get the ceiling of sqrt of N
    //    int limit_sqrt = (int)Math.Ceiling(Math.Sqrt(limit));

    //    // start from 3 and skip even numbers
    //    // don't go beyond limit or overflow into negative
    //    for (int i = 3; (i > 0 && i <= limit); i += 2)
    //    {
    //        if (candidates[i])
    //        {
    //            // add not-crossed out candidate
    //            primes.Add(i);

    //            // upto the sqrt of N
    //            if (i <= limit_sqrt)
    //            {
    //                // and cross out non-even multiples from i*i and skip even i multiples
    //                // don't go beyond limit, or overflow into negative
    //                for (int j = i * i; (j > 0 && j <= limit); j += 2 * i)
    //                {
    //                    candidates[j] = false;
    //                }
    //            }
    //        }
    //    }

    //    return primes;
    //}
    //private static List<int> GeneratePrimesUsingDivisionTrial(int limit)
    //{
    //    // guard against parameter out of range
    //    if (limit < 2)
    //    {
    //        return new List<int>();
    //    }

    //    // Legendre's constant to approximate the number of primes below N
    //    int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
    //    if (max_primes < 1)
    //    {
    //        max_primes = 1;
    //    }
    //    List<int> primes = new List<int>(max_primes);

    //    primes.Add(2);

    //    for (int i = 3; i <= limit; i += 2)
    //    {
    //        bool is_prime = true;
    //        for (int j = 3; j <= (int)Math.Sqrt(i); j += 2)
    //        {
    //            if (i % j == 0)
    //            {
    //                is_prime = false;
    //                break;
    //            }
    //        }

    //        if (is_prime)
    //        {
    //            primes.Add(i);
    //        }
    //    }

    //    return primes;
    //}
    public static bool IsPrime(long number)
    {
        if (number <= 0)        // primes are natural numbers
            return false;

        if (number == 1L)        // 1 is the unit, indivisible
            return false;        // NOT prime

        if (number == 2L)        // 2 is the first prime
            return true;

        if (number % 2L == 0L)   // exclude other even numbers to speed up search
            return false;

        long sqrt = (long)Math.Sqrt(number);
        for (long i = 3L; i <= sqrt; i += 2L)
        {
            if ((number % i) == 0L)
            {
                return false;
            }
        }
        return true;
    }
    public static bool IsPrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsPrime(number);
    }
    public static bool IsAdditivePrime(long number)
    {
        if (IsPrime(number))
        {
            return IsPrime(DigitSum(number));
        }
        return false;
    }
    public static bool IsAdditivePrime(string value, long radix)
    {
        if (IsPrime(value, radix))
        {
            return IsPrime(DigitSum(value));
        }
        return false;
    }
    public static bool IsPurePrime(long number)
    {
        if (IsAdditivePrime(number))
        {
            return IsPrimeDigits(number);
        }
        return false;
    }
    public static bool IsPurePrime(string value, long radix)
    {
        if (IsAdditivePrime(value, radix))
        {
            return IsPrimeDigits(value);
        }
        return false;
    }
    public static bool IsComposite(long number)
    {
        if (number < 0L)         // not natural numbers
            return false;

        if (number == 0L)        // 0 is NOT composite
            return false;

        if (number == 1L)        // 1 is the unit, indivisible
            return false;        // NOT composite

        if (number == 2L)        // 2 is the first prime
            return false;

        if (number % 2L == 0L)   // even numbers are composite
            return true;

        long sqrt = (long)Math.Sqrt(number);
        for (long i = 3L; i <= sqrt; i += 2L)
        {
            if ((number % i) == 0L)
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsComposite(number);
    }
    public static bool IsAdditiveComposite(long number)
    {
        if (IsComposite(number))
        {
            return IsComposite(DigitSum(number));
        }
        return false;
    }
    public static bool IsAdditiveComposite(string value, long radix)
    {
        if (IsComposite(value, radix))
        {
            return IsComposite(DigitSum(value));
        }
        return false;
    }
    public static bool IsPureComposite(long number)
    {
        if (IsAdditiveComposite(number))
        {
            return IsCompositeDigits(number);
        }
        return false;
    }
    public static bool IsPureComposite(string value, long radix)
    {
        if (IsAdditiveComposite(value, radix))
        {
            return IsCompositeDigits(value);
        }
        return false;
    }
    /// <summary>
    /// Check if three numbers are additive primes and their L2R and R2L concatinations are additive primes too.
    /// <para>Example:</para>
    /// <para>Quran chapter The Key has:</para>
    /// <para>(7, 29, 139) are primes with primes digit sums (7=7, 2+9=11, 1+3+9=13)</para>
    /// <para>and 729139, 139297 primes with prime digit sum (1+3+9+2+9+7=31)</para>
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <param name="n3"></param>
    /// <returns></returns>
    public static bool ArePrimeTriplets(string value1, string value2, string value3, long radix)
    {
        long number1 = Radix.Decode(value1, radix);
        long number2 = Radix.Decode(value2, radix);
        long number3 = Radix.Decode(value3, radix);
        return ArePrimeTriplets(number1, number2, number3);
    }
    public static bool ArePrimeTriplets(long number1, long number2, long number3)
    {
        if (
            IsAdditivePrime(number1)
            &&
            IsAdditivePrime(number2)
            &&
            IsAdditivePrime(number3)
            )
        {
            try
            {
                long l2r = long.Parse(number1.ToString() + number2.ToString() + number3.ToString());
                long r2l = long.Parse(number3.ToString() + number2.ToString() + number1.ToString());
                if (
                    IsAdditivePrime(l2r)
                    &&
                    IsAdditivePrime(r2l)
                    )
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    public static int GetDigitValue(char c)
    {
        int result = -1;
        if (Char.IsDigit(c)) // 0..9
        {
            result = (int)char.GetNumericValue(c);
        }
        else // A..Z
        {
            result = c.CompareTo('A') + 10;
        }
        return result;
    }
    public static List<int> GetDigits(long number)
    {
        List<int> result = new List<int>();
        string str = number.ToString();
        for (int i = 0; i < str.Length; i++)
        {
            result.Add((int)Char.GetNumericValue(str[i]));
        }
        return result;
    }
    public static List<char> GetDigits(string value)
    {
        List<char> result = new List<char>();
        for (int i = 0; i < value.Length; i++)
        {
            result.Add(value[i]);
        }
        return result;
    }
    public static int DigitCount(long number)
    {
        return DigitCount(number.ToString());
    }
    public static int DigitCount(string value)
    {
        return ((value.StartsWith("-")) ? value.Length - 1 : value.Length);
    }
    public static int DigitSum(long number)
    {
        return DigitSum(number.ToString());
    }
    public static int DigitSum(string value)
    {
        int result = 0;
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (Char.IsDigit(c))
                {
                    result += GetDigitValue(c);
                }
            }
        }
        return result;
    }
    public static int DigitalRoot(long number)
    {
        return DigitalRoot(number.ToString());
    }
    public static int DigitalRoot(string value)
    {
        int result = DigitSum(value);
        while (result.ToString().Length > 1)
        {
            result = DigitSum(result);
        }
        return result;
    }
    public static bool IsPrimeDigits(long number)
    {
        return IsPrimeDigits(number.ToString());
    }
    public static bool IsPrimeDigits(string value)
    {
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '-') continue;
                long digit = GetDigitValue(c);
                if (!IsPrime(digit))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    public static bool IsCompositeDigits(long number)
    {
        return IsCompositeDigits(number.ToString());
    }
    public static bool IsCompositeDigits(string value)
    {
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '-') continue;
                long digit = GetDigitValue(c);
                if (!IsComposite(digit))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    private static List<long> s_primes = null;
    private static List<long> s_additive_primes = null;
    private static List<long> s_pure_primes = null;
    public static List<long> Primes
    {
        get
        {
            if (s_primes == null)
            {
                GeneratePrimes(s_max_number_limit);
            }
            return s_primes;
        }
    }
    public static List<long> AdditivePrimes
    {
        get
        {
            if (s_additive_primes == null)
            {
                GenerateAdditivePrimes(s_max_number_limit);
            }
            return s_additive_primes;
        }
    }
    public static List<long> PurePrimes
    {
        get
        {
            if (s_pure_primes == null)
            {
                GeneratePurePrimes(s_max_number_limit);
            }
            return s_pure_primes;
        }
    }
    public static int IndexOfPrime(long number)
    {
        if (IsPrime(number))
        {
            if (s_primes == null)
            {
                GeneratePrimes(s_max_number_limit);
            }
            return BinarySearch(s_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GeneratePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfAdditivePrime(long number)
    {
        if (IsAdditivePrime(number))
        {
            if (s_additive_primes == null)
            {
                GenerateAdditivePrimes(s_max_number_limit);
            }
            return BinarySearch(s_additive_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_additive_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateAdditivePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfPurePrime(long number)
    {
        if (IsPurePrime(number))
        {
            if (s_pure_primes == null)
            {
                GeneratePurePrimes(s_max_number_limit);
            }
            return BinarySearch(s_pure_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_pure_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GeneratePurePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfPrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfPrime(number);
    }
    public static int IndexOfAdditivePrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfAdditivePrime(number);
    }
    public static int IndexOfPurePrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfPurePrime(number);
    }
    private static void GeneratePrimes(int max)
    {
        //if (s_primes != null)
        //{
        //    int primes_upto_max = (int)(max / (Math.Log(max) + 1));
        //    if (s_primes.Count >= primes_upto_max)
        //    {
        //        return; // we already have a large list, no need to RE-generate new one
        //    }
        //}

        if (s_primes == null)
        {
            BitArray composites = new BitArray(max + 1);

            s_primes = new List<long>();

            s_primes.Add(1L);
            s_primes.Add(2L);

            // process odd numbers // 3, 5, 7, 9, 11, ..., max
            long sqrt = (long)Math.Sqrt(max) + 1L;
            for (int i = 3; i <= max; i += 2)
            {
                if (!composites[i])
                {
                    s_primes.Add(i);

                    // mark off multiples of i starting from i*i and skipping even "i"s
                    if (i < sqrt)
                    {
                        for (int j = i * i; j <= max; j += 2 * i)
                        {
                            composites[j] = true;
                        }
                    }
                }
            }
        }
    }
    private static void GenerateAdditivePrimes(int max)
    {
        //// re-generate for new max if larger
        //GeneratePrimes(max);

        if (s_additive_primes == null)
        {
            if (s_primes == null)
            {
                GeneratePrimes(max);
            }

            if (s_primes != null)
            {
                s_additive_primes = new List<long>();
                int count = s_primes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsPrime(DigitSum(s_primes[i])))
                    {
                        s_additive_primes.Add(s_primes[i]);
                    }
                }
            }
        }
    }
    private static void GeneratePurePrimes(int max)
    {
        //// re-generate for new max if larger
        //GenerateAdditivePrimes(max);

        if (s_pure_primes == null)
        {
            if (s_additive_primes == null)
            {
                GenerateAdditivePrimes(max);
            }

            if (s_additive_primes != null)
            {
                s_pure_primes = new List<long>();
                int count = s_additive_primes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsPrimeDigits(s_additive_primes[i]))
                    {
                        s_pure_primes.Add(s_additive_primes[i]);
                    }
                }
            }
        }
    }
    private static string s_primes_filename = "primes.txt";
    private static string s_additive_primes_filename = "additiveprimes.txt";
    private static string s_pure_primes_filename = "pureprimes.txt";
    private static void LoadPrimes()
    {
        s_primes = new List<long>();

        string filename = NUMBERS_FOLDER + "/" + s_primes_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line = "";
                while (!reader.EndOfStream)
                {
                    try
                    {
                        line = reader.ReadLine();
                        s_primes.Add(long.Parse(line));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(line + " is not a long in " + filename + "\r\n" + ex.Message);
                    }
                }
            }
        }
    }
    private static void LoadAdditivePrimes()
    {
        s_additive_primes = new List<long>();

        string filename = NUMBERS_FOLDER + "/" + s_additive_primes_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line = "";
                while (!reader.EndOfStream)
                {
                    try
                    {
                        line = reader.ReadLine();
                        s_additive_primes.Add(long.Parse(line));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(line + " is not a long in " + filename + "\r\n" + ex.Message);
                    }
                }
            }
        }
    }
    private static void LoadPurePrimes()
    {
        s_pure_primes = new List<long>();

        string filename = NUMBERS_FOLDER + "/" + s_pure_primes_filename;
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line = "";
                while (!reader.EndOfStream)
                {
                    try
                    {
                        line = reader.ReadLine();
                        s_pure_primes.Add(long.Parse(line));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(line + " is not a long in " + filename + "\r\n" + ex.Message);
                    }
                }
            }
        }
    }
    private static void SavePrimes()
    {
        if (s_primes != null)
        {
            string filename = NUMBERS_FOLDER + "/" + s_primes_filename;
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (long number in s_primes)
                {
                    writer.WriteLine(number);
                }
            }
        }
    }
    private static void SaveAdditivePrimes()
    {
        if (s_additive_primes != null)
        {
            string filename = NUMBERS_FOLDER + "/" + s_additive_primes_filename;
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (long number in s_additive_primes)
                {
                    writer.WriteLine(number);
                }
            }
        }
    }
    private static void SavePurePrimes()
    {
        if (s_pure_primes != null)
        {
            string filename = NUMBERS_FOLDER + "/" + s_pure_primes_filename;
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (long number in s_pure_primes)
                {
                    writer.WriteLine(number);
                }
            }
        }
    }
    public static List<int> SieveOfEratosthenes(int limit)
    {
        // guard against parameter out of range
        if ((limit < 2) || (limit > (int)(int.MaxValue * 0.9999999)))
        {
            return new List<int>();
        }

        // Legendre's constant to approximate the number of primes below N
        int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
        if (max_primes < 1)
        {
            max_primes = 1;
        }
        List<int> primes = new List<int>(max_primes);

        // bit array to cross out multiples of primes successively
        // from N^2, jumping 2N at a time (to skip even multiples)
        BitArray candidates = new BitArray(limit + 1, true);

        // add number 2 as prime
        primes.Add(2);
        //// no need to cross out evens as we are skipping them anyway
        //// and cross out all its multiples
        //for (int j = 2 * 2; j <= limit; j += 2)
        //{
        //    candidates[j] = false;
        //}

        // get the ceiling of sqrt of N
        int sqrt_of_limit = (int)Math.Ceiling(Math.Sqrt(limit));

        // start from 3 and skip even numbers
        // don't go beyond limit or overflow into negative
        for (int i = 3; (i > 0 && i <= limit); i += 2)
        {
            // if not-crossed out candidate yet
            if (candidates[i])
            {
                // add candidate
                primes.Add(i);

                // upto the sqrt of N
                if (i <= sqrt_of_limit)
                {
                    // and cross out non-even multiples from i*i and skip even i multiples
                    // don't go beyond limit, or overflow into negative
                    for (int j = i * i; (j > 0 && j <= limit); j += 2 * i)
                    {
                        candidates[j] = false;
                    }
                }
            }
        }
        return primes;
    }

    private static List<long> s_composites = null;
    private static List<long> s_additive_composites = null;
    private static List<long> s_pure_composites = null;
    public static List<long> Composites
    {
        get
        {
            if (s_composites == null)
            {
                GenerateComposites(s_max_number_limit);
            }
            return s_composites;
        }
    }
    public static List<long> AdditiveComposites
    {
        get
        {
            if (s_additive_composites == null)
            {
                GenerateAdditiveComposites(s_max_number_limit);
            }
            return s_additive_composites;
        }
    }
    public static List<long> PureComposites
    {
        get
        {
            if (s_pure_composites == null)
            {
                GeneratePureComposites(s_max_number_limit);
            }
            return s_pure_composites;
        }
    }
    public static int IndexOfComposite(long number)
    {
        if (IsComposite(number))
        {
            int max = s_max_number_limit;
            if (s_composites == null)
            {
                GenerateComposites(max);
            }
            return BinarySearch(s_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfAdditiveComposite(long number)
    {
        if (IsAdditiveComposite(number))
        {
            int max = s_max_number_limit;
            if (s_additive_composites == null)
            {
                GenerateAdditiveComposites(max);
            }
            return BinarySearch(s_additive_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_additive_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateAdditiveComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfPureComposite(long number)
    {
        if (IsPureComposite(number))
        {
            int max = s_max_number_limit;
            if (s_pure_composites == null)
            {
                GeneratePureComposites(max);
            }
            return BinarySearch(s_pure_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_pure_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GeneratePureComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int IndexOfComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfComposite(number);
    }
    public static int IndexOfAdditiveComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfAdditiveComposite(number);
    }
    public static int IndexOfPureComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IndexOfPureComposite(number);
    }
    private static void GenerateComposites(int max)
    {
        //if (s_composites != null)
        //{
        //    int primes_upto_max = (int)(max / (Math.Log(max) + 1));
        //    if (s_composites.Count >= (max - primes_upto_max))
        //    {
        //        return; // we already have a large list, no need to RE-generate new one
        //    }
        //}

        if (s_composites == null)
        {
            BitArray composites = new BitArray(max + 1);

            s_composites = new List<long>(max);

            for (int i = 4; i <= max; i += 2)
            {
                composites[i] = true;
            }

            // process odd numbers // 3, 5, 7, 9, 11, ..., max
            long sqrt = (long)Math.Sqrt(max) + 1L;
            for (int i = 3; i <= max; i += 2)
            {
                if (!composites[i])
                {
                    // mark off multiples of i
                    if (i <= sqrt)
                    {
                        for (int j = i * i; j <= max; j += 2 * i)
                        {
                            composites[j] = true;
                        }
                    }
                }
            }

            for (int i = 4; i <= max; i++)
            {
                if (composites[i])
                {
                    s_composites.Add(i);
                }
            }
        }
    }
    private static void GenerateAdditiveComposites(int max)
    {
        //// re-generate for new max if larger
        //GenerateComposites(max);

        if (s_additive_composites == null)
        {
            if (s_composites == null)
            {
                GenerateComposites(max);
            }

            if (s_composites != null)
            {
                s_additive_composites = new List<long>();
                int count = s_composites.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsComposite(DigitSum(s_composites[i])))
                    {
                        s_additive_composites.Add(s_composites[i]);
                    }
                }
            }
        }
    }
    private static void GeneratePureComposites(int max)
    {
        //// re-generate for new max if larger
        //GenerateAdditiveComposites(max);

        if (s_pure_composites == null)
        {
            if (s_additive_composites == null)
            {
                GenerateAdditiveComposites(max);
            }

            if (s_additive_composites != null)
            {
                s_pure_composites = new List<long>();
                int count = s_additive_composites.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsCompositeDigits(s_additive_composites[i]))
                    {
                        s_pure_composites.Add(s_additive_composites[i]);
                    }
                }
            }
        }
    }

    private static List<long> s_mersenne_primes;
    public static List<long> MersennePrimes
    {
        get
        {
            if (s_mersenne_primes == null)
            {
                GenerateMersennePrimes();
            }
            return s_mersenne_primes;
        }
    }
    private static void GenerateMersennePrimes()
    {
        s_mersenne_primes = new List<long>() 
            { 
              1, 2, 3, 5, 7, 13, 17, 19, 31, 61, 89, 107, 127, 521, 607, 1279, 2203, 2281, 3217,
              4253, 4423, 9689, 9941, 11213, 19937, 21701, 23209, 44497, 86243, 110503, 132049,
              216091, 756839, 859433, 1257787, 1398269, 2976221, 3021377, 6972593, 13466917,
              20996011, 24036583, 25964951, 30402457, 32582657, 37156667, 42643801, 43112609, 57885161
            };
    }
    /// <summary>
    /// <para>Mersenne Prime is 2^p - 1 for some prime p exponent power</para>
    /// <para>the first 47 known Mersenne powers in 2012 are:</para>
    /// <para>2, 3, 5, 7, 13, 17, 19, 31, 61, 89, 107, 127, 521, 607, 1279, 2203, 2281, 3217,</para>
    /// <para>4253, 4423, 9689, 9941, 11213, 19937, 21701, 23209, 44497, 86243, 110503, 132049,</para>
    /// <para>216091, 756839, 859433, 1257787, 1398269, 2976221, 3021377, 6972593, 13466917,</para>
    /// <para>20996011, 24036583, 25964951, 30402457, 32582657, 37156667, 42643801, 43112609, 57885161</para>
    /// </summary>
    /// <param name="number"></param>
    /// <returns>bool</returns>
    public static bool IsMersennePrime(long number)
    {
        return MersennePrimes.Contains(number);
    }

    private static int s_fibonaccis_limit = 1024;
    private static List<long> s_fibonaccis;
    public static List<long> Fibonaccis
    {
        get
        {
            if (s_fibonaccis == null)
            {
                GenerateFibonaccis();
            }
            return s_fibonaccis;
        }
    }
    public static int IndexOfFibonacci(long number)
    {
        if (s_fibonaccis == null)
        {
            GenerateFibonaccis();
        }
        return BinarySearch(s_fibonaccis, number);
    }
    private static void GenerateFibonaccis()
    {
        int max = s_fibonaccis_limit;
        s_fibonaccis = new List<long>((int)(max));
        s_fibonaccis.Add(1); // 0th item
        s_fibonaccis.Add(1); // 1st item
        for (int i = 2; i < max; i++)
        {
            long number = s_fibonaccis[i - 1] + s_fibonaccis[i - 2];
            s_fibonaccis.Add(number);
        }
    }
    public static bool IsFibonacci(long number)
    {
        return Fibonaccis.Contains(number);
    }

    private static int s_series_limit = 100;
    //http://en.wikipedia.org/wiki/Polygon_number
    //ith number of Polygon(sides=N) = ( (N - 2)*i*i - (N - 4)*i ) / 2
    //----------------------------------------------------------------------------------------
    //N   Name            Formula            i = 1 2 3 4 5 6 7 8 9 10             OEIS number
    //----------------------------------------------------------------------------------------
    //3   Triangular      ½(1n² + 1n)         1 3 6 10 15 21 28 36 45 55           A000217 
    //4   Square          ½(2n² - 0n)         1 4 9 16 25 36 49 64 81 100          A000290 
    //5   Pentagonal      ½(3n² - 1n)         1 5 12 22 35 51 70 92 117 145        A000326 
    //6   Hexagonal       ½(4n² - 2n)         1 6 15 28 45 66 91 120 153 190       A000384 
    //7   Heptagonal      ½(5n² - 3n)         1 7 18 34 55 81 112 148 189 235      A000566 
    //8   Octagonal       ½(6n² - 4n)         1 8 21 40 65 96 133 176 225 280      A000567 
    //9   Nonagonal       ½(7n² - 5n)         1 9 24 46 75 111 154 204 261 325     A001106 
    //10  Decagonal       ½(8n² - 6n)         1 10 27 52 85 126 175 232 297 370    A001107 
    //11  Hendecagonal    ½(9n² - 7n)         1 11 30 58 95 141 196 260 333 415    A051682 
    //12  Dodecagonal     ½(10n² - 8n)        1 12 33 64 105 156 217 288 369 460   A051624 
    //13  Tridecagonal    ½(11n² - 9n)        1 13 36 70 115 171 238 316 405 505   A051865 
    //14  Tetradecagonal  ½(12n² - 10n)       1 14 39 76 125 186 259 344 441 550   A051866 
    //15  Pentadecagonal  ½(13n² - 11n)       1 15 42 82 135 201 280 372 477 595   A051867 
    //16  Hexadecagonal   ½(14n² - 12n)       1 16 45 88 145 216 301 400 513 640   A051868 
    //17  Heptadecagonal  ½(15n² - 13n)       1 17 48 94 155 231 322 428 549 685   A051869 
    //18  Octadecagonal   ½(16n² - 14n)       1 18 51 100 165 246 343 456 585 730  A051870 
    //19  Nonadecagonal   ½(17n² - 15n)       1 19 54 106 175 261 364 484 621 775  A051871 
    //20  Icosagonal      ½(18n² - 16n)       1 20 57 112 185 276 385 512 657 820  A051872 
    //21  Icosihenagonal  ½(19n² - 17n)       1 21 60 118 195 291 406 540 693 865  A051873 
    //22  Icosidigonal    ½(20n² - 18n)       1 22 63 124 205 306 427 568 729 910  A051874 
    //23  Icositrigonal   ½(21n² - 19n)       1 23 66 130 215 321 448 596 765 955  A051875 
    //24  Icositetragonal ½(22n² - 20n)       1 24 69 136 225 336 469 624 801 1000 A051876 
    //----------------------------------------------------------------------------------------
    private static Dictionary<int, List<long>> s_polygon_numbers_dictionary = new Dictionary<int, List<long>>();
    public static List<long> PolygonNumbers(int sides)
    {
        if (!s_polygon_numbers_dictionary.ContainsKey(sides))
        {
            GeneratePolygonNumbers(sides);
        }

        if (s_polygon_numbers_dictionary.ContainsKey(sides))
        {
            return s_polygon_numbers_dictionary[sides];
        }
        else
        {
            return null;
        }
    }
    private static void GeneratePolygonNumbers(int sides)
    {
        List<long> polygon_numbers = new List<long>(s_series_limit);
        for (int n = 1; n <= s_series_limit; n++)
        {
            long number = ((sides - 2) * n * n - (sides - 4) * n) / 2L;
            polygon_numbers.Add(number);
        }
        s_polygon_numbers_dictionary.Add(sides, polygon_numbers);
    }
    public static bool IsPolygonNumber(int sides, long number)
    {
        if (s_polygon_numbers_dictionary.ContainsKey(sides))
        {
            return (s_polygon_numbers_dictionary[sides].Contains(number));
        }
        else
        {
            return false;
        }
    }
    public static List<long> Triangulars
    {
        get
        {
            return PolygonNumbers(3);
        }
    }
    public static List<long> Squares
    {
        get
        {
            return PolygonNumbers(4);
        }
    }
    public static List<long> Pentagonals
    {
        get
        {
            return PolygonNumbers(5);
        }
    }
    public static List<long> Hexagonals
    {
        get
        {
            return PolygonNumbers(6);
        }
    }
    public static List<long> Heptagonals
    {
        get
        {
            return PolygonNumbers(7);
        }
    }
    public static List<long> Octagonals
    {
        get
        {
            return PolygonNumbers(8);
        }
    }
    public static List<long> Nonagonals
    {
        get
        {
            return PolygonNumbers(9);
        }
    }
    public static List<long> Decagonals
    {
        get
        {
            return PolygonNumbers(10);
        }
    }
    public static List<long> Hendecagonals
    {
        get
        {
            return PolygonNumbers(11);
        }
    }
    public static List<long> Dodecagonals
    {
        get
        {
            return PolygonNumbers(12);
        }
    }
    public static List<long> Tridecagonals
    {
        get
        {
            return PolygonNumbers(13);
        }
    }
    public static List<long> Tetradecagonals
    {
        get
        {
            return PolygonNumbers(14);
        }
    }
    public static List<long> Pentadecagonals
    {
        get
        {
            return PolygonNumbers(15);
        }
    }
    public static List<long> Hexadecagonals
    {
        get
        {
            return PolygonNumbers(16);
        }
    }
    public static List<long> Heptadecagonals
    {
        get
        {
            return PolygonNumbers(17);
        }
    }
    public static List<long> Octadecagonals
    {
        get
        {
            return PolygonNumbers(18);
        }
    }
    public static List<long> Nonadecagonals
    {
        get
        {
            return PolygonNumbers(19);
        }
    }
    public static List<long> Icosagonals
    {
        get
        {
            return PolygonNumbers(20);
        }
    }
    public static List<long> Icosihenagonals
    {
        get
        {
            return PolygonNumbers(21);
        }
    }
    public static List<long> Icosidigonals
    {
        get
        {
            return PolygonNumbers(22);
        }
    }
    public static List<long> Icositrigonals
    {
        get
        {
            return PolygonNumbers(23);
        }
    }
    public static List<long> Icositetragonals
    {
        get
        {
            return PolygonNumbers(24);
        }
    }
    public static bool IsTriangular(long number)
    {
        return (PolygonNumbers(3).Contains(number));
    }
    public static bool IsSquare(long number)
    {
        return (PolygonNumbers(4).Contains(number));
    }
    public static bool IsPentagonal(long number)
    {
        return (PolygonNumbers(5).Contains(number));
    }
    public static bool IsHexagonal(long number)
    {
        return (PolygonNumbers(6).Contains(number));
    }
    public static bool IsHeptagonal(long number)
    {
        return (PolygonNumbers(7).Contains(number));
    }
    public static bool IsOctagonal(long number)
    {
        return (PolygonNumbers(8).Contains(number));
    }
    public static bool IsNonagonal(long number)
    {
        return (PolygonNumbers(9).Contains(number));
    }
    public static bool IsDecagonal(long number)
    {
        return (PolygonNumbers(10).Contains(number));
    }
    public static bool IsHendecagonal(long number)
    {
        return (PolygonNumbers(11).Contains(number));
    }
    public static bool IsDodecagonal(long number)
    {
        return (PolygonNumbers(12).Contains(number));
    }
    public static bool IsTridecagonal(long number)
    {
        return (PolygonNumbers(13).Contains(number));
    }
    public static bool IsTetradecagonal(long number)
    {
        return (PolygonNumbers(14).Contains(number));
    }
    public static bool IsPentadecagonal(long number)
    {
        return (PolygonNumbers(15).Contains(number));
    }
    public static bool IsHexadecagonal(long number)
    {
        return (PolygonNumbers(16).Contains(number));
    }
    public static bool IsHeptadecagonal(long number)
    {
        return (PolygonNumbers(17).Contains(number));
    }
    public static bool IsOctadecagonal(long number)
    {
        return (PolygonNumbers(18).Contains(number));
    }
    public static bool IsNonadecagonal(long number)
    {
        return (PolygonNumbers(19).Contains(number));
    }
    public static bool IsIcosagonal(long number)
    {
        return (PolygonNumbers(20).Contains(number));
    }
    public static bool IsIcosihenagonal(long number)
    {
        return (PolygonNumbers(21).Contains(number));
    }
    public static bool IsIcosidigonal(long number)
    {
        return (PolygonNumbers(22).Contains(number));
    }
    public static bool IsIcositrigonal(long number)
    {
        return (PolygonNumbers(23).Contains(number));
    }
    public static bool IsIcositetragonal(long number)
    {
        return (PolygonNumbers(24).Contains(number));
    }

    //http://en.wikipedia.org/wiki/CenteredPolygonal_number
    // ith number of CenteredPolygon(sides=N) = (((N * i)/2) * (i-1)) + 1
    // Whereas a prime number p cannot be a polygon number, many centered polygon numbers are primes.
    private static Dictionary<int, List<long>> s_centered_polygon_numbers_dictionary = new Dictionary<int, List<long>>();
    public static List<long> CenteredPolygonNumbers(int sides)
    {
        if (!s_centered_polygon_numbers_dictionary.ContainsKey(sides))
        {
            GenerateCenteredPolygonNumbers(sides);
        }

        if (s_centered_polygon_numbers_dictionary.ContainsKey(sides))
        {
            return s_centered_polygon_numbers_dictionary[sides];
        }
        else
        {
            return null;
        }
    }
    private static void GenerateCenteredPolygonNumbers(int sides)
    {
        List<long> polygon_numbers = new List<long>(s_series_limit);
        for (int n = 1; n <= s_series_limit; n++)
        {
            long number = (int)(((sides * n) / 2.0D) * (n - 1)) + 1L;
            polygon_numbers.Add(number);
        }
        s_centered_polygon_numbers_dictionary.Add(sides, polygon_numbers);
    }
    public static bool IsCenteredPolygonNumber(int sides, long number)
    {
        if (s_centered_polygon_numbers_dictionary.ContainsKey(sides))
        {
            return (s_centered_polygon_numbers_dictionary[sides].Contains(number));
        }
        else
        {
            return false;
        }
    }
    public static List<long> CenteredTriangulars
    {
        get
        {
            return CenteredPolygonNumbers(3);
        }
    }
    public static List<long> CenteredSquares
    {
        get
        {
            return CenteredPolygonNumbers(4);
        }
    }
    public static List<long> CenteredPentagonals
    {
        get
        {
            return CenteredPolygonNumbers(5);
        }
    }
    public static List<long> CenteredHexagonals
    {
        get
        {
            return CenteredPolygonNumbers(6);
        }
    }
    public static List<long> CenteredHeptagonals
    {
        get
        {
            return CenteredPolygonNumbers(7);
        }
    }
    public static List<long> CenteredOctagonals
    {
        get
        {
            return CenteredPolygonNumbers(8);
        }
    }
    public static List<long> CenteredNonagonals
    {
        get
        {
            return CenteredPolygonNumbers(9);
        }
    }
    public static List<long> CenteredDecagonals
    {
        get
        {
            return CenteredPolygonNumbers(10);
        }
    }
    public static List<long> CenteredHendecagonals
    {
        get
        {
            return CenteredPolygonNumbers(11);
        }
    }
    public static List<long> CenteredDodecagonals
    {
        get
        {
            return CenteredPolygonNumbers(12);
        }
    }
    public static List<long> CenteredTridecagonals
    {
        get
        {
            return CenteredPolygonNumbers(13);
        }
    }
    public static List<long> CenteredTetradecagonals
    {
        get
        {
            return CenteredPolygonNumbers(14);
        }
    }
    public static List<long> CenteredPentadecagonals
    {
        get
        {
            return CenteredPolygonNumbers(15);
        }
    }
    public static List<long> CenteredHexadecagonals
    {
        get
        {
            return CenteredPolygonNumbers(16);
        }
    }
    public static List<long> CenteredHeptadecagonals
    {
        get
        {
            return CenteredPolygonNumbers(17);
        }
    }
    public static List<long> CenteredOctadecagonals
    {
        get
        {
            return CenteredPolygonNumbers(18);
        }
    }
    public static List<long> CenteredNonadecagonals
    {
        get
        {
            return CenteredPolygonNumbers(19);
        }
    }
    public static List<long> CenteredIcosagonals
    {
        get
        {
            return CenteredPolygonNumbers(20);
        }
    }
    public static List<long> CenteredIcosihenagonals
    {
        get
        {
            return CenteredPolygonNumbers(21);
        }
    }
    public static List<long> CenteredIcosidigonals
    {
        get
        {
            return CenteredPolygonNumbers(22);
        }
    }
    public static List<long> CenteredIcositrigonals
    {
        get
        {
            return CenteredPolygonNumbers(23);
        }
    }
    public static List<long> CenteredIcositetragonals
    {
        get
        {
            return CenteredPolygonNumbers(24);
        }
    }
    public static bool IsCenteredTriangular(long number)
    {
        return (CenteredPolygonNumbers(3).Contains(number));
    }
    public static bool IsCenteredSquare(long number)
    {
        return (CenteredPolygonNumbers(4).Contains(number));
    }
    public static bool IsCenteredPentagonal(long number)
    {
        return (CenteredPolygonNumbers(5).Contains(number));
    }
    public static bool IsCenteredHexagonal(long number)
    {
        return (CenteredPolygonNumbers(6).Contains(number));
    }
    public static bool IsCenteredHeptagonal(long number)
    {
        return (CenteredPolygonNumbers(7).Contains(number));
    }
    public static bool IsCenteredOctagonal(long number)
    {
        return (CenteredPolygonNumbers(8).Contains(number));
    }
    public static bool IsCenteredNonagonal(long number)
    {
        return (CenteredPolygonNumbers(9).Contains(number));
    }
    public static bool IsCenteredDecagonal(long number)
    {
        return (CenteredPolygonNumbers(10).Contains(number));
    }
    public static bool IsCenteredHendecagonal(long number)
    {
        return (CenteredPolygonNumbers(11).Contains(number));
    }
    public static bool IsCenteredDodecagonal(long number)
    {
        return (CenteredPolygonNumbers(12).Contains(number));
    }
    public static bool IsCenteredTridecagonal(long number)
    {
        return (CenteredPolygonNumbers(13).Contains(number));
    }
    public static bool IsCenteredTetradecagonal(long number)
    {
        return (CenteredPolygonNumbers(14).Contains(number));
    }
    public static bool IsCenteredPentadecagonal(long number)
    {
        return (CenteredPolygonNumbers(15).Contains(number));
    }
    public static bool IsCenteredHexadecagonal(long number)
    {
        return (CenteredPolygonNumbers(16).Contains(number));
    }
    public static bool IsCenteredHeptadecagonal(long number)
    {
        return (CenteredPolygonNumbers(17).Contains(number));
    }
    public static bool IsCenteredOctadecagonal(long number)
    {
        return (CenteredPolygonNumbers(18).Contains(number));
    }
    public static bool IsCenteredNonadecagonal(long number)
    {
        return (CenteredPolygonNumbers(19).Contains(number));
    }
    public static bool IsCenteredIcosagonal(long number)
    {
        return (CenteredPolygonNumbers(20).Contains(number));
    }
    public static bool IsCenteredIcosihenagonal(long number)
    {
        return (CenteredPolygonNumbers(21).Contains(number));
    }
    public static bool IsCenteredIcosidigonal(long number)
    {
        return (CenteredPolygonNumbers(22).Contains(number));
    }
    public static bool IsCenteredIcositrigonal(long number)
    {
        return (CenteredPolygonNumbers(23).Contains(number));
    }
    public static bool IsCenteredIcositetragonal(long number)
    {
        return (CenteredPolygonNumbers(24).Contains(number));
    }

    //http://en.wikipedia.org/wiki/Platonic_solid
    //http://en.wikipedia.org/wiki/Centered_polyhedral_number
    // 4 Faces: Centered tetrahedral  numbers
    // 6 Faces: Centered hexahedron   numbers / Centered cube numbers
    // 8 Faces: Centered octahedral   numbers
    //12 Faces: Centered dodecahedral numbers
    //20 Faces: Centered icosahedral  numbers

    //Centered Tetrahedral Numbers
    //http://en.wikipedia.org/wiki/Centered_tetrahedral_number
    //http://oeis.org/A005894
    //(2*n+1)*(n^2+n+3)/3
    //1, 5, 15, 35, 69, 121, 195, 295, 425, 589, 791, 1035, 1325, 1665, 2059, 2511, 3025, 3605, 4255, 4979, 5781, 6665, 7635, 8695, 9849, 11101, 12455, 13915, 15485, 17169, 18971, 20895, 22945, 25125, 27439, 29891, 32485, 35225, 38115, ...

    //Centered Hexahedron Numbers / Centered Cube Numbers
    //http://en.wikipedia.org/wiki/Centered_cube_number
    //http://oeis.org/A005898
    //n^3 + (n+1)^3
    //1, 9, 35, 91, 189, 341, 559, 855, 1241, 1729, 2331, 3059, 3925, 4941, 6119, 7471, 9009, 10745, 12691, 14859, 17261, 19909, 22815, 25991, 29449, 33201, 37259, 41635, 46341, 51389, 56791, 62559, 68705, 75241, 82179, 89531, 97309, 105525, ...

    //Centered Octahedral Numbers
    //http://en.wikipedia.org/wiki/Centered_octahedral_number
    //http://oeis.org/A001845
    //(2*n+1)*(2*n^2 + 2*n + 3)/3
    //1, 7, 25, 63, 129, 231, 377, 575, 833, 1159, 1561, 2047, 2625, 3303, 4089, 4991, 6017, 7175, 8473, 9919, 11521, 13287, 15225, 17343, 19649, 22151, 24857, 27775, 30913, 34279, 37881, 41727, 45825, 50183, 54809, 59711, 64897, 70375, 76153, 82239, ...

    //Centered Dodecahedral Numbers
    //http://en.wikipedia.org/wiki/Centered_dodecahedral_number
    //http://oeis.org/A005904
    //(2*n+1)*(5*n^2+5*n+1)
    //1, 33, 155, 427, 909, 1661, 2743, 4215, 6137, 8569, 11571, 15203, 19525, 24597, 30479, 37231, 44913, 53585, 63307, 74139, 86141, 99373, 113895, 129767, 147049, 165801, 186083, 207955, 231477, 256709, 283711, 312543, 343265, 375937, 410619, 447371, ...

    //Centered Icosahedral Numbers
    //http://en.wikipedia.org/wiki/Centered_icosahedral_number
    //http://oeis.org/A005902
    //(2*n+1)*(5*n^2+5*n+3)/3
    //1, 13, 55, 147, 309, 561, 923, 1415, 2057, 2869, 3871, 5083, 6525, 8217, 10179, 12431, 14993, 17885, 21127, 24739, 28741, 33153, 37995, 43287, 49049, 55301, 62063, 69355, 77197, 85609, 94611, 104223, 114465, 125357, 136919, 149171, ...
    private static List<long> s_polyhedral_4_numbers = null;
    private static List<long> s_polyhedral_6_numbers = null;
    private static List<long> s_polyhedral_8_numbers = null;
    private static List<long> s_polyhedral_12_numbers = null;
    private static List<long> s_polyhedral_20_numbers = null;
    public static List<long> CenteredTetrahedralNumbers
    {
        get
        {
            if (s_polyhedral_4_numbers == null)
            {
                s_polyhedral_4_numbers = new List<long>(s_series_limit);
                for (int n = 0; n < s_series_limit; n++)
                {
                    long number = ((2 * n + 1) * ((n * n) + n + 3)) / 3;
                    s_polyhedral_4_numbers.Add(number);
                }
            }
            return s_polyhedral_4_numbers;
        }
    }
    public static List<long> CenteredCubeNumbers
    {
        get
        {
            return CenteredHexahedronNumbers;
        }
    }
    public static List<long> CenteredHexahedronNumbers
    {
        get
        {
            if (s_polyhedral_6_numbers == null)
            {
                s_polyhedral_6_numbers = new List<long>(s_series_limit);
                for (int n = 0; n < s_series_limit; n++)
                {
                    long number = (n * n * n) + ((n + 1) * (n + 1) * (n + 1));
                    s_polyhedral_6_numbers.Add(number);
                }
            }
            return s_polyhedral_6_numbers;
        }
    }
    public static List<long> CenteredOctahedralNumbers
    {
        get
        {
            if (s_polyhedral_8_numbers == null)
            {
                s_polyhedral_8_numbers = new List<long>(s_series_limit);
                for (int n = 0; n < s_series_limit; n++)
                {
                    long number = (2 * n + 1) * (2 * (n * n) + 2 * n + 3) / 3;
                    s_polyhedral_8_numbers.Add(number);
                }
            }
            return s_polyhedral_8_numbers;
        }
    }
    public static List<long> CenteredDodecahedralNumbers
    {
        get
        {
            if (s_polyhedral_12_numbers == null)
            {
                s_polyhedral_12_numbers = new List<long>(s_series_limit);
                for (int n = 0; n < s_series_limit; n++)
                {
                    long number = (2 * n + 1) * (5 * (n * n) + 5 * n + 1);
                    s_polyhedral_12_numbers.Add(number);
                }
            }
            return s_polyhedral_12_numbers;
        }
    }
    public static List<long> CenteredIcosahedralNumbers
    {
        get
        {
            if (s_polyhedral_20_numbers == null)
            {
                s_polyhedral_20_numbers = new List<long>(s_series_limit);
                for (int n = 0; n < s_series_limit; n++)
                {
                    long number = (2 * n + 1) * (5 * (n * n) + 5 * n + 3) / 3;
                    s_polyhedral_20_numbers.Add(number);
                }
            }
            return s_polyhedral_20_numbers;
        }
    }
    public static bool IsCenteredTetrahedralNumber(long number)
    {
        return (CenteredTetrahedralNumbers.Contains(number));
    }
    public static bool IsCenteredCubeNumber(long number)
    {
        return IsCenteredHexahedronNumber(number);
    }
    public static bool IsCenteredHexahedronNumber(long number)
    {
        return (CenteredHexahedronNumbers.Contains(number));
    }
    public static bool IsCenteredOctahedralNumber(long number)
    {
        return (CenteredOctahedralNumbers.Contains(number));
    }
    public static bool IsCenteredDodecahedralNumber(long number)
    {
        return (CenteredDodecahedralNumbers.Contains(number));
    }
    public static bool IsCenteredIcosahedralNumber(long number)
    {
        return (CenteredIcosahedralNumbers.Contains(number));
    }

    // https://oeis.org  // Chemical polyhex hydrocarbons with 19 hexagons
    // Number of Isomers of polyhex hydrocarbons with C_(2h) symmetry with nineteen hexagons
    // 3, 17, 66, 189, 589, 1677, 3829, 7948, 15649, 25543, 26931, 15472 
    // Number of isomers of polyhex hydrocarbons with C_(2v) symmetry with nineteen hexagons
    // 3, 17, 14, 92, 60, 316, 175, 814, 495, 2323, 1402, 6037, 3113, 12851, 6200, 24710, 11851, 46152, 18123, 72151, 18007, 74547, 8970, 40141
    private static Dictionary<int, List<long>> s_polyhex_numbers_dictionary = new Dictionary<int, List<long>>();
    public static List<long> PolyhexNumbers(int hexagons)
    {
        if (!s_polyhex_numbers_dictionary.ContainsKey(hexagons))
        {
            GeneratePolyhexNumbers(hexagons);
        }

        if (s_polyhex_numbers_dictionary.ContainsKey(hexagons))
        {
            return s_polyhex_numbers_dictionary[hexagons];
        }
        else
        {
            return null;
        }
    }
    private static void GeneratePolyhexNumbers(int hexagons)
    {
        //List<long> polyhex_numbers = new List<long>(s_series_limit);
        //for (int n = 1; n <= s_series_limit; n++)
        //{
        //    long number = ((hexagons - 2) * n * n - (hexagons - 4) * n) / 2L;
        //    polyhex_numbers.Add(number);
        //}
        //s_polyhex_numbers_dictionary.Add(hexagons, polyhex_numbers);

        List<long> polyhex_numbers = null;
        if (hexagons == 1)
        {
            polyhex_numbers = new List<long>() { 3, 17, 66, 189, 589, 1677, 3829, 7948, 15649, 25543, 26931, 15472 };
        }
        else if (hexagons == 2)
        {
            polyhex_numbers = new List<long>() { 3, 17, 14, 92, 60, 316, 175, 814, 495, 2323, 1402, 6037, 3113, 12851, 6200, 24710, 11851, 46152, 18123, 72151, 18007, 74547, 8970, 40141 };
        }
        else
        {
            //
        }
        s_polyhex_numbers_dictionary.Add(hexagons, polyhex_numbers);
    }
    public static bool IsPolyhexNumber(int hexagons, long number)
    {
        if (s_polyhex_numbers_dictionary.ContainsKey(hexagons))
        {
            return (s_polyhex_numbers_dictionary[hexagons].Contains(number));
        }
        else
        {
            return false;
        }
    }
    public static List<long> C2hPolyhexHydrocarbons
    {
        get
        {
            return PolyhexNumbers(1);
        }
    }
    public static List<long> C2vPolyhexHydrocarbons
    {
        get
        {
            return PolyhexNumbers(2);
        }
    }
    public static bool IsC2hPolyhexHydrocarbon(long number)
    {
        return (PolyhexNumbers(1).Contains(number));
    }
    public static bool IsC2vPolyhexHydrocarbon(long number)
    {
        return (PolyhexNumbers(2).Contains(number));
    }

    //Tetrahedral,            // n*(n+1)*(n+2)/6   = 1, 4, 10, 20, 35, 56, 84, 120, 165, 220, 286, 364, 455, 560, 680, 816, 969, 1140, 1330, 1540, 1771, 2024, 2300, 2600, 2925, 3276, 3654, 4060, 4495, 4960, 5456, 5984, 6545, 7140, 7770, 8436, 9139, 9880, 10660, 11480, 12341, 13244, 14190, 15180, ...
    private static int s_tetrahedrals_limit = s_series_limit;
    private static List<long> s_tetrahedrals;
    public static List<long> Tetrahedrals
    {
        get
        {
            if (s_tetrahedrals == null)
            {
                GenerateTetrahedrals();
            }
            return s_tetrahedrals;
        }
    }
    private static void GenerateTetrahedrals()
    {
        int max = s_tetrahedrals_limit;
        s_tetrahedrals = new List<long>(max);
        for (int number = 1; number <= max; number++)
        {
            long result = (number * (number + 1) * (number + 2)) / 6L;
            s_tetrahedrals.Add(result);
        }
    }
    public static bool IsTetrahedral(long number)
    {
        return (Tetrahedrals.Contains(number));
    }

    //Pyramidal,              // n*(n+1)*(2*n+1)/6 = 1, 5, 14, 30, 55, 91, 140, 204, 285, 385, 506, 650, 819, 1015, 1240, 1496, 1785, 2109, 2470, 2870, 3311, 3795, 4324, 4900, 5525, 6201, 6930, 7714, 8555, 9455, 10416, 11440, 12529, 13685, 14910, 16206, 17575, 19019, 20540, 22140, 23821, 25585, 27434, 29370, ...
    private static int s_pyramidals_limit = s_series_limit;
    private static List<long> s_pyramidals;
    public static List<long> Pyramidals
    {
        get
        {
            if (s_pyramidals == null)
            {
                GeneratePyramidals();
            }
            return s_pyramidals;
        }
    }
    private static void GeneratePyramidals()
    {
        int max = s_pyramidals_limit;
        s_pyramidals = new List<long>(max);
        for (int number = 1; number <= max; number++)
        {
            long result = (number * (number + 1) * ((2 * number) + 1)) / 6L;
            s_pyramidals.Add(result);
        }
    }
    public static bool IsPyramidal(long number)
    {
        return (Pyramidals.Contains(number));
    }

    //Cubic,                  // n^3               = 1, 8, 27, 64, 125, ...
    private static int s_cubics_limit = s_series_limit;
    private static List<long> s_cubics;
    public static List<long> Cubics
    {
        get
        {
            if (s_cubics == null)
            {
                GenerateCubics();
            }
            return s_cubics;
        }
    }
    private static void GenerateCubics()
    {
        int max = s_cubics_limit;
        s_cubics = new List<long>(max);
        for (int number = 1; number <= max; number++)
        {
            s_cubics.Add(number * number * number);
        }
    }
    public static bool IsCubic(long number)
    {
        return (Cubics.Contains(number));
    }

    //Collatz Conjecture: Starting with any positive integer n, do 3n+1 if n=Odd, do n/2 if n=Even, if continue this process, n must reach 1
    //Collatz Numbers = Squence of numbers with increasing steps to reach 1
    //1, 2, 3, 6, 7, 9, 18, 25, 27, 54, 73, 97, 129, 171, 231, 313, 327, 649, 703, 871, 1161, 2223, 2463, 2919, 3711, 6171, 10971, 13255, 17647, 23529, 26623, 34239, 35655, 52527, 77031, 106239, 142587, 156159, 216367, 230631, 410011, 511935, 626331, 837799, ...
    private static List<long> s_collatzs;
    public static List<long> Collatzs
    {
        get
        {
            if (s_collatzs == null)
            {
                GenerateCollatzs();
            }
            return s_collatzs;
        }
    }
    private static void GenerateCollatzs()
    {
        s_collatzs = new List<long>() 
            {
               1, 2, 3, 6, 7, 9, 18, 25, 27, 54, 73, 97, 129, 171, 231, 313,
               327, 649, 703, 871, 1161, 2223, 2463, 2919, 3711, 6171, 10971,
               13255, 17647, 23529, 26623, 34239, 35655, 52527, 77031, 106239,
               142587, 156159, 216367, 230631, 410011, 511935, 626331, 837799
            };
    }
    /// <summary>
    /// <para>Collatz Conjecture</para>
    /// <para>Starting with any positive integer n, do 3n+1 if n=Odd, do n/2 if n=Even, if continue this process, n must reach 1</para>
    /// <para>Collatz Numbers = Squence of numbers with increasing steps to reach 1</para>
    /// <para>1, 2, 3, 6, 7, 9, 18, 25, 27, 54, 73, 97, 129, 171, 231, 313, 327, 649, 703, 871, 1161, 2223, 2463, 2919, 3711, 6171, 10971,</para>
    /// <para>13255, 17647, 23529, 26623, 34239, 35655, 52527, 77031, 106239, 142587, 156159, 216367, 230631, 410011, 511935, 626331, 837799, ...</para>
    /// </summary>
    /// <param name="number"></param>
    /// <returns>bool</returns>
    public static bool IsCollatz(long number)
    {
        return (Collatzs.Contains(number));
    }

    //Gematria,                  // 1..10..100..1000..10000
    private static List<long> s_gematria;
    public static List<long> Gematria
    {
        get
        {
            if (s_gematria == null)
            {
                GenerateGematria();
            }
            return s_gematria;
        }
    }
    private static void GenerateGematria()
    {
        s_gematria = new List<long>() 
            { 
                 1,    2,    3,    4,    5,    6,    7,    8,    9,
                 10,   20,   30,   40,   50,   60,   70,   80,   90,
                 100,  200,  300,  400,  500,  600,  700,  800,  900,
                 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000,
                 10000
            };
    }
    public static bool IsGematria(long number)
    {
        return Gematria.Contains(number);
    }

    // QuranNumber,             // 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 30, 40, 50, 60, 70, 80, 99, 100, 200, 300, 1000, 2000, 3000, 5000, 50000, 100000
    private static List<long> s_quran_numbers;
    public static List<long> QuranNumbers
    {
        get
        {
            if (s_quran_numbers == null)
            {
                GenerateQuranNumbers();
            }
            return s_quran_numbers;
        }
    }
    private static void GenerateQuranNumbers()
    {
        s_quran_numbers = new List<long>() 
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 30, 40, 50, 60, 70, 80, 99, 100, 200, 300, 1000, 2000, 3000, 5000, 50000, 100000
            };
    }
    public static bool IsQuranNumber(long number)
    {
        return QuranNumbers.Contains(number);
    }


    public static List<long> Factorize(long number)
    {
        List<long> result = new List<long>();
        if (number < 0L)
        {
            result.Add(-1L);
            number *= -1L;
        }

        if ((number >= 0L) && (number <= 2L))
        {
            result.Add(number);
        }
        else // if (number > 2L)
        {
            // if number has a prime factor add it to factors,
            // number /= p,
            // reloop until  number == 1L
            while (number != 1L)
            {
                if ((number % 2L) == 0L) // if even number
                {
                    result.Add(2L);
                    number /= 2L;
                }
                else // trial divide by all primes upto sqrt(number)
                {
                    long max = (long)(Math.Sqrt(number)) + 1L;	// extra 1 for double calculation errors

                    bool is_factor_found = false;
                    for (long i = 3L; i <= max; i += 2L)
                    {
                        if ((number % i) == 0L)
                        {
                            is_factor_found = true;
                            result.Add(i);
                            number /= i;
                            break; // for loop, reloop while
                        }
                    }

                    // if no prime factor found the number must be prime in the first place
                    if (!is_factor_found)
                    {
                        result.Add(number);
                        break; // while loop
                    }
                }
            }
        }
        return result;
    }
    public static string FactorizeToString(long value)
    {
        StringBuilder str = new StringBuilder();
        List<long> factors = Factorize(value);
        if (factors != null)
        {
            if (factors.Count > 0)
            {
                foreach (long factor in factors)
                {
                    str.Append(factor.ToString() + " * "); // • ×
                }
                if (str.Length > 3)
                {
                    str.Remove(str.Length - 3, 3);
                }
            }
        }
        return str.ToString();
    }

    public static int BinarySearch(IList<long> sorted_list, long number)
    {
        if (sorted_list == null) return -1;
        if (sorted_list.Count < 1) return -1;

        int min = 0;
        int max = sorted_list.Count - 1;
        int old_mid = -1;
        int mid;
        while ((mid = (min + max) / 2) != old_mid)
        {
            if (number == sorted_list[min]) { return min; }

            if (number == sorted_list[max]) { return max; }

            if (number == sorted_list[mid]) { return mid; }
            else if (number < sorted_list[mid]) { max = mid; }
            else /*if (number > sorted_list[mid])*/ { min = mid; }

            old_mid = mid;
        }

        return -1;
    }
    public static void QuickSort(IList<long> list, int min, int max)
    {
        if (list == null) return;
        if (list.Count < 1) return;
        if (min > max) return;
        if ((min < 0) || (max >= list.Count)) return;

        int lo = min;
        int hi = max;
        long mid = list[(lo + hi) / 2];	// uses copy constructor

        do
        {
            while (list[lo] < mid)		// uses comparison operator
                lo++;
            while (mid < list[hi])
                hi--;

            if (lo <= hi)
            {
                long temp = list[hi];
                list[hi] = list[lo];
                list[hi] = temp;
                lo++;
                hi--;
            }
        }
        while (lo <= hi);

        if (hi > min)
            QuickSort(list, min, hi);
        if (lo < max)
            QuickSort(list, lo, max);
    }

    // SubsetSum Methods
    // dynamic programming
    private static Dictionary<int, bool> m_memo = new Dictionary<int, bool>();
    private static Dictionary<int, KeyValuePair<int, int>> m_previous = new Dictionary<int, KeyValuePair<int, int>>();
    public static bool FindSubset(IList<int> set, int sum)
    {
        m_memo.Clear();
        m_previous.Clear();
        m_memo[0] = true;
        m_previous[0] = new KeyValuePair<int, int>(-1, 0);

        for (int i = 0; i < set.Count; ++i)
        {
            int num = set[i];
            for (int s = sum; s >= num; --s)
            {
                if (m_memo.ContainsKey(s - num) && m_memo[s - num] == true)
                {
                    m_memo[s] = true;

                    if (!m_previous.ContainsKey(s))
                    {
                        m_previous[s] = new KeyValuePair<int, int>(i, num);
                    }
                }
            }
        }

        return m_memo.ContainsKey(sum) && m_memo[sum];
    }
    public static IEnumerable<int> GetLastIndex(int sum)
    {
        while (m_previous[sum].Key != -1)
        {
            yield return m_previous[sum].Key;
            sum -= m_previous[sum].Value;
        }
    }
    // backtracking recursive programming
    private static int s_count = 0;
    /// <summary>
    /// Count all subests of given size (0 size means all sizes) with given target sum
    /// </summary>
    /// <param name="numbers"></param>
    /// <param name="index"></param>
    /// <param name="current"></param>
    /// <param name="sum"></param>
    /// <param name="size"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static int CountSubsets(int[] numbers, int index, int current, int sum, int size, List<int> result)
    {
        if ((numbers.Length <= index) || (current > sum)) return 0;
        if (result == null) result = new List<int>();

        List<int> temp = new List<int>(result);
        if (current + numbers[index] == sum)
        {
            temp.Add(index);
            if ((size == 0) || (temp.Count == size))
            {
                s_count++;
            }
        }
        else if (current + numbers[index] < sum)
        {
            temp.Add(index);
            CountSubsets(numbers, index + 1, current + numbers[index], sum, size, temp);
        }

        CountSubsets(numbers, index + 1, current, sum, size, result);
        return s_count;
    }
    // backtracking recursive programming
    private static List<List<int>> m_subsets = new List<List<int>>();
    /// <summary>
    /// Find all subests of given size (0 size means all sizes) with given target sum
    /// </summary>
    /// <param name="numbers"></param>
    /// <param name="index"></param>
    /// <param name="current"></param>
    /// <param name="sum"></param>
    /// <param name="size"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static List<List<int>> FindSubsets(int[] numbers, int index, int current, int sum, int size, List<int> result)
    {
        if ((numbers.Length <= index) || (current > sum)) return m_subsets;
        if (result == null) result = new List<int>();

        List<int> temp = new List<int>(result);
        if (current + numbers[index] == sum)
        {
            temp.Add(index);
            if ((size == 0) || (temp.Count == size))
            {
                m_subsets.Add(temp);
            }
        }
        else if (current + numbers[index] < sum)
        {
            temp.Add(index);
            FindSubsets(numbers, index + 1, current + numbers[index], sum, size, temp);
        }

        FindSubsets(numbers, index + 1, current, sum, size, result);

        return m_subsets;
    }
    public static void SubsetSumMain(string[] args)
    {
        int[] numbers = new int[]
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

        int sum = 17;
        int size = 2;

        // call dynamic programming
        Console.WriteLine("FindSubset");
        if (Numbers.FindSubset(numbers, sum))
        {
            foreach (int index in Numbers.GetLastIndex(sum))
            {
                Console.Write((index + 1) + "." + numbers[index] + "\t");
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        // call backtracking recursive programming
        Console.WriteLine("CountSubsets");
        int count = Numbers.CountSubsets(numbers, 0, 0, sum, size, null);
        Console.WriteLine("Count = " + count);
        Console.WriteLine();

        // call backtracking recursive programming
        Console.WriteLine("FindSubsets");
        List<List<int>> subsets = Numbers.FindSubsets(numbers, 0, 0, sum, size, null);
        for (int i = 0; i < subsets.Count; i++)
        {
            if (subsets[i] != null)
            {
                Console.Write((i + 1).ToString() + ":\t");
                for (int j = 0; j < subsets[i].Count; j++)
                {
                    int index = subsets[i][j];
                    Console.Write((index + 1) + "." + numbers[index] + " ");
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine("Count = " + subsets.Count);

        Console.ReadKey();
    }
    //http://stackoverflow.com/questions/30006497/c-sharp-net-2-0-find-all-k-size-subsets-with-sum-s-of-an-n-size-bag-of-duplicat
    /*
    Please note that this is required for a **C# .NET 2.0** project (**Linq not allowed**).

    I know very similar questions have been asked here and I have already produce some working code (see below) but still would like an advice as to how to make the algorithm faster given k and s conditions.

    This is what I've learnt so far:
    Dynamic programming is the most efficient way to finding ONE (not all) subsets. Please correct me if I am wrong. And is there a way of repeatedly calling the DP code to produce newer subsets till the bag (set with duplicates) is exhausted?

    If not, then is there a way that may speed up the backtracking recursive algorithm I have below which does produce what I need but runs in O(2^n), I think, by taking s and k into account?

    Here is my fixed bag of numbers that will NEVER change with n=114 and number range from 3 to 286:

            int[] numbers = new int[]
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


    **Requirements**

     - Space limit to 2-3GB max but time should be O(n^something) not
       (something^n).

     - The bag must not be sorted and duplicate must not be    removed.

     - The result should be the indices of the numbers in the    matching
       subset, not the numbers themselves (as we have duplicates).


    **Dynamic Programming Attempt**

    Here is the C# dynamic programming version adapted from an answer to a similar question here on stackoverflow.com:

        using System;
        using System.Collections.Generic;

        namespace Utilities
        {
            public static class Combinations
            {
                private static Dictionary<int, bool> m_memo = new Dictionary<int, bool>();
                private static Dictionary<int, KeyValuePair<int, int>> m_previous = new Dictionary<int, KeyValuePair<int, int>>();
                static Combinations()
                {
                    m_memo.Clear();
                    m_previous.Clear();
                    m_memo[0] = true;
                    m_previous[0] = new KeyValuePair<int, int>(-1, 0);

                }

                public static bool FindSubset(IList<int> set, int sum)
                {
                    //m_memo.Clear();
                    //m_previous.Clear();
                    //m_memo[0] = true;
                    //m_previous[0] = new KeyValuePair<int, int>(-1, 0);

                    for (int i = 0; i < set.Count; ++i)
                    {
                        int num = set[i];
                        for (int s = sum; s >= num; --s)
                        {
                            if (m_memo.ContainsKey(s - num) && m_memo[s - num] == true)
                            {
                                m_memo[s] = true;

                                if (!m_previous.ContainsKey(s))
                                {
                                    m_previous[s] = new KeyValuePair<int, int>(i, num);
                                }
                            }
                        }
                    }

                    return m_memo.ContainsKey(sum) && m_memo[sum];
                }
                public static IEnumerable<int> GetLastIndex(int sum)
                {
                    while (m_previous[sum].Key != -1)
                    {
                        yield return m_previous[sum].Key;
                        sum -= m_previous[sum].Value;
                    }
                }

                public static void SubsetSumMain(string[] args)
                {
                    int[] numbers = new int[]
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

                    int sum = 400;
                    //int size = 4; // don't know to use in dynamic programming

                    // call dynamic programming
                    if (Numbers.FindSubset(numbers, sum))
                    {
                        foreach (int index in Numbers.GetLastIndex(sum))
                        {
                            Console.Write((index + 1) + "." + numbers[index] + "\t");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();

                    Console.ReadKey();
                }
            }
        }

    **Recursive Programming Attempt**

    and Here is the C# recursive programming version adapted from an answer to a similar question here on stackoverflow.com:

        using System;
        using System.Collections.Generic;

        namespace Utilities
        {
            public static class Combinations
            {
                private static int s_count = 0;
                public static int CountSubsets(int[] numbers, int index, int current, int sum, int size, List<int> result)
                {
                    if ((numbers.Length <= index) || (current > sum)) return 0;
                    if (result == null) result = new List<int>();

                    List<int> temp = new List<int>(result);
                    if (current + numbers[index] == sum)
                    {
                        temp.Add(index);
                        if ((size == 0) || (temp.Count == size))
                        {
                            s_count++;
                        }
                    }
                    else if (current + numbers[index] < sum)
                    {
                        temp.Add(index);
                        CountSubsets(numbers, index + 1, current + numbers[index], sum, size, temp);
                    }

                    CountSubsets(numbers, index + 1, current, sum, size, result);
                    return s_count;
                }

                private static List<List<int>> m_subsets = new List<List<int>>();
                public static List<List<int>> FindSubsets(int[] numbers, int index, int current, int sum, int size, List<int> result)
                {
                    if ((numbers.Length <= index) || (current > sum)) return m_subsets;
                    if (result == null) result = new List<int>();

                    List<int> temp = new List<int>(result);
                    if (current + numbers[index] == sum)
                    {
                        temp.Add(index);
                        if ((size == 0) || (temp.Count == size))
                        {
                            m_subsets.Add(temp);
                        }
                    }
                    else if (current + numbers[index] < sum)
                    {
                        temp.Add(index);
                        FindSubsets(numbers, index + 1, current + numbers[index], sum, size, temp);
                    }

                    FindSubsets(numbers, index + 1, current, sum, size, result);

                    return m_subsets;
                }

                public static void SubsetSumMain(string[] args)
                {
                    int[] numbers = new int[]
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

                    int sum = 17;
                    int size = 2;

                    // call backtracking recursive programming
                    Console.WriteLine("CountSubsets");
                    int count = Numbers.CountSubsets(numbers, 0, 0, sum, size, null);
                    Console.WriteLine("Count = " + count);
                    Console.WriteLine();

                    // call backtracking recursive programming
                    Console.WriteLine("FindSubsets");
                    List<List<int>> subsets = Numbers.FindSubsets(numbers, 0, 0, sum, size, null);
                    for (int i = 0; i < subsets.Count; i++)
                    {
                        if (subsets[i] != null)
                        {
                            Console.Write((i + 1).ToString() + ":\t");
                            for (int j = 0; j < subsets[i].Count; j++)
                            {
                                int index = subsets[i][j];
                                Console.Write((index + 1) + "." + numbers[index] + " ");
                            }
                            Console.WriteLine();
                        }
                    }
                    Console.WriteLine("Count = " + subsets.Count);

                    Console.ReadKey();
                }
            }
        }

    Please let me know if how to restrict the dynamic programming version to subset size k and if I can call it repeatedly so it returns different subsets on every call until there are no more matching subsets.

    Also I am not sure where to initialize the memo of the DP algorithm. I did it in the static constructor which auto-runs when accessing any method. Is this the correct initialization place or does it need to be moved to inside the FindSunset() method [commented out]?

    As for the recursive version, is it backtracking? and how can we speed it up. It works correctly and takes k and s into account but totally inefficient.

    Let's make this thread the mother of all C# SubsetSum related questions!

    Thank you :)
    */

    // DO NOT USE as Microsoft is required by law to provide a backdoor
    // to the NSA in System.Security.Cryptography for "national security".
    public static void GenerateRSAKeys()
    {
        GenerateRSAKeys(null);
    }
    public static void GenerateRSAKeys(string username)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss");
            if (!String.IsNullOrEmpty(username))
            {
                username = username.Replace(" ", "");
                username = username.Replace(".", "");
                username = username.Replace("\b", "");
                username = username.Replace("\t", "");
                username = username.Replace("\r", "");
                username = username.Replace("\n", "");
            }
            else
            {
                username = "";
            }

            int key_length = 16 * 1024; // in bits
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(key_length);

            string private_key_filename = NUMBERS_FOLDER + "/" + timestamp + "_" + username + ((username.Length == 0) ? "" : "_") + "PrivateKey.xml";
            using (StreamWriter writer = new StreamWriter(private_key_filename, false, Encoding.Unicode))
            {
                writer.WriteLine(provider.ToXmlString(true));
            }

            string public_key_filename = NUMBERS_FOLDER + "/" + timestamp + "_" + username + ((username.Length == 0) ? "" : "_") + "PublicKey.xml";
            using (StreamWriter writer = new StreamWriter(public_key_filename, false, Encoding.Unicode))
            {
                writer.WriteLine(provider.ToXmlString(false));
            }
        }
        catch
        {
            // silence IO error in case running from read-only media (CD/DVD)
        }
    }
    private static void RSATest()
    {
        var publicPrivateRsa = new RSACryptoServiceProvider
            (
                new CspParameters()
                {
                    KeyContainerName = "PublicPrivateKeys",
                    Flags = CspProviderFlags.UseMachineKeyStore
                    //Flags = CspProviderFlags.UseDefaultKeyContainer 
                }
            )
        {
            PersistKeyInCsp = true,
        };

        var publicRsa = new RSACryptoServiceProvider(
                new CspParameters()
                {
                    KeyContainerName = "PublicKey",
                    Flags = CspProviderFlags.UseMachineKeyStore
                    //Flags = CspProviderFlags.UseDefaultKeyContainer 
                }
            )
        {
            PersistKeyInCsp = true
        };


        //Export the key. 
        publicRsa.ImportParameters(publicPrivateRsa.ExportParameters(false));
        Console.WriteLine(publicRsa.ToXmlString(false));
        Console.WriteLine(publicPrivateRsa.ToXmlString(false));
        //Dispose those two CSPs. 
        using (publicRsa)
        {
            publicRsa.Clear();
        }
        using (publicPrivateRsa)
        {
            publicRsa.Clear();
        }


        //Retrieve keys
        publicPrivateRsa = new RSACryptoServiceProvider(
                new CspParameters()
                {
                    KeyContainerName = "PublicPrivateKeys",
                    Flags = CspProviderFlags.UseMachineKeyStore
                    //Flags = CspProviderFlags.UseDefaultKeyContainer 
                }
            );

        publicRsa = new RSACryptoServiceProvider(
                new CspParameters()
                {
                    KeyContainerName = "PublicKey",
                    Flags = CspProviderFlags.UseMachineKeyStore
                    //Flags = CspProviderFlags.UseDefaultKeyContainer 
                }
            );
        Console.WriteLine(publicRsa.ToXmlString(false));
        Console.WriteLine(publicPrivateRsa.ToXmlString(false));
        using (publicRsa)
        {
            publicRsa.Clear();
        }
        using (publicPrivateRsa)
        {
            publicRsa.Clear();
        }
    }
}