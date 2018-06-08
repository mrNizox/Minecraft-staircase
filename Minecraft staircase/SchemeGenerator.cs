﻿using System;
using System.Collections.Generic;

namespace Minecraft_staircase
{
    class SchemeGenerator
    {
        const int GlobalMaximumHeight = 129;

        UnsettedBlock[,] rawScheme;

        public SchemeGenerator(ref UnsettedBlock[,] rawScheme)
        {
            this.rawScheme = rawScheme;
        }

        public SettedBlock[,] GenerateFlow(out int MaxHeight)
        {
            SettedBlock[,] BlockMap = new SettedBlock[rawScheme.GetLength(0), rawScheme.GetLength(1) + 1];
            MaxHeight = 0;
            #region
            //for (int i = 0; i < rawScheme.GetLength(0); ++i)
            //    BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
            //for (int i = 0; i < rawScheme.GetLength(0); ++i)
            //{
            //    int minHeight = 0;
            //    for (int j = 1; j < rawScheme.GetLength(1) + 1; ++j)
            //    {
            //        BlockMap[i, j].ID = rawScheme[i, j - 1].ID;
            //        switch (rawScheme[i, j - 1].Set)
            //        {
            //            case ColorType.Normal:
            //                BlockMap[i, j].Height = BlockMap[i, j - 1].Height;
            //                break;
            //            case ColorType.Dark:
            //                BlockMap[i, j].Height = BlockMap[i, j - 1].Height - 1;
            //                break;
            //            case ColorType.Light:
            //                BlockMap[i, j].Height = BlockMap[i, j - 1].Height + 1;
            //                break;
            //        }
            //        if (j != 1 && (j - 1) % 128 == 0)
            //            BlockMap[i, j].Height = 0;
            //        minHeight = BlockMap[i, j].Height < minHeight ? BlockMap[i, j].Height : minHeight;
            //    }
            //    for (int j = 0; j < rawScheme.GetLength(1) + 1; ++j)
            //    {
            //        BlockMap[i, j].Height = BlockMap[i, j].Height - minHeight;
            //        MaxHeight = BlockMap[i, j].Height > MaxHeight ? BlockMap[i, j].Height : MaxHeight;
            //    }
            //}
            #endregion
            for (int i = 0; i < rawScheme.GetLength(0); ++i)
            {
                SettedBlock[] layer = GenerateFlowLayer(i, out int curMaxHeight);

                if (curMaxHeight > MaxHeight)
                    MaxHeight = curMaxHeight;
                for (int j = 0; j < layer.Length; ++j)
                    BlockMap[i, j] = layer[j];
            }
            return BlockMap;
        }

        public SettedBlock[,] GenerateSegmented(out int MaxHeight)
        {
            SettedBlock[,] BlockMap = new SettedBlock[rawScheme.GetLength(0), rawScheme.GetLength(1) + 1];
            MaxHeight = 0;
            #region
            //for (int i = 0; i < rawScheme.GetLength(0); ++i)
            //{
            //    List<MBlock> mBlocks = new List<MBlock>();
            //    UnsettedBlock last = new UnsettedBlock { ID = -1, Set = ColorType.Normal };
            //    MBlock curMBlock = new MBlock();
            //    curMBlock.Add(new UnsettedBlock { ID = -1, Set = ColorType.Normal });
            //    bool isUp = rawScheme[i, 0].Set == ColorType.Light;
            //    for (int j = 0; j < rawScheme.GetLength(1); ++j)
            //    {
            //        if (!isUp)
            //        {
            //            if (rawScheme[i, j].Set == ColorType.Dark || rawScheme[i, j].Set == ColorType.Normal)
            //                curMBlock.Add(rawScheme[i, j]);
            //            else
            //            {
            //                isUp = true;
            //                curMBlock.Add(rawScheme[i, j]);
            //            }
            //        }
            //        else
            //        {
            //            if (rawScheme[i, j].Set == ColorType.Light || rawScheme[i, j].Set == ColorType.Normal)
            //                curMBlock.Add(rawScheme[i, j]);
            //            else
            //            {
            //                isUp = false;
            //                mBlocks.Add(curMBlock.Clone() as MBlock);
            //                curMBlock = new MBlock();
            //                curMBlock.Add(rawScheme[i, j]);
            //            }
            //        }
            //    }
            //    mBlocks.Add(curMBlock.Clone() as MBlock);

            //    foreach (MBlock mb in mBlocks)
            //        mb.Calculate();
            //    int minH = 0;
            //    for (int j = 1; j < mBlocks.Count; ++j)
            //    {
            //        if (mBlocks[j].StartH >= mBlocks[j - 1].EndH)
            //        {
            //            mBlocks[j].Shift = mBlocks[j].StartH - mBlocks[j - 1].EndH + 1;
            //            if (minH > mBlocks[j].Shift)
            //                minH = mBlocks[j].Shift;
            //            for (int k = j - 1; k >= 0; --k)
            //                if (mBlocks[k].EndH <= mBlocks[k + 1].StartH)
            //                    mBlocks[k].Shift += mBlocks[k + 1].StartH - mBlocks[k].EndH + 1;
            //        }
            //    }

            //    int curPos = -1;
            //    for (int j = 0; j < mBlocks.Count; ++j)
            //    {
            //        if (MaxHeight < mBlocks[j].StartH)
            //            MaxHeight = mBlocks[j].StartH;
            //        if (MaxHeight < mBlocks[j].EndH)
            //            MaxHeight = mBlocks[j].EndH;
            //        SettedBlock[] cur = mBlocks[j].Get();
            //        for (int k = 0; k < cur.Length; ++k)
            //            BlockMap[i, ++curPos] = cur[k];
            //    }
            //}
            #endregion
            for (int i = 0; i < rawScheme.GetLength(0); ++i)
            {
                SettedBlock[] layer = GenerateSegmentedLayer(i, out int curMaxHeight);

                if (curMaxHeight > MaxHeight)
                    MaxHeight = curMaxHeight;
                for (int j = 0; j < layer.Length; ++j)
                    BlockMap[i, j] = layer[j];
            }
            return BlockMap;
        }

        public SettedBlock[,] GenerateMixed(out int MaxHeight)
        {
            SettedBlock[,] BlockMap = new SettedBlock[rawScheme.GetLength(0), rawScheme.GetLength(1) + 1];
            MaxHeight = 0;
            for (int i = 0; i < rawScheme.GetLength(0); ++i)
            {
                SettedBlock[] layer = GenerateSegmentedLayer(i, out int curMaxHeight);
                if (curMaxHeight > GlobalMaximumHeight)
                    layer = GenerateFlowLayer(i, out curMaxHeight);

                if (curMaxHeight > MaxHeight)
                    MaxHeight = curMaxHeight;
                for (int j = 0; j < layer.Length; ++j)
                    BlockMap[i, j] = layer[j];
            }
            return BlockMap;
        }

        SettedBlock[] GenerateFlowLayer(int layerNum, out int MaxHeight)
        {
            SettedBlock[] layer = new SettedBlock[rawScheme.GetLength(1) + 1];
            MaxHeight = 0;

            layer[0] = new SettedBlock() { ID = -1, Height = 0 };
            int minHeight = 0;
            for (int i = 1; i < rawScheme.GetLength(1) + 1; ++i)
            {
                layer[i].ID = rawScheme[layerNum, i - 1].ID;
                switch (rawScheme[layerNum, i - 1].Set)
                {
                    case ColorType.Normal:
                        layer[i].Height = layer[i - 1].Height;
                        break;
                    case ColorType.Dark:
                        layer[i].Height = layer[i - 1].Height - 1;
                        break;
                    case ColorType.Light:
                        layer[i].Height = layer[i - 1].Height + 1;
                        break;
                }
                if (i != 1 && (i - 1) % 128 == 0)
                    layer[i].Height = 0;
                minHeight = layer[i].Height < minHeight ? layer[i].Height : minHeight;
            }
            for (int i = 0; i < rawScheme.GetLength(1) + 1; ++i)
            {
                layer[i].Height = layer[i].Height - minHeight;
                if (layer[i].Height > MaxHeight)
                    MaxHeight = layer[i].Height;
            }
            return layer;
        }

        SettedBlock[] GenerateSegmentedLayer(int layerNum, out int MaxHeight)
        {
            SettedBlock[] layer = new SettedBlock[rawScheme.GetLength(1) + 1];
            MaxHeight = 0;

            List<MBlock> mBlocks = new List<MBlock>();
            UnsettedBlock last = new UnsettedBlock { ID = -1, Set = ColorType.Normal };
            MBlock curMBlock = new MBlock();
            curMBlock.Add(new UnsettedBlock { ID = -1, Set = ColorType.Normal });
            bool isUp = rawScheme[layerNum, 0].Set == ColorType.Light;
            for (int i = 0; i < rawScheme.GetLength(1); ++i)
            {
                if (!isUp)
                {
                    if (rawScheme[layerNum, i].Set == ColorType.Dark || rawScheme[layerNum, i].Set == ColorType.Normal)
                        curMBlock.Add(rawScheme[layerNum, i]);
                    else
                    {
                        isUp = true;
                        curMBlock.Add(rawScheme[layerNum, i]);
                    }
                }
                else
                {
                    if (rawScheme[layerNum, i].Set == ColorType.Light || rawScheme[layerNum, i].Set == ColorType.Normal)
                        curMBlock.Add(rawScheme[layerNum, i]);
                    else
                    {
                        isUp = false;
                        mBlocks.Add(curMBlock.Clone() as MBlock);
                        curMBlock = new MBlock();
                        curMBlock.Add(rawScheme[layerNum, i]);
                    }
                }
            }
            mBlocks.Add(curMBlock.Clone() as MBlock);

            foreach (MBlock mb in mBlocks)
                mb.Calculate();
            int minH = 0;
            for (int j = 1; j < mBlocks.Count; ++j)
            {
                if (mBlocks[j].StartH >= mBlocks[j - 1].EndH)
                {
                    mBlocks[j].Shift = mBlocks[j].StartH - mBlocks[j - 1].EndH + 1;
                    if (minH > mBlocks[j].Shift)
                        minH = mBlocks[j].Shift;
                    for (int k = j - 1; k >= 0; --k)
                        if (mBlocks[k].EndH <= mBlocks[k + 1].StartH)
                            mBlocks[k].Shift += mBlocks[k + 1].StartH - mBlocks[k].EndH + 1;
                }
            }

            int curPos = -1;
            for (int j = 0; j < mBlocks.Count; ++j)
            {
                if (MaxHeight < mBlocks[j].StartH)
                    MaxHeight = mBlocks[j].StartH;
                if (MaxHeight < mBlocks[j].EndH)
                    MaxHeight = mBlocks[j].EndH;
                SettedBlock[] cur = mBlocks[j].Get();
                for (int k = 0; k < cur.Length; ++k)
                    layer[++curPos] = cur[k];
            }

            return layer;
        }

        class MBlock : ICloneable
        {
            private int _startH;
            private int _endH;

            private List<UnsettedBlock> _UBlocks = new List<UnsettedBlock>();
            private SettedBlock[] _SBlocks;


            public int StartH { get => _startH - Shift; private set => _startH = value; }
            public int EndH { get => _endH - Shift; private set => _endH = value; }

            public int Shift { get; set; } = 0;

            public void Add(UnsettedBlock block) => _UBlocks.Add(block);

            public SettedBlock[] Get()
            {
                if (_SBlocks == null)
                    throw new NullReferenceException("Trying to get not calculated MBlock");
                return _SBlocks;
            }

            public void Calculate()
            {
                _SBlocks = new SettedBlock[_UBlocks.Count];
                int minHeight = 0;
                _SBlocks[0].ID = _UBlocks[0].ID;
                _SBlocks[0].Height = 0;
                for (int i = 1; i < _UBlocks.Count; ++i)
                {
                    _SBlocks[i].ID = _UBlocks[i].ID;
                    switch (_UBlocks[i].Set)
                    {
                        case ColorType.Normal:
                            _SBlocks[i].Height = _SBlocks[i - 1].Height;
                            break;
                        case ColorType.Dark:
                            _SBlocks[i].Height = _SBlocks[i - 1].Height - 1;
                            break;
                        case ColorType.Light:
                            _SBlocks[i].Height = _SBlocks[i - 1].Height + 1;
                            break;
                    }
                    if (_SBlocks[i].Height < minHeight)
                        minHeight = _SBlocks[i].Height;
                }
                for (int i = 0; i < _SBlocks.Length; ++i)
                    _SBlocks[i].Height -= minHeight;
                _startH = _SBlocks[0].Height;
                _endH = _SBlocks[_SBlocks.Length - 1].Height;
            }

            public object Clone() => new MBlock
            {
                _startH = _startH,
                _endH = _endH,
                _UBlocks = new List<UnsettedBlock>(_UBlocks)
            };
        }
    }
}