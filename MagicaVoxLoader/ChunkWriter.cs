﻿//-------------------------------------------------
// Author:  FreeKnigt
// Date:    20170708
// Desc:    
//-------------------------------------------------
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//-------------------------------------------------
namespace MagicaVoxLoader
{
    [ContentTypeWriter]
    public class ChunkWriter : ContentTypeWriter<ChunkContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FKVoxelEngine.ChunkReader, FKVoxelEngine";
        }

        protected override void Write(ContentWriter output, ChunkContent value)
        {
            output.Write(value.SizeX);
            output.Write(value.SizeY);
            output.Write(value.SizeZ);
            output.Write(value.Position);
            output.Write(value.BlockCount);
            output.Write(value.ActiveBlocks);

            for (var i = 0; i < value.BlockCount; i++)
            {
                var b = value.Blocks[i];
                output.Write(b.X);
                output.Write(b.Y);
                output.Write(b.Z);
                output.Write(b.Index);
            }

            foreach (var c in value.Palette)
            {
                output.Write(c);
            }
        }
    }
}
