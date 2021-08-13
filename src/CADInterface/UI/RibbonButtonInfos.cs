using Autodesk.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CADInterface.UI
{
    
    public static class RibbonButtonInfos
    {
        #region 智能设计项目

        //系统设置按钮
        private static RibbonButtonEX sysSetting;
        public static RibbonButtonEX SYSSetting
        {
            get
            {
                sysSetting = new RibbonButtonEX("系统设置", RibbonItemSize.Large, Orientation.Vertical, "init");
                sysSetting.SetImg(Config.CurPath + "Images\\设置.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "系统设置";
                toolTip.Content = "系统设置";
                toolTip.Command = "init";
                toolTip.ExpandedContent = "用init命令，打开设置界面进行配置";
                string imgToolTipFileName = Config.CurPath + "Images\\设置.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                sysSetting.ToolTip = toolTip;
                //鼠标进入时的图片
                sysSetting.ImgHoverFileName = Config.CurPath + "Images\\点击.PNG";
                return sysSetting;
            }
        }
        //GA按钮
        private static RibbonButtonEX gaBtn;
        public static RibbonButtonEX GABtn
        {
            get
            {
                gaBtn = new RibbonButtonEX("桥形图", RibbonItemSize.Large, Orientation.Vertical, "GA");
                gaBtn.SetImg(Config.CurPath + "Images\\桥.PNG");//设置按钮图片
                //添加提示对象
                RibbonToolTip toolTip = new RibbonToolTip();
                toolTip.Title = "桥形图";
                toolTip.Content = "生成桥型图";
                toolTip.Command = "GA";
                toolTip.ExpandedContent = "用GA命令，生成桥形图";
                string imgToolTipFileName = Config.CurPath + "Images\\桥.PNG";
                Uri toolTipUri = new Uri(imgToolTipFileName);
                BitmapImage toolTipBitmapImge = new BitmapImage(toolTipUri);
                toolTip.ExpandedImage = toolTipBitmapImge;
                gaBtn.ToolTip = toolTip;
                //鼠标进入时的图片
                gaBtn.ImgHoverFileName = Config.CurPath + "Images\\点击.PNG";
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
                gpBtn.ImgHoverFileName = Config.CurPath + "Images\\点击.PNG";
                return gpBtn;
            }
        }



        #endregion

    }
}
