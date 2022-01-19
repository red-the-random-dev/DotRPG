using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotRPG.Behavior.Routines
{
    public class FrameLoader
    {
        public ILoadable Loaded;

        public Single LoadPercentage
        {
            get
            {
                if (Loaded.ContentTasks_Total + Loaded.ObjectTasks_Total == 0)
                {
                    return Single.NaN;
                }
                return (Single)Math.Round((100.0m * Loaded.ContentTasks_Done + Loaded.ObjectTasks_Done) / (Loaded.ContentTasks_Total + Loaded.ObjectTasks_Total), 1);
            }
        }

        public void Update(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
        {
            if (Loaded.Loaded)
            {
                return;
            }
            else if (!Loaded.ReadyForLoad)
            {
                Loaded.PreloadTask();
            }
            else if (Loaded.ContentTasks_Done < Loaded.ContentTasks_Total)
            {
                Loaded.PerformContentTask(gd);
            }
            else if (Loaded.ObjectTasks_Done < Loaded.ObjectTasks_Total)
            {
                Loaded.PerformObjectTask();
            }
            else
            {
                Loaded.PostLoadTask();
            }
        }
        public FrameLoader(ILoadable il)
        {
            Loaded = il;
        }
    }
}
