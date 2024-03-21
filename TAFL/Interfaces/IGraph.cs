﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAFL.Classes.Graph;

namespace TAFL.Interfaces;
public interface IGraph
{
    public void AddNode(Node node);
    public void RemoveNode(Node node);
    public void Clear();
}
