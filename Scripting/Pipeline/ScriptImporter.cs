using Microsoft.Xna.Framework.Content.Pipeline;
using Scripting = DotRPG.Scripting;
using System.IO;

namespace DotRPG.Scripting.Pipeline
{
    [ContentImporter(".lua", DisplayName = "DotRPG embeddable script handler", DefaultProcessor = "ScriptProcessor")]
    public class ScriptImporter : ContentImporter<LuaModule>
    {
        public override LuaModule Import(string filename, ContentImporterContext context)
        {
            return new Scripting::LuaModule(File.ReadAllText(filename));
        }
    }
}
