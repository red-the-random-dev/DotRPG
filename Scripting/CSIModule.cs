using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Scripting
{
    public abstract class CSIModule : IScriptModule
    {
        public String LastError { get; protected set; }
        public Exception LastErrorDetails { get; protected set; }
        public Boolean SuppressExceptions { get; set; }

        public void AddData(Dictionary<String, Object> data)
        {
            foreach (String i in data.Keys)
            {
                AddData(i, data[i]);
            }
        }
        public abstract void AddData(String key, Object value);
        public abstract void Start();
        public abstract void Update(String EventID, Single ElapsedGameTime, Single TotalGameTime);
    }
}
