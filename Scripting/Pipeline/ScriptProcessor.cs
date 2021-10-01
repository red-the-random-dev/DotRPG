using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = System.String;
using TOutput = DotRPG.Scripting.LuaModule;

namespace ScriptImporter
{
    [ContentProcessor(DisplayName = "DotRPG script processor")]
    class ScriptProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            return new TOutput(input);
        }
    }
}
