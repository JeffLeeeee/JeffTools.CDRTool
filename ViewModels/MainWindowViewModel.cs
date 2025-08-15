
using JeffTools.CDRTool.Common.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using VgCore = Corel.Interop.VGCore;
using JeffTools.CDRTool.Common.Helpers;
using Svg;
using JeffTools.CDRTool.Common;


namespace JeffTools.CDRTool.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly System.Windows.Threading.DispatcherTimer _timer;
        private VgCore.Application CdrApp { get; set; } = null;

        public ConfigHelper Config { get; set; } = new ConfigHelper();

        public string CdrVersion { get; set; }

        private string _tempPath;

        public string TempPath
        {
            get { return _tempPath; }
            set { SetProperty<string>(ref _tempPath, value); }
        }


        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty<bool>(ref _isRunning, value); }
        }



        //Title
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }



        private int _gap;

        public int Gap
        {
            get { return _gap; }
            set { SetProperty<int>(ref _gap, value); }
        }


        private int _row;

        public int Row
        {
            get { return _row; }
            set { SetProperty<int>(ref _row, value); }
        }


        private int _column;

        public int Column
        {
            get { return _column; }
            set { SetProperty<int>(ref _column, value); }
        }


        private float _radius;

        public float Radius
        {
            get { return _radius; }
            set { SetProperty<float>(ref _radius, value); }
        }

        private float _minRadius;

        public float MinRadius
        {
            get { return _minRadius; }
            set { SetProperty<float>(ref _minRadius, value); }
        }

        private float _width;

        public float Width
        {
            get { return _width; }
            set { SetProperty<float>(ref _width, value); }
        }

        private float _minWidth;

        public float MinWidth
        {
            get { return _minWidth; }
            set { SetProperty<float>(ref _minWidth, value); }
        }

        public List<ShapeTypeEnums> ShapeTypes { get; set; } = new List<ShapeTypeEnums>
        {
            ShapeTypeEnums.Circle,
            ShapeTypeEnums.Rectangle,
            ShapeTypeEnums.Rhombus,
            ShapeTypeEnums.Star,
            ShapeTypeEnums.Triangle
        };

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty<int>(ref _selectedIndex, value); }
        }





        public RelayCommand CreateShapeCommand => new RelayCommand((obj) =>
        {
            SvgDocument svgDoc = new SvgDocument();

            if(CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }

            switch(SelectedIndex)
            {
                case 0: // Circle
                    for(int i = 0; i < _row; i++)
                    {
                        for(int j = 0; j < _column; j++)
                        {
                            float x = j * (Radius + Gap);
                            float y = i * (Radius + Gap);
                            svgDoc.Children.Add(new SvgCircle
                            {
                                CenterX = (2 * j + 1) * _radius + _gap * j,
                                CenterY = (2 * i + 1) * _radius + _gap * i,
                                Radius = _radius - (_radius - _minRadius) / _row * (_row - i),
                            });
                        }
                    }
                    break;
               
            }
            string tempFile = Path.Combine(TempPath, $"shape{DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss")}.svg");

            svgDoc.Write(tempFile);
            VgCore.ImportFilter svgFilter = CdrApp.ActiveLayer.ImportEx(tempFile, VgCore.cdrFilter.cdrSVG);
            svgFilter.Finish();

        });



        public RelayCommand OpenFolderCommand => new RelayCommand((obj) =>
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            if (!string.IsNullOrEmpty(dialog.SelectedPath))
            {
                TempPath = dialog.SelectedPath;
                Config.SetTempPath(TempPath);
            }

        });

        private void InitApp()
        {
            _title = "JeffTools-CDRTool";
            // 创建temp文件夹
            _tempPath = Config.GetTempPath();
            _selectedIndex = 0; // 默认选择第一个形状类型

            _gap = 10; // 默认间距
            _row = 50; // 默认行数
            _column = 50; // 默认列数
            _radius = 10; // 默认半径
            _minRadius = 5; // 默认最小半径
            _width = 20; // 默认宽度
            _minWidth = 10; // 默认最小宽度

            try
            {
                if (CdrApp == null)
                {
                    CdrApp = new VgCore.Application();
                }
                CdrVersion = CdrApp.Version.ToString();
                _isRunning = true;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show($"CorelDRAW 应用程序未运行", "提示", MessageBoxButton.OK);

                _isRunning = false;
            }
        }

        public RelayCommand RefreshCommand => new RelayCommand((obj) =>
        {
            try
            {
                if (CdrApp == null)
                {
                    CdrApp = new VgCore.Application();
                }
                CdrVersion = CdrApp.Version.ToString();
                RefreshCommand.CanExecute(null); // 触发 CanExecuteChanged 事件
                IsRunning = true;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show($"CorelDRAW 2017 应用程序未运行", "提示", MessageBoxButton.OK);
                IsRunning = false;
            }
        });

        public MainWindowViewModel()
        {
            InitApp();

            // 监听CorelDRAW的关闭事件
            _timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => OnCdrAppClosed();
            _timer.Start();
        }

        //监听CorelDRAW的关闭事件
        private void OnCdrAppClosed()
        {
            // 监听系统运行程序，如果没有CorelDraw程序时，设置IsRunning为false
            try
            {
                Process[] processes = Process.GetProcessesByName("CorelDRW");
                
                if (processes.Length == 0)
                {
                    IsRunning = false;
                    CdrApp = null;
                    //System.Windows.MessageBox.Show("CorelDRAW 应用程序已关闭", "提示", MessageBoxButton.OK);
                }
                else
                {
                    foreach (var process in processes)
                    {
                        if (process.MainWindowTitle.Contains("CorelDRAW 2017"))
                        {
                            CdrApp = new VgCore.Application();
                            CdrVersion = CdrApp.Version.ToString();
                            IsRunning = true;
                            return; // 找到CorelDRAW程序后退出
                        }
                        else
                        {
                            IsRunning = false;
                            CdrApp = null;
                        }
                    }
                }
               
              
                       
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButton.OK);
            }

        }

        private int selectedShapeType;

        public int SelectedShapeType { get => selectedShapeType; set => SetProperty(ref selectedShapeType, value); }
    }
}
