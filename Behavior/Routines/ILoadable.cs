using System;

namespace DotRPG.Behavior.Routines
{
    public interface ILoadable
    {
        public Int32 ContentTasks_Total { get; }
        public Int32 ObjectTasks_Total { get; }
        public Int32 ContentTasks_Done { get; }
        public Int32 ObjectTasks_Done { get; }
        // Do single task per call
        public void PerformContentTask();
        public void PerformObjectTask();

        public Boolean SupportsMultiLoading { get; }
        // Do multiple tasks at once
        public void PerformContentTasks(Int32 step);
        public void PerformObjectTasks(Int32 step);

        public void PreloadTask();
        public void PostLoadTask();
    }
}
