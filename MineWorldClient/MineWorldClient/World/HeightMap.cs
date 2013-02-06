using System;

namespace MineWorld.World
{
    public class HeightMap
    {
        public float[,] Heights { get; set; }
        public int Size { get; set; }

        public HeightMap(int size)
        {
            Size = size;
            Heights = new float[Size, Size];
            Perlin = new PerlinGenerator(new Random().Next(-50000, 50000));
            AddPerlinNoise(6f);
            Perturb(10, 10);
            for (int i = 0; i < 10; i++)
            {
                Erode(8);
            }
            Smoothen();
        }

        private PerlinGenerator Perlin { get; set; }

        public void AddPerlinNoise(float f)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Heights[i, j] += Perlin.Noise(f * i / Size, f * j / Size, 0);
                }
            }
        }

        public void Perturb(float f, float d)
        {
            float[,] temp = new float[Size, Size];
            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    int u = i + (int)(Perlin.Noise(f * i / Size, f * j / Size, 0) * d);
                    int v = j + (int)(Perlin.Noise(f * i / Size, f * j / Size, 1) * d);
                    if (u < 0) u = 0; if (u >= Size) u = Size - 1;
                    if (v < 0) v = 0; if (v >= Size) v = Size - 1;
                    temp[i, j] = Heights[u, v];
                }
            }
            Heights = temp;
        }

        public void Erode(float smoothness)
        {
            for (int i = 1; i < Size - 1; i++)
            {
                for (int j = 1; j < Size - 1; j++)
                {
                    float dMax = 0.0f;
                    int[] match = { 0, 0 };

                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {
                            if (Math.Abs(u) + Math.Abs(v) > 0)
                            {
                                float dI = Heights[i, j] - Heights[i + u, j + v];
                                if (dI > dMax)
                                {
                                    dMax = dI;
                                    match[0] = u; match[1] = v;
                                }
                            }
                        }
                    }

                    if (0 < dMax && dMax <= (smoothness / Size))
                    {
                        float dH = 0.5f * dMax;
                        Heights[i, j] -= dH;
                        Heights[i + match[0], j + match[1]] += dH;
                    }
                }
            }
        }

        public void Smoothen()
        {
            for (int i = 1; i < Size - 1; ++i)
            {
                for (int j = 1; j < Size - 1; ++j)
                {
                    float total = 0.0f;
                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {
                            total += Heights[i + u, j + v];
                        }
                    }

                    Heights[i, j] = total / 9.0f;
                }
            }
        }
    }
}
