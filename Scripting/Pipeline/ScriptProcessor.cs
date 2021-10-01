using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = DotRPG.Scripting.LuaModule;
using TOutput = DotRPG.Scripting.LuaModule;

namespace ScriptImporter
{
    [ContentProcessor(DisplayName = "DotRPG script processor")]
    class ScriptProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
