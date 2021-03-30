using System;
using Xamarin.Forms;

namespace DoomFireSharp
{
    public partial class MainPage : ContentPage
    {
        private DoomFire _doomFire;

        private int _width = 360;
        private int _height = 300;
        private int _scale = 4;

        public MainPage()
        {
            InitializeComponent();

            WidthSlider.Value = _width;
            WidthValue.Text = _width.ToString();
            HeightSlider.Value = _height;
            HeightValue.Text = _height.ToString();
            ScaleSlider.Value = _scale;
            ScaleValue.Text = _scale.ToString();

            _doomFire = new DoomFire(SkiaCanvasView);
        }

        private void Stop()
        {
            _doomFire?.Stop();
            StartButton.IsEnabled = true;
        }

        private void Start()
        {
            _doomFire.Run(_width, _height, _scale);
            StartButton.IsEnabled = false;
        }

        private void StartClicked(object sender, EventArgs e)
        {
            Start();
        }

        private void StopClicked(object sender, EventArgs e)
        {
            Stop();
        }

        private void WidthSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Stop();
            _width = (int)e.NewValue;
            WidthValue.Text = _width.ToString();
        }

        private void HeightSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Stop();
            _height = (int)e.NewValue;
            HeightValue.Text = _height.ToString();
        }

        private void ScaleSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Stop();
            _scale = (int)e.NewValue;
            ScaleValue.Text = _scale.ToString();
        }
    }
}
