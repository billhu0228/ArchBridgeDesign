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


        //GA按钮
        private static RibbonButtonEX gaBtn;
        public static RibbonButtonEX GABtn
        {
            get
            {
                gaBtn = new RibbonButtonEX("绘制主拱", RibbonItemSize.Large, Orientation.Vertical, "GA");
                gaBtn.SetImg(Config.CurPath + "Images\\bridge.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "绘制主拱";
                toolTip.Content = "绘制主拱";
                toolTip.Command = "GA";
                toolTip.ExpandedContent = "用GA命令，生成桥形图";
                string imgToolTipFileName = Config.CurPath + "Images\\bridge.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                gaBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                gaBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return gaBtn;
            }
        }

        //GP按钮
        private static RibbonButtonEX gpBtn;
        public static RibbonButtonEX GPBtn
        {
            get
            {
                gpBtn = new RibbonButtonEX("桥位图", RibbonItemSize.Large, Orientation.Vertical, "GP");
                gpBtn.SetImg(Config.CurPath + "Images\\平面.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "桥位图";
                toolTip.Content = "生成桥位图";
                toolTip.Command = "GP";
                toolTip.ExpandedContent = "用GP命令，生成桥位图";
                string imgToolTipFileName = Config.CurPath + "Images\\平面.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                gpBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                gpBtn.ImgHoverFileName = Config.CurPath + "Images\\cursor-default-click-outline.PNG";
                return gpBtn;
            }
        }

        //立柱绘制
        private static RibbonButtonEX colBtn;
        public static RibbonButtonEX ColBtn
        {
            get
            {
                colBtn = new RibbonButtonEX("绘制立柱", RibbonItemSize.Large, Orientation.Vertical, "ColumnDraw");
                colBtn.SetImg(Config.CurPath + "Images\\ladder.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "绘制立柱";
                toolTip.Content = "绘制立柱";
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

        #endregion

    }
}
