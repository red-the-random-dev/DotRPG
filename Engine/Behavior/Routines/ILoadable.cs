using System;
using Microsoft.Xna.Framework.Graphics;

namespace DotRPG.Behavior.Routines
{
    public interface ILoadable
    {
        public Boolean ReadyForLoad { get; }
        public Boolean Loaded { get; }
        public Int32 ContentTasks_Total { get; }
        public Int32 ObjectTasks_Total { get; }
        public Int32 ContentTasks_Done { get; }
        public Int32 ObjectTasks_Done { get; }
        // Do single task per call
        public void PerformContentTask(GraphicsDevice gd);
        public void PerformObjectTask();

        public Boolean SupportsMultiLoading { get; }
        // Do multiple tasks at once
        public void PerformContentTasks(Int32 step, GraphicsDevice gd);
        public void PerformObjectTasks(Int32 step);

        public void PreloadTask();
        public void PostLoadTask();
    }
}
