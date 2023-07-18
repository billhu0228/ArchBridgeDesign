using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.Windows;
using CADInterface.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[assembly: ExtensionApplication(typeof(CADInterface.Main))]
namespace CADInterface
{
    public class Main : IExtensionApplication
    {
        public void Initialize()
        {


            //AddRibbon();//添加ribbon菜单的函数  
            AddRibbon();//添加ribbon菜单的函数 
            AddMenu();
            //SRBInterface.InitSetting.IniConfig();
            //ComponentManager.ItemInitialized += this.ComponentManager_ItemInitialized;   //利用委托添加菜单
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }


        public void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {

            if (ComponentManager.Ribbon != null)
            {
                AddRibbon();//添加ribbon菜单的函数 
                AddMenu();
            }
        }


        public void AddRibbon()
        {
            RibbonControl ribbonCtrl = Autodesk.AutoCAD.Ribbon.RibbonServices.RibbonPaletteSet.RibbonControl;
            RibbonTab tab = ribbonCtrl.AddTab("标准桥梁设计", "Acad.RibbonId1", true); //给Ribbon界面添加一个选项卡
            Config.CurPath = Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\"; //获取程序集的加载路径

            RibbonPanelSource panelDataSource0 = tab.AddPanel("拱桥设计"); //给选项卡添加面板   


            panelDataSource0.Items.Add(RibbonButtons.SYSSetting); //添加查询出图桥梁信息命令按钮
            panelDataSource0.Items.Add(RibbonButtons.GenModelBT); //添加查询出图桥梁信息命令按钮
            panelDataSource0.Items.Add(RibbonButtons.ArchBtn); //添加配置绘制标准按钮 
            panelDataSource0.Items.Add(RibbonButtons.ColBtn); //添加配置绘制标准按钮 
            panelDataSource0.Items.Add(RibbonButtons.GABtn); //添加配置绘制标准按钮 
            panelDataSource0.Items.Add(RibbonButtons.SegBtn); //添加配置绘制标准按钮 
            panelDataSource0.Items.Add(RibbonButtons.ListBtn); //添加配置绘制标准按钮 


            //SysInfo.isCreated = true;
            //SRBPublicFunc.Extensions.curPaht = CurPaht.curPaht;
            //string sysConfigPath = CurPaht.curPaht + "\\Config\\BridgeSetting.config";
            //SRBPublicFunc.Extensions.curTKPath = CurPaht.curPaht + "DWG\\" + ConfigUtils.GetKey(sysConfigPath, "TK").Replace(CurPaht.curPaht, "").Replace("DWG\\", "");
            //if (!File.Exists(SRBPublicFunc.Extensions.curTKPath))
            //{
            //    SRBPublicFunc.Extensions.curTKPath = CurPaht.curPaht + "DWG\\TK-BR.dwg";
            //}
            //SRBPublicFunc.Extensions.curMapSheet = ConfigUtils.GetKey(sysConfigPath, "MapSheet");
        }
        public static void AddMenu()
        {
            Autodesk.AutoCAD.Interop.AcadPopupMenu popMenu;
            Autodesk.AutoCAD.Interop.AcadApplication acadApp = Application.AcadApplication as Autodesk.AutoCAD.Interop.AcadApplication;

            //为AutoCAD添加一个新的菜单，并设置标题为"智能设计项目系统设置"
            try
            {
                popMenu = acadApp.MenuGroups.Item(0).Menus.Add("智能设计项目系统设置");

                Autodesk.AutoCAD.Interop.AcadPopupMenuItem popItem1 = popMenu.AddMenuItem(0, "选择项目桥梁", "LoadProjectData ");
                Autodesk.AutoCAD.Interop.AcadPopupMenuItem popItem0 = popMenu.AddMenuItem(1, "项目设置", "InitDrawStandard ");
                popMenu.InsertInMenuBar(acadApp.MenuBar.Count + 1);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
