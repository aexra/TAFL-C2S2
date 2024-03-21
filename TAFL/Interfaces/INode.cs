using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Classes.Graph;

namespace TAFL.Interfaces;
public interface INode
{
    public void Connect(Node to, string weight, bool isOriented);
    public void Disconnect(Node node);
    public void Delete();
}
