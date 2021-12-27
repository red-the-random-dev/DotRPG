using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class CheckpointManager
    {
        public Boolean DoLoad = false;
        public Boolean DoSave = false;

        public void SoftReset()
        {
            DoLoad = true;
        }
        public void Save()
        {
            DoSave = true;
        }
    }
}
