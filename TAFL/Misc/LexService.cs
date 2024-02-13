using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Misc;
public class LexService
{
    public static uint Encode(string alphabet, string word, out string process)
    {
        IDictionary<char, uint> pairs = new Dictionary<char, uint>();
        var n = alphabet.Length;
        var k = word.Length;

        for (var i = 1; i <= alphabet.Length; i++)
        {
            pairs.Add(new KeyValuePair<char, uint>(alphabet[i - 1], (uint)i));
        }

        process = "";
        long sum = 0;
        for (var i = 0; i < k; i++)
        {
            sum += pairs[word[i]] * (long)Math.Pow(n, k - i - 1);
            process += $" + {pairs[word[i]]}{(k - i - 1 != 0? k - i - 1 != 1? $"*{n}^{k - i - 1}" : $"*{n}" : "")}";
        }

        process = process[2..] + $" = {sum}";

        return (uint)sum;
    }
    public static string Decode(string alphabet, uint N, out string process)
    {
        List<uint> rems = new List<uint>();
        var n = (uint)alphabet.Length;

        process =  __CalculateDecodeProcessString__(n, N, rems, out var _, out var _);
        process = process[1..(process.Length - 1)];
        rems.Reverse();

        var word = "";
        foreach (var rem in rems)
        {
            word += alphabet[(int)rem - 1];
        }
        
        var sumLine = "";
        for (var i = 0; i < rems.Count; i++) 
        {
            sumLine += rems.Count - i - 1 > 0 ? $"+{rems[i]}*{n}{(rems.Count - i - 1 == 1? "" : $"^{rems.Count - i - 1}")}" : $"+{rems[i]}";
        }

        process += " = " + sumLine[1..] + " = " + word;

        return word;
    }

    private static string __CalculateDecodeProcessString__(uint n, uint N, List<uint> rems, out uint div, out uint rem)
    {
        if (N % n == 0)
        {
            div = (N - n) / n;
            rem = n;
        } 
        else
        {
            div = N / n;
            rem = N % n;
        }

        rems.Add(rem);

        if (div > n)
        {
            var res = __CalculateDecodeProcessString__(n, div, rems, out var _, out var _);
            return $"({res}*{n}+{rem})";
        }
        else
        {
            if (div != 0)
            {
                rems.Add(div);
                return $"({div}*{n}+{rem})";
            }
            else
            {
                return $"({rem})";
            }
        }
    }
}
