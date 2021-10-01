using Microsoft.Xna.Framework.Content.Pipeline;
using Scripting = DotRPG.Scripting;
using System.IO;

using TImport = System.String;

namespace DotRPG.Scripting.Pipeline
{
    [ContentImporter(".lua", DisplayName = "DotRPG embeddable script handler", DefaultProcessor = "ScriptProcessor")]
    public class ScriptImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllText(filename);
        }
    }
}
