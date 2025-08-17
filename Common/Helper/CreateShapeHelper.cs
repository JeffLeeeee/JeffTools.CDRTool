using JeffTools.CDRTool.Common.Helpers;
using Svg;
using Svg.Transforms;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffTools.CDRTool.Common.Helper
{
    public class CreateShapeHelper
    {
        public ConfigHelper Config { get; set; } = new ConfigHelper();
        public string CreateCirleShape(int row,int column,float radius,float minRadius,float gap)
        {
            SvgDocument svgDoc = new SvgDocument();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    float x = j * (radius + gap);
                    float y = i * (radius + gap);
                    svgDoc.Children.Add(new SvgCircle
                    {
                        CenterX = (2 * j + 1) * radius + gap * j,
                        CenterY = (2 * i + 1) * radius + gap * i,
                        Radius = radius - (radius - minRadius) / row * (row - i),
                    });
                }
            }

            string tempFile = System.IO.Path.Combine(Config.GetTempPath(), $"shape{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.svg");
            svgDoc.Write(tempFile);
            return tempFile;
        }


        public string CreateRectangleShape(int row, int column, float width, float minWidth, float gap)
        {
            SvgDocument svgDoc = new SvgDocument();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    float x = j * (width + gap);
                    float y = i * (width + gap);
                    svgDoc.Children.Add(new SvgRectangle
                    {
                        X = x,
                        Y = y,
                        Width = width-(width-minWidth)/row*(row-i),
                        Height = width - (width - minWidth) / row * (row - i)
                    });
                }
            }
            string tempFile = System.IO.Path.Combine(Config.GetTempPath(), $"shape{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.svg");
            svgDoc.Write(tempFile);
            return tempFile;
        }

        public string CreateRhombusShape(int row, int column, float width, float minWidth, float gap)
        {
            SvgDocument svgDoc = new SvgDocument();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    float x = j * (width + gap);
                    float y = i * (width + gap);
                    SvgRectangle svgRect = new SvgRectangle()
                    {
                        X = (SvgUnit)((width * Math.Sqrt(2) + gap) * j),
                        Y = (SvgUnit)((width * Math.Sqrt(2) + gap) * i),
                        Width = width - (width - minWidth) / row * (row - i),
                        Height = width - (width - minWidth) / row * (row - i),
                    };
                    SvgRotate svgRotate = new SvgRotate(45)
                    {
                        CenterX = svgRect.X + svgRect.Width / 2,
                        CenterY = svgRect.Y + svgRect.Height / 2
                    };
                    if (svgRect.Transforms == null)
                    {
                        svgRect.Transforms = new SvgTransformCollection();
                    }
                    svgRect.Transforms.Add(svgRotate);
                    svgDoc.Children.Add(svgRect);
                }
            }
            string tempFile = System.IO.Path.Combine(Config.GetTempPath(), $"shape{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.svg");
            svgDoc.Write(tempFile);
            return tempFile;
        }
    }
}
