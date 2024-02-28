using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Misc;
internal class StringUtils
{
    public static string StringToDistinctString(string text)
    {
        return string.Join("", text.Distinct());
    }
}
