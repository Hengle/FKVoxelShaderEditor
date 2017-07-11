﻿//-------------------------------------------------
// Author:  FreeKnigt
// Date:    20170706
// Desc:    MagicaVox格式加载器
//-------------------------------------------------
using FKVoxelEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
//-------------------------------------------------
namespace MagicaVoxLoader
{
    [ContentProcessor(DisplayName = "Vox Processor - Chunks")]
    public class ChunkProcessor : ContentProcessor<VoxContent, ChunkContent>
    {
        [DefaultValue(-1)]
        public int ChunkSize;
        private VoxContent m_InputVoxContent;

        public override ChunkContent Process(VoxContent input, ContentProcessorContext context)
        {
            m_InputVoxContent = input;

            var voxels = m_InputVoxContent.Voxels;
            // load voxels into a grid so we can easily check neighbors
            var grid = BuildGrid(m_InputVoxContent);

            var actives = new List<VertexWithIndex>();
            var inactives = new List<VertexWithIndex>();
            for (var i = 0; i < m_InputVoxContent.Voxels.Length; i++)
            {
                var v = voxels[i];
                // if the voxel is outside the grid or empty, skip it
                if (v.X >= grid.Length || v.Y >= grid[0].Length || v.Z >= grid[0][0].Length || v.IsEmpty)
                    continue;

                var data = new VertexWithIndex(voxels[i].X, voxels[i].Y, voxels[i].Z, voxels[i].ColorIndex);

                if (IsActive(data.X, data.Y, data.Z, grid))
                    actives.Add(data);
                else
                    inactives.Add(data);
            }

            var chunk = new ChunkContent(Vector3.Zero, m_InputVoxContent.SizeX, m_InputVoxContent.SizeY, m_InputVoxContent.SizeZ,
                actives.Concat(inactives).ToArray(), m_InputVoxContent.Palette, actives.Count);

            return chunk;
        }

        private static byte[][][] BuildGrid(VoxContent input)
        {
            var grid = new byte[input.SizeX][][];
            for (var x = 0; x < input.SizeX; x++)
            {
                grid[x] = new byte[input.SizeY][];

                for (var y = 0; y < input.SizeY; y++)
                {
                    grid[x][y] = new byte[input.SizeZ];
                }
            }

            foreach (var voxel in input.Voxels)
                grid[voxel.X][voxel.Y][voxel.Z] = voxel.ColorIndex;

            return grid;
        }

        private static bool IsActive(byte x, byte y, byte z, byte[][][] grid)
        {
            // check if the block is at the edge of the chunk
            if (x == 0 || y == 0 || z == 0 || x == grid.Length - 1 || y == grid[0].Length - 1 || z == grid[0][0].Length - 1)
                return true;

            // check neighbors
            if (grid[x][y][z + 1] == 0 || grid[x][y][z - 1] == 0 || grid[x][y + 1][z] == 0 ||
                grid[x][y - 1][z] == 0 || grid[x + 1][y][z] == 0 || grid[x - 1][y][z] == 0)
                return true;

            return false;
        }
    }
}
