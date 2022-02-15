using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void MainDialog_Load(object sender, EventArgs e)
        {

        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            // 画直线  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red);
            pen.Width = 2;
            Point startPoint = new Point(20, 20);
            Point endPoint = new Point(70, 20);
            gra.DrawLine(pen, startPoint, endPoint);

            pen.Dispose();
            gra.Dispose();
        }

        private void rectangleButton_Click(object sender, EventArgs e)
        {
            //画矩形  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red);
            gra.DrawRectangle(pen, 20, 50, 100, 100);
            pen.Dispose();
            gra.Dispose();
        }
        private void cyliderButton_Click(object sender, EventArgs e)
        {
            //圆柱体,有许多个椭圆有底部逐渐叠起来的，最后填充颜色  

            int height = this.ClientSize.Height - 150;
            int width = this.ClientSize.Width - 50;
            int vHeight = 200;
            int vWidth = 100;
            Graphics gra = this.CreateGraphics();
            gra.Clear(Color.White);
            Pen pen = new Pen(Color.Gray, 2);
            SolidBrush brush = new SolidBrush(Color.Gainsboro);

            for (int i = height / 2; i > 0; i--)
            {
                gra.DrawEllipse(pen, width / 2, i, vHeight, vWidth);
            }

            gra.FillEllipse(brush, width / 2, 0, vHeight, vWidth);
        }

        private void fillRectangleButton_Click(object sender, EventArgs e)
        {
            //画矩形  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red, 3);
            Brush brush = pen.Brush;
            Rectangle rect = new Rectangle(20, 50, 100, 100);
            gra.FillRectangle(brush, rect);
            gra.Dispose();
        }




        private void fontButton_Click(object sender, EventArgs e)
        {
            Graphics gra = this.CreateGraphics();
            Font font = new Font("隶书", 24, FontStyle.Italic);
            Pen pen = new Pen(Color.Blue, 3);
            gra.DrawString("Windows应用程序设计", font, pen.Brush, 10, 100);
        }

        private void ellispeButton_Click(object sender, EventArgs e)
        {
            // 画圆形  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red);
            gra.DrawEllipse(pen, 0, 0, 200, 100);
            pen.Dispose();
            gra.Dispose();
        }

        private void moveEllispeButton_Click(object sender, EventArgs e)
        {
            // 移动圆形  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red);
            gra.TranslateTransform(10, 10);// 改变起坐标(10,10)  
            gra.DrawEllipse(pen, 0, 0, 200, 100);

            gra.Dispose();
        }

        private void scaleEllispeButton_Click(object sender, EventArgs e)
        {
            // 缩放圆形  
            float xScale = 1.5F;
            float yScale = 2F;
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Red);
            gra.ScaleTransform(xScale, yScale);// X轴放大1.5倍， Y轴放大2倍  
            gra.DrawEllipse(pen, 0, 0, 200, 100);
            gra.Dispose();
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            //绘制曲线  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Blue, 3);
            Point oo1 = new Point(30, this.ClientSize.Height - 100);
            Point oo2 = new Point(this.ClientSize.Width - 50, this.ClientSize.Height - 100);
            gra.DrawLine(pen, oo1, oo2);
            Point oo3 = new Point(30, 30);
            gra.DrawLine(pen, oo1, oo3);
            Font font = new System.Drawing.Font("宋体", 12, FontStyle.Bold);
            gra.DrawString("X", font, pen.Brush, oo2);
            gra.DrawString("Y", font, pen.Brush, 10, 10);

            int x1 = 0, x2 = 0;
            double a = 0;
            double y1 = 0, y2 = this.ClientSize.Height - 100;
            for (x2 = 0; x2 < this.ClientSize.Width; x2++)
            {
                a = 2 * Math.PI * x2 / (this.ClientSize.Width);
                y2 = Math.Sin(a);
                y2 = (1 - y2) * (this.ClientSize.Height - 100) / 2;
                gra.DrawLine(pen, x1 + 30, (float)y1, x2 + 30, (float)y2);
                x1 = x2;
                y1 = y2;
            }
            gra.Dispose();
        }

        private void piechartButton_Click(object sender, EventArgs e)
        {
            //饼图  
            Graphics gra = this.CreateGraphics();
            Pen pen = new Pen(Color.Blue, 3);
            Rectangle rect = new Rectangle(50, 50, 200, 100);
            Brush brush = new SolidBrush(Color.Blue);
            gra.FillPie(pen.Brush, rect, 0, 60);
            gra.FillPie(brush, rect, 60, 150);
            brush = new SolidBrush(Color.Yellow);
            gra.FillPie(brush, rect, 210, 150);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            rectangleButton_Click(sender, e);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics; //创建画板,这里的画板是由Form提供的.
            Pen p = new Pen(Color.Blue, 2);//定义了一个蓝色,宽度为的画笔
            g.DrawLine(p, 10, 10, 100, 100);//在画板上画直线,起始坐标为(10,10),终点坐标为(100,100)
            g.DrawRectangle(p, 10, 10, 100, 100);//在画板上画矩形,起始坐标为(10,10),宽为,高为
            g.DrawEllipse(p, 10, 10, 100, 100);//在画板上画椭圆,起始坐标为(10,10),外接矩形的宽为,高为
        }

    }
}
