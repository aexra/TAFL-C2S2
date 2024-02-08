using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Misc;
public class LexService
{
    public static uint Encode(string alphabet, string word, out uint N)
    {
        // TODO: Encoding algorithm
        N = 0;
        return 0;
    }
    public static string Decode(string alphabet, uint N, out string word)
    {
        List<uint> rems = new List<uint>();
        var n = (uint)alphabet.Length;
        var process =  __CalculateProcessString__(n, N, rems, out var _, out var _);
        rems.Reverse();
        process = process[1..(process.Length - 1)];
        var sumLine = "";
        word = "";
        foreach (var rem in rems)
        {
            word += alphabet[(int)rem - 1];
        }
        for (var i = 0; i < rems.Count; i++) 
        {
            sumLine += rems.Count - i - 1 > 0 ? $"+{rems[i]}*{n}{(rems.Count - i - 1 == 1? "" : $"^{rems.Count - i - 1}")}" : $"+{rems[i]}";
        }
        return process + " = " + sumLine[1..] + " = " + word;
    }

    private static string __CalculateProcessString__(uint n, uint N, List<uint> rems, out uint div, out uint rem)
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
            var res = __CalculateProcessString__(n, div, rems, out var _, out var _);
            return $"({res}*{n}+{rem})";
        }
        else
        {
            rems.Add(div);
            return $"({div}*{n}+{rem})";
        }
    }
}
