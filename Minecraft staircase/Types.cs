﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minecraft_staircase
{
    public struct UnsettedBlock
    {
        public int ID { get; set; }
        public ColorType Set { get; set; }
    }

    public struct SettedBlock
    {
        public int ID { get; set; }
        public int Height { get; set; }
    }

    public enum ColorType
    {
        /// <summary>
        /// Downward block
        /// </summary>
        Dark = 0,
        /// <summary>
        /// On level block
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Upward block
        /// </summary>
        Light = 2
    }

    public enum ArtType
    {
        Flat = 0,
        Lite = 1,
        Full = 2
    }

    public struct BlockData
    {
        //public int ColorID { get; }
        public string TextureName { get; }
        public string Name { get; }
        public byte ID { get; }
        public byte Data { get; }
        public bool IsTransparent { get; }

        public BlockData(string textureName, string name, byte iD, byte data, bool isTransparent)
        {
            //ColorID = colorID;
            TextureName = textureName;
            Name = name;
            ID = iD;
            Data = data;
            IsTransparent = isTransparent;
        }
    }

    public class ColorNote
    {
        public int ColorID { get; set; }
        public Color DarkColor { get; set; }
        public Color NormalColor { get; set; }
        public Color LightColor { get; set; }

        public bool Use { get; set; }

        public BlockData SelectedBlock { get; set; }
        public List<BlockData> PossibleBlocks { get; set; }

        public int Uses { get; set; }

        public ColorNote(int colorID, Color darkColor, Color normalColor, Color lightColor)
        {
            ColorID = colorID;
            DarkColor = darkColor;
            NormalColor = normalColor;
            LightColor = lightColor;
        }

        public string ResourcesToString()
        {
            return $"{SelectedBlock.Name} - {Uses}";
        }

        public string DataToString()
        {
            string str = Use.ToString() + '~';
            foreach (BlockData bd in PossibleBlocks)
                str += $"{bd.TextureName}-{bd.Name}-{bd.ID}-{bd.Data}-{bd.IsTransparent},";
            str = str.Remove(str.Length - 1, 1);
            str += $"~{SelectedBlock.TextureName}-{SelectedBlock.Name}-{SelectedBlock.ID}-{SelectedBlock.Data}-{SelectedBlock.IsTransparent}";
            return str;
        }
    }
}