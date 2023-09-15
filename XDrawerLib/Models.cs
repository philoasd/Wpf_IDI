using System.Windows.Input;
using System.Windows.Media;

namespace XDrawerLib
{
    public enum Tool
    {
        None,
        Selection,
        Ink,
        //Line,
        //Rectangle,
        //Ellipse,
        //Text,
        MoveResize,
        //Triangle,
        Arrow,
        Custom,
        Highlight,
        Pan,

        //None,
        Line,// 直线
        Rectangle,// 矩形
        Circle,// 圆
        Triangle,// 三角形
        BrokenLine,// 折线
        Ellipse,// 椭圆
        SquareRounded,// 圆角矩形
        Barcode,// 条码
        Text,// 文本
        Image,// 图片
    }

    public enum KeyFunction
    {
        None,
        Line,// 直线
        Rectangle,// 矩形
        Circle,// 圆
        Triangle,// 三角形
        BrokenLine,// 折线
        Ellipse,// 椭圆
        SquareRounded,// 圆角矩形
        Barcode,// 条码
        Text,// 文本
        Image,// 图片
        //Selection,
        //Pan,
        //Ink,
        //Arrow,
        //Custom,
        PreserveSize,
        //Cancel,
        //Delete,
        //Undo,
        //Redo,
        //SelectAll
    }

    public class DrawerStyle
    {
        public Brush Background { get; set; }
        public Brush Border { get; set; }
        public double BorderSize { get; set; }
        public double FontSize { get; set; }
        public double Opacity { get; set; }

        public DrawerStyle(DrawerStyle style)
        {
            this.Background = style.Background;
            this.Border = style.Border;
            this.BorderSize = style.BorderSize;
            this.FontSize = style.FontSize;
            this.Opacity = style.Opacity;
        }

        public DrawerStyle()
        {

        }
    }

    public class HotKey
    {
        public Key PrimaryKey { get; set; }
        public Key SecondaryKey { get; set; }
    }
}