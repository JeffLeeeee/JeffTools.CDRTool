using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VgCore = Corel.Interop.VGCore;

namespace JeffTools.CDRTool.Common.Helper
{
    public class CDRHelper
    {

        private VgCore.Application _app;
        public CDRHelper(VgCore.Application app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            
        }

        public void CreateRectForImage(bool isSelect,double width,double height,double radiusUL,double radiusUR,double radiusLR ,double radiusLL,bool isResize)
        {
            double w;//矩形宽度
            double h;//矩形高度
            _app.ActiveDocument.Unit = VgCore.cdrUnit.cdrMillimeter; //设置单位为毫米
            VgCore.ShapeRange shapes;
            if (isSelect)
            {
                shapes = _app.ActiveDocument.SelectionRange;
            }
            else
            {
                shapes = _app.ActiveDocument.ActivePage.Shapes.Range();
            }
            if (shapes == null || shapes.Count == 0)
            {
                MessageBox.Show("啥都没选我咋办啊。");
            }
            else
            {
                foreach (VgCore.Shape shape in shapes)
                {
                    if(shape.Type == VgCore.cdrShapeType.cdrBitmapShape)
                    {
                        //获取图片的宽高
                        
                        if (isResize)
                        {
                            w = width;
                            h = height;
                        }
                        else
                        {
                            w = shape.SizeWidth;
                            h = shape.SizeHeight;
                        }

                        //创建一个矩形
                        VgCore.Rect rect = new VgCore.Rect() { 
                            Width = w,
                            Height = h,
                            x = shape.PositionX,
                            y = shape.PositionY-shape.SizeHeight,
                        };

                        VgCore.Shape s = _app.ActiveLayer.CreateRectangleRect(rect,radiusUL,radiusUR,radiusLR,radiusLL);

                        //根据s的尺寸，图片按比例调整尺寸满铺s
                        double scaleX = s.SizeWidth / shape.SizeWidth;
                        double scaleY = s.SizeHeight / shape.SizeHeight;
                        double scale = Math.Max(scaleX, scaleY);
                        shape.SizeWidth = shape.SizeWidth * scale;
                        shape.SizeHeight = shape.SizeHeight * scale;

                        shape.AddToPowerClip(s,VgCore.cdrTriState.cdrTrue);
                        //调整内容为按比例填充


                    }

                }
            }
        }

       
    }
}
