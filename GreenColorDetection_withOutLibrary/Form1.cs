
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;



namespace GreenColorDetection_withOutLibrary
{

    public partial class Form1 : Form
    {
 
        Bitmap image;
        Bitmap originalimage;


        int grayValue;
        int[,] matrix;


        public Form1()
        {

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void openImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                originalimage = image;
                pictureBox1.Image = image;
            }
            else
            {
                throw new Exception("No file selected.");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            GreenDetector(image);

        }

        public void GreenDetector(Bitmap image)
        {

            image = GreenColorThresholding(image, 100);

            image = ConvertToGrayAndBlackAndWhite(image);

            matrix = ConvertToMatrix(image);

            DrawBoundingBox(matrix);


        }

        private void detect_Click(object sender, EventArgs e)
        {
            GreenDetector(image);

        }
        public static Bitmap GreenColorThresholding(Bitmap image, int threshold)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    if (pixelColor.G >= threshold && pixelColor.G > pixelColor.R && pixelColor.G > pixelColor.B)
                    {
                        result.SetPixel(i, j, Color.Green);
                    }
                    else
                    {
                        result.SetPixel(i, j, Color.Black);
                    }
                }
            }

            return result;
        }
        public Bitmap ConvertToGrayAndBlackAndWhite(Bitmap image)
        {
            Bitmap bw = new Bitmap(image.Width, image.Height);


            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = image.GetPixel(i, j);
                    grayValue = (color.R + color.G + color.B) / 3;
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    bw.SetPixel(i, j, grayColor);
                    Color c = bw.GetPixel(i, j);
                    int gray = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bw.SetPixel(i, j, gray < grayValue ? Color.Black : Color.White);


                }
            }
            pictureBox2.Image = bw;
            return bw;
        }
        public static int[,] ConvertToMatrix(Bitmap bw)
        {
            int width = bw.Width;
            int height = bw.Height;
            int[,] matrix = new int[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color c = bw.GetPixel(i, j);
                    matrix[i, j] = c.R == 0 ? 1 : 0;
                }
            }

            return matrix;
        }

        public void DrawBoundingBox(int[,] matrix)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            Bitmap bmp = new Bitmap(width, height);

            // پیمایش تصویر با الگوریتم DFS
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (matrix[i, j] == 1)
                    {

                        int startX = i;
                        int startY = j;


                        int minX = i;
                        int minY = j;
                        int maxX = i;
                        int maxY = j;


                        Stack<(int, int)> stack = new Stack<(int, int)>();
                        stack.Push((i, j));

                        while (stack.Count > 0)
                        {
                            (int x, int y) = stack.Pop();
                            if (x >= 0 && x < width && y >= 0 && y < height && matrix[x, y] == 1)
                            {
                                matrix[x, y] = 0;
                                minX = Math.Min(minX, x);
                                minY = Math.Min(minY, y);
                                maxX = Math.Max(maxX, x);
                                maxY = Math.Max(maxY, y);
                                stack.Push((x + 1, y));
                                stack.Push((x - 1, y));
                                stack.Push((x, y + 1));
                                stack.Push((x, y - 1));
                            }
                        }

                        // رسم خط قرمز دور شکل های سیاه
                        for (int k = minX; k <= maxX; k++)
                        {
                            originalimage.SetPixel(k, minY, Color.Red);
                            originalimage.SetPixel(k, maxY, Color.Red);
                        }
                        for (int k = minY; k <= maxY; k++)
                        {
                            originalimage.SetPixel(minX, k, Color.Red);
                            originalimage.SetPixel(maxX, k, Color.Red);
                        }
                    }
                }
            }
            pictureBox3.Image = originalimage;


        }


    }

}