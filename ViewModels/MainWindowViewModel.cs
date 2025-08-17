
using JeffTools.CDRTool.Common;
using JeffTools.CDRTool.Common.Helper;
using JeffTools.CDRTool.Common.Helpers;
using JeffTools.CDRTool.Common.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using VgCore = Corel.Interop.VGCore;


namespace JeffTools.CDRTool.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly System.Windows.Threading.DispatcherTimer _timer;
        private VgCore.Application CdrApp { get; set; } = null;

        public ConfigHelper Config { get; set; } = new ConfigHelper();
        public CDRHelper CdrHelper { get; set; }

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



        private float _gap;

        public float Gap
        {
            get { return _gap; }
            set { SetProperty<float>(ref _gap, value); }
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




        private double _rectWidth;

        public double RectWidth
        {
            get { return _rectWidth; }
            set { SetProperty<double>(ref _rectWidth, value); }
        }

        private double _rectHeight;
        public double RectHeight
        {
            get { return _rectHeight; }
            set { SetProperty<double>(ref _rectHeight, value); }
        }

        private double _radiusUL;

        public double RadiusUL
        {
            get { return _radiusUL; }
            set { SetProperty<double>(ref _radiusUL, value); }
        }

        private double _radiusUR;

        public double RadiusUR
        {
            get { return _radiusUR; }
            set { SetProperty<double>(ref _radiusUR, value); }
        }

        private double _radiusLL;

        public double RadiusLL
        {
            get { return _radiusLL; }
            set { SetProperty<double>(ref _radiusLL, value); }
        }

        private double _radiusLR;
        public double RadiusLR
        {
            get { return _radiusLR; }
            set { SetProperty<double>(ref _radiusLR, value); }
        }   

        private bool _isResize;

        public bool IsResize
        {
            get { return _isResize; }
            set { SetProperty<bool>(ref _isResize, value); }
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



        private bool _isRevert;

        public bool IsRevert
        {
            get { return _isRevert; }
            set { SetProperty<bool>(ref _isRevert, value); }
        }

        private bool _isGray;

        public bool IsGray
        {
            get { return _isGray; }
            set { SetProperty<bool>(ref _isGray, value); }
        }

        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty<string>(ref _imagePath, value); }
        }

        private int _blocks;

        public int Blocks
        {
            get { return _blocks; }
            set { SetProperty<int>(ref _blocks, value); }
        }


        public RelayCommand ConvertToCurvesCommand => new RelayCommand((obj) =>
        {
            if (CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }
            if (CdrApp.ActiveSelection.Shapes.Count == 0)
            {
                System.Windows.MessageBox.Show("请先选择一个或多个图形", "提示", MessageBoxButton.OK);
                return;
            }
            CdrApp.ActiveSelection.ConvertToCurves();
        });



        public RelayCommand OpenImageFileCommand => new RelayCommand((obj) =>
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff",
                Title = "选择图片文件"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ImagePath = dialog.FileName;
            }
        });

        public RelayCommand CreateImageShapeCommand => new RelayCommand((obj) =>
        {
            if (CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(ImagePath))
            {
                System.Windows.MessageBox.Show("请先选择一张图片", "提示", MessageBoxButton.OK);
                return;
            }
            string tempFile = new CreateImageShapeHelper().CreateImageShape(ImagePath,Blocks,IsGray,IsRevert);
            VgCore.ImportFilter imageFilter = CdrApp.ActiveLayer.ImportEx(tempFile, VgCore.cdrFilter.cdrSVG);
            imageFilter.Finish();
            //移动到可视区域中心
            VgCore.ActiveView view = CdrApp.ActiveWindow.ActiveView;
            view.GetViewArea(out double x, out double y ,out double width,out double height);
            CdrApp.ActiveSelection.SetPosition(x+width/2- CdrApp.ActiveSelection.Shapes.First.SizeWidth / 2, y+height/2+CdrApp.ActiveSelection.Shapes.First.SizeHeight / 2);
        });


        public RelayCommand CreateSelectShapeCommand => new RelayCommand((obj) =>
        {
            if (CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }

            CdrApp.ActiveDocument.Unit = VgCore.cdrUnit.cdrMillimeter; // 设置单位为毫米
            if (CdrApp.ActiveSelection.Shapes.Count == 1)
            {
                VgCore.Shape sp = CdrApp.ActiveSelection.Shapes.First;
                //var fill = sp.Fill;
                //sp.Fill.UniformColor.RGBAssign(0, 0, 0);

                double posX = sp.PositionX;
                double posY = sp.PositionY;
                double width = sp.SizeWidth;
                double height = sp.SizeHeight;

                string savePath = System.IO.Path.Combine(Config.GetTempPath(), $"Image{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.jpg");

                VgCore.ExportFilter ef= CdrApp.ActiveDocument.ExportBitmap(savePath, VgCore.cdrFilter.cdrJPEG, VgCore.cdrExportRange.cdrSelection, VgCore.cdrImageType.cdrRGBColorImage,(int) width,(int)height,100,100);
                ef.Finish();
                //sp.Fill = fill; // 还原填充
                if (File.Exists(savePath))
                {
                    ImagePath = savePath;
                    string tempfile = new CreateImageShapeHelper().CreateImageShape(ImagePath, Blocks, IsGray, IsRevert);
                    VgCore.ImportFilter imageFilter = CdrApp.ActiveLayer.ImportEx(tempfile, VgCore.cdrFilter.cdrSVG);
                    imageFilter.Finish();
                    CdrApp.ActiveSelection.SetSize(width, height);
                    CdrApp.ActiveSelection.SetPosition(posX, posY);
                }
                else
                {   
                    System.Windows.MessageBox.Show("导出图片失败", "提示", MessageBoxButton.OK);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请仅选择一个图形。", "提示", MessageBoxButton.OK);
            }
        });



        public RelayCommand CreateShapeCommand => new RelayCommand((obj) =>
        {
            string tempFile = "";
            if (CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }

            switch(SelectedIndex)
            {
                case 0: // Circle
                    tempFile = new CreateShapeHelper().CreateCirleShape(Row, Column, Radius, MinRadius, Gap);
                    break;
                case 1: // Rectangle
                    tempFile = new CreateShapeHelper().CreateRectangleShape(Row, Column, Width, MinWidth, Gap);
                    break;
                case 2: // Rhombus
                    tempFile = new CreateShapeHelper().CreateRhombusShape(Row, Column, Width, MinWidth, Gap);
                    break;
                default:
                    System.Windows.MessageBox.Show("暂不支持该形状类型", "提示", MessageBoxButton.OK);
                    return;

            }

            VgCore.ImportFilter svgFilter = CdrApp.ActiveLayer.ImportEx(tempFile, VgCore.cdrFilter.cdrSVG);
            svgFilter.Finish();
            //移动到可视区域中心
            VgCore.ActiveView view = CdrApp.ActiveWindow.ActiveView;
            view.GetViewArea(out double x, out double y, out double width, out double height);
            CdrApp.ActiveSelection.SetPosition(x + width / 2- CdrApp.ActiveSelection.Shapes.First.SizeWidth/2, y + height / 2 + CdrApp.ActiveSelection.Shapes.First.SizeHeight / 2);

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


        public RelayCommand AddRectangleCommand=> new RelayCommand((obj) =>
        {
            if (CdrApp.ActiveDocument == null)
            {
                System.Windows.MessageBox.Show("请先打开一个CorelDRAW文档", "提示", MessageBoxButton.OK);
                return;
            }
            CdrHelper = new CDRHelper(CdrApp);
            CdrHelper.CreateRectForImage(true,RectWidth,RectHeight,RadiusUL,RadiusUR, RadiusLR, RadiusLL,IsResize);
        });

        private void InitProperties()
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
            _isRevert = false; // 默认不反转
            _isGray = true; // 默认不灰度化
            _blocks = 100; // 默认块数

            _radiusUL = 0; // 默认左边圆角半径
            _radiusUR = 0; // 默认上边圆角半径
            _radiusLL = 0; // 默认右边圆角半径
            _radiusLR = 0; // 默认下边圆角半径
            _isResize = false; // 默认不调整大小
        }

        private void InitApp()
        {
            InitProperties();

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
                System.Windows.MessageBox.Show($"CorelDRAW应用程序未运行", "提示", MessageBoxButton.OK);
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
                        if (process.MainModule.FileVersionInfo.FileDescription.Contains("CorelDRAW 2022"))
                        {

                            try
                            {
                                CdrApp = new VgCore.Application();
                                CdrVersion = CdrApp.Version.ToString();
                                IsRunning = true;
                                return; // 找到CorelDRAW程序后退出
                            }
                            catch (Exception)
                            {
                                IsRunning = false;
                                CdrApp = null;
                            }
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
