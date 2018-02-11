﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Minecraft_staircase
{
    public partial class FormTopView : Form
    {
        const string BlockIDS = @"data\PossibleBlocks.txt";
        const int blockSize = 16;
        const int maxSize = 3;

        int[,] blockMap;
        Dictionary<int, Bitmap> textures;
        Dictionary<int, string> blockNames;

        Image originalImage;

        int curSize = 1;

        Color defMeshColor = Color.Black;
        Color chunkMeshColor = Color.Red;
        Color mapMeshColor = Color.Purple;

        public FormTopView()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(WheelRolled);
        }

        internal void Show(int[,] ids)
        {
            Show();
            blockMap = ids;
            LoadTextures();
            pictureBox1.Image = new Bitmap(ids.GetLength(0) * blockSize, ids.GetLength(1) * blockSize);
            pictureBox1.Width = ids.GetLength(0) * blockSize;
            pictureBox1.Height = ids.GetLength(1) * blockSize;
            CreateImage();
            originalImage = pictureBox1.Image;
            PrintMesh(pictureBox1.Image);
            PrintChunkMesh(pictureBox1.Image);
            PrintMapMesh(pictureBox1.Image);
        }

        void LoadTextures()
        {
            textures = new Dictionary<int, Bitmap>();
            textures.Add(-1, new Bitmap(@"data\Textures\overflow.png"));
            blockNames = new Dictionary<int, string>();
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                int id = 1;
                while (line != null)
                {
                    line = line.Split(',')[0];
                    if (line[0] != '/' && line[1] != '/')
                    {
                        textures.Add(id, new Bitmap($@"data\Textures\{line.Split(new char[] { '-' })[0]}.png"));
                        blockNames.Add(id++, line.Split(new char[] { '-' })[1]);
                    }
                    line = reader.ReadLine();
                }
            }
        }

        void CreateImage()
        {
            Graphics graph = Graphics.FromImage(pictureBox1.Image);
            for (int i = 0; i < blockMap.GetLength(0); i++)
                for (int j = 0; j < blockMap.GetLength(1); j++)
                    graph.DrawImage(textures[blockMap[i, j]], i * blockSize, j * blockSize);
        }


        bool isTopMost = false;
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            TopMost = isTopMost ? false : true;
            isTopMost = !isTopMost;
            ShowInfo(isTopMost ? "TopMost enabled" : "TopMost disabled", Color.LightPink);
        }


        void PrintMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) - 1; i++)
                graph.DrawLine(new Pen(defMeshColor, 1), new Point(blockSize * (i + 1), 0), new Point(blockSize * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) - 1; i++)
                graph.DrawLine(new Pen(defMeshColor, 1), new Point(0, blockSize * (i + 1)), new Point(image.Width, blockSize * (i + 1)));
        }

        void PrintChunkMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) / 16 - 1; i++)
                graph.DrawLine(new Pen(chunkMeshColor, 2), new Point(blockSize * 16 * (i + 1), 0), new Point(blockSize * 16 * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) - 1; i++)
                graph.DrawLine(new Pen(chunkMeshColor, 2), new Point(0, blockSize * 16 * (i + 1)), new Point(image.Width, blockSize * 16 * (i + 1)));
        }

        void PrintMapMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) / 128 - 1; i++)
                graph.DrawLine(new Pen(mapMeshColor, 2), new Point(blockSize * 128 * (i + 1), 0), new Point(blockSize * 128 * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) / 128 - 1; i++)
                graph.DrawLine(new Pen(mapMeshColor, 2), new Point(0, blockSize * 128 * (i + 1)), new Point(image.Width, blockSize * 128 * (i + 1)));
        }

        Point cur, curnew;
        bool moveImage;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            cur = Cursor.Position;
            moveImage = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveImage)
            {
                Point newLoc = pictureBox1.Location;
                if (Cursor.Position.X - cur.X + curnew.X <= 0)
                    //&& Cursor.Position.X - cur.X + curnew.X + pictureBox1.Width >= Width)
                    newLoc.X = Cursor.Position.X - cur.X + curnew.X;
                if (Cursor.Position.Y - cur.Y + curnew.Y <= 0)
                    //&& Cursor.Position.Y - cur.Y + curnew.Y >= Height - pictureBox1.Height + blockSize)
                    newLoc.Y = Cursor.Position.Y - cur.Y + curnew.Y;

                if (newLoc.X + pictureBox1.Width <= Width)
                    newLoc.X = Width - pictureBox1.Width - 15;
                if (newLoc.Y + pictureBox1.Height <= Height)
                    newLoc.Y = Height - pictureBox1.Height - 38;

                pictureBox1.Location = newLoc;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moveImage = false;
            curnew = pictureBox1.Location;
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int k = (int)Math.Pow(2, curSize - 1);
            ShowInfo(blockNames[blockMap[e.Location.X / (blockSize * k), e.Location.Y / (blockSize * k)]].ToString(), Color.Aquamarine);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            label1.Visible = false;
        }

        void ShowInfo(string text, Color col)
        {
            label1.Text = text;
            label1.BackColor = col;
            label1.Visible = true;
            timer1.Stop();
            timer1.Start();
        }

        private void FormTopView_Resize(object sender, EventArgs e)
        {
            Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            if (pictureBox1.Location.X + pictureBox1.Width <= Width)
                loc.X = Width - pictureBox1.Width - 15;
            if (pictureBox1.Location.Y + pictureBox1.Height <= Height)
                loc.Y = Height - pictureBox1.Height - 38;
            pictureBox1.Location = loc;
        }

        private void WheelRolled(object sender, MouseEventArgs e)
        {
            Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            int k = (int)Math.Pow(2, curSize);
            if (e.Delta < 0 && curSize > 1)
            {
                loc = new Point(pictureBox1.Location.X /*+ ((Width - 15) / k)*/, pictureBox1.Location.Y /*+ ((Height - 38) / k)*/);
                --curSize;
                pictureBox1.Width = pictureBox1.Width / 2;
                pictureBox1.Height = pictureBox1.Height / 2;               
            }
            else if (e.Delta > 0 && curSize < maxSize)
            {
                loc = new Point(pictureBox1.Location.X /*- ((Width - 15) / k)*/, pictureBox1.Location.Y /*- ((Height - 38) / k)*/);
                ++curSize;
                pictureBox1.Width = pictureBox1.Width * 2;
                pictureBox1.Height = pictureBox1.Height * 2;
            }         
            if (pictureBox1.Location.X + pictureBox1.Width <= Width)
                loc.X = Width - pictureBox1.Width - 15;
            if (pictureBox1.Location.Y + pictureBox1.Height <= Height)
                loc.Y = Height - pictureBox1.Height - 38;
            if (pictureBox1.Location.X > 0)
                loc.X = 0;
            if (pictureBox1.Location.Y > 0)
                loc.Y = 0;
            pictureBox1.Location = loc;
        }
    }
}