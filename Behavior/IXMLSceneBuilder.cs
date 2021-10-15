using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DotRPG.Objects;
using Microsoft.Xna.Framework;

namespace DotRPG.Behavior
{
    public interface IXMLSceneBuilder
    {
        Frame BuildFromXML(XDocument x, Object[] embeddedData);
    }
}
