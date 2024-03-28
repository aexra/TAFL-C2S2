using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Services;

namespace TAFL.Helpers;
public static class SetHelper
{
    public static string SetToString<T>(HashSet<T> set)
    {
        var list = new List<string>();
        set.ToList().ForEach(x => list.Add(x.ToString()));
        list.Sort();
        var s = "{ ";
        if (list.Count > 0)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                s += list[i].ToString() + ", ";
            }
            s += list.Last().ToString();
        }
        s += " }";
        return s;
    }
}
