using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;

namespace DoomFireSharp
{
    public class DoomFire
    {
        private readonly SKCanvasView _skiaCanvasView;
        private readonly SKColor[] _firePalette = new SKColor[37];
        private readonly Random _random;

        private int _height;
        private int _width;
        private int _pixelSize;
        private int[] _firePixels;
        private bool _shouldDraw;
        private SKPaint _paint;

        public DoomFire(SKCanvasView skiaCanvasView)
        {
            _random = new Random();
            _skiaCanvasView = skiaCanvasView;
            _skiaCanvasView.PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Save();

            canvas.Scale(_pixelSize);

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    SetPaintColor(y, x);
                    canvas.DrawRect(GetPixelRect(x, y), _paint);
                }
            }

            canvas.Restore();
        }

        private void SetPaintColor(int y, int x)
        {
            var index = _firePixels[y * _width + x];
            var pixel = _firePalette[index];

            _paint.Color = pixel;
        }

        private SKRect GetPixelRect(int x, int y)
        {
            return SKRect.Create(x, y, 1, 1);
        }

        private void DoFire()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 1; y < _height; y++)
                {
                    SpreadFire(y * _width + x);
                }
            }
        }

        private void SpreadFire(int src)
        {
            var pixel = _firePixels[src];
            if (pixel == 0)
            {
                _firePixels[src - _width] = 0;
            }
            else
            {
                var randIdx = (int)Math.Round(_random.NextDouble() * 3.0);
                var dst = src - randIdx + 1;
                _firePixels[Wrap(dst - _width, _firePixels.Length - 1)] = pixel - (randIdx & 1);
            }
        }

        private static int Wrap(int index, int n)
        {
            return ((index % n) + n) % n;
        }

        private void EnsureInitialized(int width, int height, int pixelSize)
        {
            _width = width;
            _height = height;
            _pixelSize = pixelSize;

            _firePixels = new int[_width * _height];

            _paint = new SKPaint { Style = SKPaintStyle.Fill };

            InitializePalette(_firePalette);
            InitializePixels(_firePixels, _width, _height);
        }

        private static void InitializePixels(int[] firePixels, int width, int height)
        {
            for (var i = 0; i < firePixels.Length; i++)
            {
                firePixels[i] = 0;
            }

            for (var i = 0; i < width; i++)
            {
                firePixels[(height - 1) * width + i] = 36;
            }
        }

        private static void InitializePalette(SKColor[] firePalette)
        {
            for (var i = 0; i < firePalette.Length; i++)
            {
                firePalette[i] = new SKColor(
                    FireColors.Colors[i * 3 + 0],
                    FireColors.Colors[i * 3 + 1],
                    FireColors.Colors[i * 3 + 2]);
            }
        }

        public void Run(int width, int height, int scale)
        {
            _shouldDraw = true;
            EnsureInitialized(width, height, scale);
            Task.Run(DrawLoop);
        }

        public void Stop()
        {
            _shouldDraw = false;
        }

        private async Task DrawLoop()
        {
            while (_shouldDraw)
            {
                DoFire();
                _skiaCanvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 30));
            }
        }
    }
}
