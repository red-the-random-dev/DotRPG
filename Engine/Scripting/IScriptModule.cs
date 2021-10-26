using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Scripting
{
    public interface IScriptModule
    {
        String LastError { get; }
        Exception LastErrorDetails { get; }
        Boolean SuppressExceptions { get; set; }

        void AddData(Dictionary<String, Object> data);
        void AddData(String key, Object value);
        void Start();
        void Update(String EventID, Single ElapsedGameTime, Single TotalGameTime);
    }
}
