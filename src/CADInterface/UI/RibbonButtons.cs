using Autodesk.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CADInterface.UI
{
    
    public static class RibbonButtons
    {
        #region 智能设计项目

        //系统设置按钮
        private static RibbonButtonEX sysSetting;
        public static RibbonButtonEX SYSSetting
        {
            get
            {
                sysSetting = new RibbonButtonEX("系统设置", RibbonItemSize.Large, Orientation.Vertical, "init");
                sysSetting.SetImg(Config.CurPath + "Images\\cog-outline.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "系统设置";
                toolTip.Content = "系统设置";
                toolTip.Command = "init";
                toolTip.ExpandedContent = "用init命令，打开设置界面进行配置";
                string imgToolTipFileName = Config.CurPath + "Images\\cog-outline.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                sysSetting.ToolTip = toolTip;
                //鼠标进入时的图片
                sysSetting.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return sysSetting;
            }
        }

        //模型生成
        private static RibbonButtonEX genModelBT;
        public static RibbonButtonEX GenModelBT
        {
            get
            {
                genModelBT = new RibbonButtonEX("生成模型", RibbonItemSize.Large, Orientation.Vertical, "GenModel");
                genModelBT.SetImg(Config.CurPath + "Images\\laravel.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "生成模型";
                toolTip.Content = "生成模型";
                toolTip.Command = "GenModel";
                toolTip.ExpandedContent = "";
                string imgToolTipFileName = Config.CurPath + "Images\\laravel.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                genModelBT.ToolTip = toolTip;
                //鼠标进入时的图片
                genModelBT.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return genModelBT;
            }
        }


        //主拱圈一般构造图按钮
        private static RibbonButtonEX archBtn;
        public static RibbonButtonEX ArchBtn
        {
            get
            {
                archBtn = new RibbonButtonEX("主拱构造", RibbonItemSize.Large, Orientation.Vertical, "ArchDraw");
                archBtn.SetImg(Config.CurPath + "Images\\bridge.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "主拱构造";
                toolTip.Content = "主拱构造";
                toolTip.Command = "ArchDraw";
                toolTip.ExpandedContent = "主拱构造";
                string imgToolTipFileName = Config.CurPath + "Images\\bridge.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                archBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                archBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return archBtn;
            }
        }

        //GA按钮
        private static RibbonButtonEX gaBtn;
        public static RibbonButtonEX GABtn
        {
            get
            {
                gaBtn = new RibbonButtonEX("一般布置图", RibbonItemSize.Large, Orientation.Vertical, "GeneralArrangement");
                gaBtn.SetImg(Config.CurPath + "Images\\平面.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "一般布置图";
                toolTip.Content = "一般布置图";
                toolTip.Command = "GeneralArrangement";
                toolTip.ExpandedContent = "一般布置图";
                string imgToolTipFileName = Config.CurPath + "Images\\平面.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                gaBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                gaBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return gaBtn;
            }
        }

        //立柱绘制
        private static RibbonButtonEX colBtn;
        public static RibbonButtonEX ColBtn
        {
            get
            {
                colBtn = new RibbonButtonEX("立柱构造", RibbonItemSize.Large, Orientation.Vertical, "ColumnDraw");
                colBtn.SetImg(Config.CurPath + "Images\\ladder.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "立柱构造";
                toolTip.Content = "立柱构造";
                toolTip.Command = "ColumnDraw";
                toolTip.ExpandedContent = "";
                string imgToolTipFileName = Config.CurPath + "Images\\ladder.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                colBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                colBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return colBtn;
            }
        }


        
        //立柱绘制
        private static RibbonButtonEX segBtn;
        public static RibbonButtonEX SegBtn
        {
            get
            {
                segBtn = new RibbonButtonEX("节段构造", RibbonItemSize.Large, Orientation.Vertical, "DrawSegment");
                segBtn.SetImg(Config.CurPath + "Images\\ladder.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "节段构造";
                toolTip.Content = "节段构造";
                toolTip.Command = "DrawSegment";
                toolTip.ExpandedContent = "";
                string imgToolTipFileName = Config.CurPath + "Images\\ladder.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                segBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                segBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return segBtn;
            }
        }

        private static RibbonButtonEX listBtn;
        public static RibbonButtonEX ListBtn
        {
            get
            {
                listBtn = new RibbonButtonEX("导出坐标表", RibbonItemSize.Large, Orientation.Vertical, "ListArch");
                listBtn.SetImg(Config.CurPath + "Images\\file.png");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "导出坐标表";
                toolTip.Content = "导出坐标表";
                toolTip.Command = "ListArch";
                toolTip.ExpandedContent = "";
                string imgToolTipFileName = Config.CurPath + "Images\\file.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                listBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                listBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return listBtn;
            }
        }

        #endregion

    }
}
