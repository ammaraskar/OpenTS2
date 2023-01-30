using OpenTS2.Common;
using OpenTS2.Content;
using OpenTS2.Content.Effects;
using OpenTS2.Files.Formats.DBPF;

namespace OpenTS2.Files.Formats.Effects
{
    [Codec(TypeIDs.EFFECT)]
    public class EffectsCodec : AbstractCodec
    {
        public override AbstractAsset Deserialize(byte[] bytes, ResourceKey tgi, DBPFFile sourceFile)
        {
            return new EffectAsset();
        }
    }
}