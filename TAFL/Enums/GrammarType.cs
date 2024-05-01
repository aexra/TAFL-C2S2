using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL.Enums;
public enum GrammarType
{
    [Description("Грамматика фразовой структуры (грамматика без ограничений)")]
    Type0,

    [Description("Контекстно-зависимая грамматика")]
    Type1,

    [Description("Контекстно-свободная грамматика")]
    Type2,

    [Description("Регулярная грамматика")]
    Type3
}
