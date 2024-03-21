using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Interfaces;

namespace TAFL.Classes.Graph;
public class OrientedEdge : Edge
{
    public OrientedEdge(Node left, Node right, string weight) : base(left, right, weight)
    {

    }
}
