﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace SvgToPng
{
    public partial class MainWindow : Window, IConvertProgress, ISaveProgress
    {
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<string> ReferencePaths { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Items = new ObservableCollection<Item>();
            ReferencePaths = new ObservableCollection<string>();
#if DEBUG
            TextOutputPath.Text = Path.Combine(Directory.GetCurrentDirectory(), "png");
            ReferencePaths = new ObservableCollection<string>(new string[]
            {
                @"c:\DOWNLOADS\GitHub\Svg.Skia\externals\SVG\Tests\W3CTestSuite\png\",
                @"c:\DOWNLOADS\GitHub-Forks\resvg-test-suite\png\",
                @"e:\Dropbox\Draw2D\SVG\vs2017-png\",
                @"e:\Dropbox\Draw2D\SVG\W3CTestSuite-png\"
            });
#endif
            this.Loaded += MainWindow_Loaded;
            DataContext = this;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            items.SelectionChanged += Items_SelectionChanged;
        }

        private void Items_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            skelement.InvalidateVisual();
            glhost.Child?.Invalidate();
        }

        private void OnGLControlHost(object sender, EventArgs e)
        {
            var glControl = new SKGLControl();
            glControl.PaintSurface += OnPaintGL;
            glControl.Dock = System.Windows.Forms.DockStyle.None;
            var host = (WindowsFormsHost)sender;
            host.Child = glControl;
        }

        private void OnPaintGL(object sender, SKPaintGLSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            canvas.Clear(SKColors.White);

            if (items.SelectedItem is Item item)
            {
                if (item.Svg?.Picture != null)
                {
                    float pwidth = item.Svg.Picture.CullRect.Width;
                    float pheight = item.Svg.Picture.CullRect.Height;
                    if (pwidth > 0f && pheight > 0f)
                    {
                        skelement.Width = pwidth;
                        skelement.Height = pheight;
                        //glhost.Width = pwidth;
                        //glhost.Height = pheight;
                        canvas.DrawPicture(item.Svg.Picture);
                    }
                }
            }
        }

        public async Task HandleDrop(string[] paths, string referencePath)
        {
            var inputFiles = SvgToPngConverter.GetFilesDrop(paths).ToList();
            if (inputFiles.Count > 0)
            {
                await SvgToPngConverter.Convert(inputFiles, Items, referencePath, this);
            }
        }

        public async Task ConvertStatusReset()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
            StackPanelStatus.Visibility = Visibility.Visible;
        }

        public async Task ConvertStatus(string message)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = message;
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
        }

        public async Task ConvertStatusProgress(int count, int total, string inputFile)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Conterting file {count}/{total}";
                TextInputFile.Text = $"{inputFile}";
            });
        }

        public async Task ConvertStatusDone()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Done";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
            StackPanelStatus.Visibility = Visibility.Collapsed;
        }

        public async Task SaveStatusReset()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
            StackPanelStatus.Visibility = Visibility.Visible;
        }

        public async Task SaveStatusProgress(int count, int total, string inputFile, string outputFile)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Saving file {count}/{total}";
                TextInputFile.Text = $"{inputFile}";
                TextOutputFile.Text = $"{outputFile}";
            });
        }

        public async Task SaveStatusDone()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Done";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
            StackPanelStatus.Visibility = Visibility.Collapsed;
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths != null && paths.Length > 0)
                {
                    await HandleDrop(paths, TextReferencePath.Text);
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            var items = Items.ToList();

            Items.Clear();

            foreach (var item in items)
            {
                item.Svg?.Dispose();
                item.Image = null;
            }
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Supported Files (*.svg;*.svgz)|*.svg;*.svgz|Svg Files (*.svg)|*.svg;|Svgz Files (*.svgz)|*.svgz|All Files (*.*)|*.*",
                Multiselect = true,
                FilterIndex = 0
            };
            if (dlg.ShowDialog() == true)
            {
                var paths = dlg.FileNames;
                if (paths != null && paths.Length > 0)
                {
                    await HandleDrop(paths, TextReferencePath.Text);
                }
            }
        }

        private async void ButtonSavePng_Click(object sender, RoutedEventArgs e)
        {
            string outputPath = TextOutputPath.Text;
            await SvgToPngConverter.Save(outputPath, Items, this);
        }
    }
}
