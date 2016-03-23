using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;

namespace Filters
{
    abstract class Filter
    {
        protected abstract Color calcNewPixelColor(Bitmap sourceImage, int x, int y);

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calcNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    }

    class InvertFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb( 255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);

            return resultColor;
        }
    }

    class GrayScaleFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            int resultR = Clamp((int)Intensity, 0, 255);
            int resultG = resultR;
            int resultB = resultR;

            Color resultColor = Color.FromArgb(resultR, resultG, resultB);

            return resultColor;
        }
    }

    class SepiaFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int coef = 30;
            Color sourceColor = sourceImage.GetPixel(x, y);

            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            int resultR = Clamp((int)(Intensity + 2* coef), 0, 255);
            int resultG = Clamp((int)(Intensity + 0.5* coef), 0, 255);
            int resultB = Clamp((int)(Intensity - coef), 0, 255);

            Color resultColor = Color.FromArgb(resultR, resultG, resultB);

            return resultColor;
        }
    }

    class BrightnessIncFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int coef = 30;
            Color sourceColor = sourceImage.GetPixel(x, y);
            
            int resultR = Clamp((int)(sourceColor.R + coef), 0, 255);
            int resultG = Clamp((int)(sourceColor.G + coef), 0, 255);
            int resultB = Clamp((int)(sourceColor.B + coef), 0, 255);

            Color resultColor = Color.FromArgb(resultR, resultG, resultB);

            return resultColor;
        }
    }

    class TransferFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int k = Clamp(x + 50, 0, sourceImage.Width - 1);
            int l = Clamp(y, 0, sourceImage.Height - 1);
            Color Color = sourceImage.GetPixel(k, l);

            return Color;
        }
    }

    class TurnFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int x0 = (int)((sourceImage.Width - 1) / 2);
            int y0 = (int)((sourceImage.Height - 1) / 2);
            int k = Clamp((int)((x - x0) / Math.Sqrt(2) - (y - y0) / Math.Sqrt(2) + x0), 0, sourceImage.Width - 1);
            int l = Clamp((int)((x - x0) / Math.Sqrt(2) + (y - y0) / Math.Sqrt(2) + y0), 0, sourceImage.Height - 1);
            Color Color = sourceImage.GetPixel(k, l);

            return Color;
        }
    }

    class Waves1Filter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int k = Clamp((int)(x + 20*Math.Sin(2*Math.PI*y / 60)), 0, sourceImage.Width - 1);
            int l = Clamp(y, 0, sourceImage.Height - 1);
            Color Color = sourceImage.GetPixel(k, l);

            return Color;
        }
    }

    class Waves2Filter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int k = Clamp(x, 0, sourceImage.Width - 1);
            int l = Clamp((int)(y + 20 * Math.Sin(2 * Math.PI * x / 30)), 0, sourceImage.Height - 1);
            Color Color = sourceImage.GetPixel(k, l);

            return Color;
        }
    }

    class GlassFilter : Filter
    {
        private Random rand = new Random(DateTime.Now.Millisecond);
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int k = Clamp((int)(x + ((rand.NextDouble() - 0.5) * 5)), 0, sourceImage.Width - 1);
            int l = Clamp((int)(y + ((rand.NextDouble() - 0.5) * 5)), 0, sourceImage.Height - 1);
            Color Color = sourceImage.GetPixel(k, l);

            return Color;
        }
    }

    class GrayWorldFilter : Filter
    {
        public Color AverageColor;

        public GrayWorldFilter(Bitmap Img)
        {
            AverageColor = Average(Img);
        }

        public Color Average(Bitmap sourceImage)
        {
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    resultR += sourceColor.R;
                    resultG += sourceColor.G;
                    resultB += sourceColor.B;
                }
            }
            resultR /= sourceImage.Width * sourceImage.Height;
            resultG /= sourceImage.Width * sourceImage.Height;
            resultB /= sourceImage.Width * sourceImage.Height;

            Color AvgColor = Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
            return AvgColor;
        }

        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color avgColor = AverageColor;
            float Avg = (avgColor.R + avgColor.G + avgColor.B) / 3;
            int resR = Clamp((int)(sourceColor.R * Avg / avgColor.R), 0, 255);
            int resG = Clamp((int)(sourceColor.G * Avg / avgColor.G), 0, 255);
            int resB = Clamp((int)(sourceColor.B * Avg / avgColor.B), 0, 255);
            Color resultColor = Color.FromArgb(resR, resG, resB);
            return resultColor;
        }
    }

    class DilationFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color resColor = Color.FromArgb(0, 0, 0);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Color nearbyColor = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if (nearbyColor.R > resColor.R || nearbyColor.G > resColor.G || nearbyColor.B > resColor.B)
                        resColor = nearbyColor;
                }
            }
            return resColor;
        }
    }

    class ErosionFilter : Filter
    {
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color resColor = Color.FromArgb(255, 255, 255);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Color nearbyColor = sourceImage.GetPixel(Clamp(x + i, 0, sourceImage.Width - 1), Clamp(y + j, 0, sourceImage.Height - 1));
                    if (nearbyColor.R < resColor.R || nearbyColor.G < resColor.G || nearbyColor.B < resColor.B)
                        resColor = nearbyColor;
                }
            }
            return resColor;
        }
    }

    class MedianFilter : Filter
    {
        class CompareColors : IComparer<Color>
        {
            public int Compare(Color c1, Color c2)
            {
                if ((c1.R + c1.G + c1.B) > (c2.R + c2.G + c2.B))
                    return 1;
                if ((c1.R + c1.G + c1.B) < (c2.R + c2.G + c2.B))
                    return -1;
                else
                    return 0;
            }
        }
        protected override Color calcNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int k = 0;
            Color[] nearbyColor = new Color[9];
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    nearbyColor[k++] = sourceImage.GetPixel(Clamp(x + i, 1, sourceImage.Width - 2), Clamp(y + j, 1, sourceImage.Height - 2));
            Array.Sort(nearbyColor, new CompareColors());
            return nearbyColor[4];
        }
    }

    class MatrixFilter : Filter
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calcNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceimage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceimage.Height - 1);
                    Color neighborColor = sourceimage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            // размер ядра
            int size = 2 * radius + 1;
            // ядро фильтра
            kernel = new float[size, size];
            // коэф нормировки ядра
            float norm = 0;
            // ядро линейного фильтра
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            // нормируем ядро
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }

        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class SobelFilter : MatrixFilter
    {
        protected override Color calcNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            kernel = new float[3, 3];
            kernel[0, 0] = -1;
            kernel[0, 1] = 0;
            kernel[0, 2] = 1;

            kernel[1, 0] = -2;
            kernel[1, 1] = 0;
            kernel[1, 2] = 2;

            kernel[2, 0] = -1;
            kernel[2, 1] = 0;
            kernel[2, 2] = 1;
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR1 = 0;
            float resultG1 = 0;
            float resultB1 = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceimage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceimage.Height - 1);
                    Color neighborColor = sourceimage.GetPixel(idX, idY);
                    resultR1 += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG1 += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB1 += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            kernel[0, 0] = -1;
            kernel[0, 1] = -2;
            kernel[0, 2] = -1;

            kernel[1, 0] = 0;
            kernel[1, 1] = 0;
            kernel[1, 2] = 0;

            kernel[2, 0] = 1;
            kernel[2, 1] = 2;
            kernel[2, 2] = 1;

            float resultR2 = 0;
            float resultG2 = 0;
            float resultB2 = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceimage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceimage.Height - 1);
                    Color neighborColor = sourceimage.GetPixel(idX, idY);
                    resultR2 += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG2 += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB2 += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            int resultR = Clamp((int)(Math.Sqrt(Math.Pow(resultR1, 2) + Math.Pow(resultR2, 2))), 0, 255);
            int resultG = Clamp((int)(Math.Sqrt(Math.Pow(resultG1, 2) + Math.Pow(resultG2, 2))), 0, 255);
            int resultB = Clamp((int)(Math.Sqrt(Math.Pow(resultB1, 2) + Math.Pow(resultB2, 2))), 0, 255);

            return Color.FromArgb(resultR, resultG, resultB);
        }
    }

    class HarshnessFilter : MatrixFilter
    {
        public HarshnessFilter()
        {
            kernel = new float[3, 3];

            kernel[0, 0] = -1;
            kernel[0, 1] = -1;
            kernel[0, 2] = -1;

            kernel[1, 0] = -1;
            kernel[1, 1] = 9;
            kernel[1, 2] = -1;

            kernel[2, 0] = -1;
            kernel[2, 1] = -1;
            kernel[2, 2] = -1;
        }
    }

    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter()
        {
            int size = 5;
            kernel = new float[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        kernel[i, j] = 1.0f / size;
                    else
                        kernel[i, j] = 0;
                }
        }
    }
}
