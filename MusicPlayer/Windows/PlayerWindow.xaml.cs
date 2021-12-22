using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MusicPlayer
{
    public partial class PlayerWindow : Window
    {
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();
        private DispatcherTimer _timer;
        private bool _musicIsPlayed;

        public PlayerWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 (*.mp3)|*.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                SetMusic(path);
            }
        }

        private void SetMusic(string path)
        {
            _mediaPlayer.Open(new Uri(path));

            _mediaPlayer.MediaOpened += (s, e) =>
            {
                SliderMusicDuration.Maximum = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                TextBlockEndTime.Text = _mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            };
            TextBlockMusicName.Text = path.Split('\\').Last();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer.Source != null)
            {
                TextBlockCurrentTime.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
                SliderMusicDuration.Value = (int)_mediaPlayer.Position.TotalSeconds;
            }

        }

        private void SliderMusicDuration_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((int)SliderMusicDuration.Value != (int)_mediaPlayer.Position.TotalSeconds)
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(SliderMusicDuration.Value);
                TextBlockCurrentTime.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer.Source == null)
                return;

            if (!_musicIsPlayed)
            {
                ImageButtonPlay.Source = new BitmapImage(new Uri(@"pack://application:,,,/MusicPlayer;component/Img/pause.png"));
                ImageButtonPlay.Margin = new Thickness(0, 0, 0, 0);
                _mediaPlayer.Play();
                _timer.Start();
            }
            else
            {
                ImageButtonPlay.Source = new BitmapImage(new Uri(@"pack://application:,,,/MusicPlayer;component/Img/play.png"));
                ImageButtonPlay.Margin = new Thickness(5, 0, 0, 0);
                _mediaPlayer.Pause();
            }

            _musicIsPlayed = !_musicIsPlayed;
        }

        private void SliderMusicDuration_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _mediaPlayer.Pause();
        }

        private void SliderMusicDuration_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _mediaPlayer.Play();
        }

        private void SliderMusicVolume_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = SliderMusicVolume.Value / 100;
        }
    }
}