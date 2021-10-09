using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using DotRPG.Objects;

namespace DotRPG.Behavior
{
    public interface IXMLSceneBuilder
    {
        Frame BuildFromXML(XDocument x);
    }
}
