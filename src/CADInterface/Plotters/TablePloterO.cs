using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADInterface.Plotters
{

    public class TablePloterO
    {
        /// <summary>
        /// 承台钢筋明细表格
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startPoint"></param>
        /// <param name="headName"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="tbHeight"></param>
        /// <param name="tbWidth"></param>
        /// <param name="txtStyle"></param>
        /// <param name="scale"></param>
        ///  <param name="isHaveNetRebar">是否有钢筋网</param>
        public static void DrawCapRebarTable(Database db, ref Extents2d ext, Point3d startPoint, System.Data.DataTable dt, List<string> headName,
            int rowNum, int colNum, ref double tbHeight,ref double tbWidth, string txtStyle, double scale, bool isHaveNetRebar = false,int netNum=2)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                ObjectId id;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                id = st[txtStyle];

                // 创建table 
                Table tb = new Table();
                tb.SetSize(rowNum, colNum);       // 设置几行几列
                tb.Position = startPoint;
                tb.GenerateLayout();
                //tb.SetColumnWidth(50);
                //tb.Width = tbWidth;
                //tb.Height = tbHeight;

                // 设置表头
                for (int i = 0; i < colNum; ++i)
                {
                    tb.Cells[0, i].TextString = headName[i]; //获取i行j列数据
                    tb.Cells[0, i].Contents[0].TextHeight = 2.5 * scale;
                    tb.Cells[0, i].Alignment = CellAlignment.MiddleCenter;
                    tb.Cells[0, i].Contents[0].Rotation = 0;
                    tb.Cells[0, i].Borders.Horizontal.Margin = 2 * scale;
                    tb.Cells[0, i].Contents[0].TextStyleId = id;
                }
                tb.Rows[0].Height = 10 * scale;
               
                //Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
                //for(int i = 0; i < dt.Rows.Count; ++i)
                //{
                //    string strNo = dt.Rows[i]["diameter"].ToString();
                //    if(dic.Count == 0)
                //    {
                //        List<int> list = new List<int>();
                //        list.Add(i);
                //        dic.Add(strNo, list);
                //    }
                //    else
                //    {
                //        if(!dic.ContainsKey(strNo))
                //        {
                //            List<int> list = new List<int>();
                //            list.Add(i);
                //            dic.Add(strNo, list);
                //        }
                //        else
                //        {
                //            List<int> list = new List<int>();
                //            dic.TryGetValue(strNo, out list);
                //            list.Add(i);
                //            dic[strNo] = list;
                //        }
                //    }
                //}

                //int index = 0;
                //foreach(KeyValuePair<string,List<int>> pr in dic)
                //{
                //    // 设置表数据
                //    for (int i = 0; i < pr.Value.Count; ++i)
                //    {
                //        string strNo = dt.Rows[pr.Value[i]]["no"].ToString();                                     // 钢筋编号
                //        string strDiameter = dt.Rows[pr.Value[i]]["diameter"].ToString();                         // 直径
                //        string strProbably_length = dt.Rows[pr.Value[i]]["probably_length"].ToString();           // 大样
                //        string strLength = dt.Rows[pr.Value[i]]["length"].ToString();                             // 单根长
                //        string strNum = dt.Rows[pr.Value[i]]["number"].ToString();                                // 数量
                //        string strTotal_length = dt.Rows[pr.Value[i]]["total_length"].ToString();                 // 总长
                //        string strSingle_weight = dt.Rows[pr.Value[i]]["single_weight"].ToString();               // 单重
                //        string strTotal_weight = dt.Rows[pr.Value[i]]["total_weight"].ToString();                 // 总重
                //        if (headName.Contains("大样"))
                //        {
                //            tb.Cells[index + i + 1, 0].TextString = strNo; //获取i行j列数据
                //            tb.Cells[index + i + 1, 1].TextString = strDiameter; //获取i行j列数据
                //            tb.Cells[index + i + 1, 2].TextString = strProbably_length; //获取i行j列数据
                //            tb.Cells[index + i + 1, 3].TextString = strLength; //获取i行j列数据
                //            tb.Cells[index + i + 1, 4].TextString = strNum; //获取i行j列数据
                //            tb.Cells[index + i + 1, 5].TextString = strTotal_length; //获取i行j列数据
                //            tb.Cells[index + i + 1, 6].TextString = strSingle_weight; //获取i行j列数据
                //            tb.Cells[index + i + 1, 7].TextString = strTotal_weight; //获取i行j列数据
                //        }
                //        else
                //        {
                //            tb.Cells[index + i + 1, 0].TextString = strNo; //获取i行j列数据
                //            tb.Cells[index + i + 1, 1].TextString = strDiameter; //获取i行j列数据
                //            tb.Cells[index + i + 1, 2].TextString = strLength; //获取i行j列数据
                //            tb.Cells[index + i + 1, 3].TextString = strNum; //获取i行j列数据
                //            tb.Cells[index + i + 1, 4].TextString = strTotal_length; //获取i行j列数据
                //            tb.Cells[index + i + 1, 5].TextString = strSingle_weight; //获取i行j列数据
                //            tb.Cells[index + i + 1, 6].TextString = strTotal_weight; //获取i行j列数据
                //        }
                //    }
                //    index += pr.Value.Count;
                //}           
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // 设置表数据    
                    string strNo = "N"+ dt.Rows[i]["no"].ToString().Replace("N","");                                     // 钢筋编号
                    string strDiameter = "Ф"+ dt.Rows[i]["diameter"].ToString();                         // 直径
                    string strProbably_length = dt.Rows[i]["probably_length"].ToString();           // 大样
                    string strLength = (double.Parse(dt.Rows[i]["length"].ToString())/10).ToString();                             // 单根长
                    string strNum = dt.Rows[i]["number"].ToString();                                // 数量
                    string strTotal_length = dt.Rows[i]["total_length"].ToString();                 // 总长
                    string strSingle_weight = dt.Rows[i]["single_weight"].ToString();               // 单重
                    string strTotal_weight = dt.Rows[i]["total_weight"].ToString();                 // 总重
                    if (headName.Contains("大样"))
                    {
                        tb.Cells[i + 1, 0].TextString = strNo; //获取i行j列数据
                        tb.Cells[i + 1, 1].TextString = strDiameter; //获取i行j列数据
                        tb.Cells[i + 1, 2].TextString = strProbably_length; //获取i行j列数据
                        tb.Cells[i + 1, 3].TextString = strLength; //获取i行j列数据
                        tb.Cells[i + 1, 4].TextString = strNum; //获取i行j列数据
                        tb.Cells[i + 1, 5].TextString = strTotal_length; //获取i行j列数据
                        tb.Cells[i + 1, 6].TextString = strSingle_weight; //获取i行j列数据
                        tb.Cells[i + 1, 7].TextString = strTotal_weight; //获取i行j列数据
                    }
                    else
                    {
                        tb.Cells[i + 1, 0].TextString = strNo; //获取i行j列数据
                        tb.Cells[i + 1, 1].TextString = strDiameter; //获取i行j列数据
                        tb.Cells[i + 1, 2].TextString = strLength; //获取i行j列数据
                        tb.Cells[i + 1, 3].TextString = strNum; //获取i行j列数据
                        tb.Cells[i + 1, 4].TextString = strTotal_length; //获取i行j列数据
                        tb.Cells[i + 1, 5].TextString = strSingle_weight; //获取i行j列数据
                        tb.Cells[i + 1, 6].TextString = strTotal_weight; //获取i行j列数据
                    }
                }

                for (int i = 1; i < rowNum; ++i)
                {
                    tb.Rows[i].Height = 6 * scale;
                    int num = colNum;
                    if (headName.Contains("小计(kg)"))
                        num = colNum - 1;
                    for (int j = 0; j < num; ++j)
                    {
                        tb.Cells[i, j].Contents[0].TextHeight = 2.5 * scale;
                        tb.Cells[i, j].Alignment = CellAlignment.MiddleCenter;
                        tb.Cells[i, j].Contents[0].Rotation = 0;
                        tb.Cells[i, j].Borders.Horizontal.Margin = 2 * scale;
                        tb.Cells[i, j].Contents[0].TextStyleId = id;
                    }
                }

                // 设置列宽,行高
                for (int i = 0; i < colNum; ++i)
                {
                    tb.Columns[i].Width = 20 * scale;
                }

                #region 绘制大样
                if (headName.Contains("大样"))
                {
                    Point2d pt = tb.Position.Convert2D(20 * scale * 2 + 1 * scale, -5 * scale - tb.Rows[0].Height);
                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        Polyline line = new Polyline() { Closed = false, Layer = "粗线", ColorIndex = 1 };//定义不封闭的Polyline 平面虚线
                        if (isHaveNetRebar && i > dt.Rows.Count - netNum-1)
                        {
                            line.AddVertexAt(0, pt.Convert2D(0, -i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                            line.AddVertexAt(1, pt.Convert2D(18 * scale, -i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                        }
                        else
                        {
                            line.AddVertexAt(0, pt.Convert2D(0, 2 * scale - i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                            line.AddVertexAt(1, pt.Convert2D(0, -i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                            line.AddVertexAt(2, pt.Convert2D(18 * scale, -i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                            line.AddVertexAt(3, pt.Convert2D(18 * scale, 2 * scale - i * 6 * scale), 0, 0.25 * scale, 0.25 * scale);
                        }
                        modelSpace.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                    }
                }
                #endregion

                #region 列单元格合并
                if (headName.Contains("小计(kg)"))
                {
                    // 数据排序
                    Dictionary<int, int> dic = new Dictionary<int, int>();
                    int rowIndex = 0;
                    int num = 0;
                    string stDia = dt.Rows[0]["diameter"].ToString();
                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        if (dt.Rows[i]["diameter"].ToString() == stDia)
                        {
                            num++;
                        }
                        else
                        {
                            dic.Add(rowIndex, num);
                            num = 1;
                            rowIndex = i;
                            stDia = dt.Rows[i]["diameter"].ToString();

                        }
                        if (i == dt.Rows.Count - 1)
                            dic.Add(rowIndex, num);
                    }

                    // 列单元格合并
                    int topRow = 1;

                    List<CellRange> cellList = new List<CellRange>();

                    foreach (KeyValuePair<int, int> pr in dic)
                    {
                        double weight = 0.0;
                        for (int i = 0; i < pr.Value; ++i)
                        {
                            weight += double.Parse(tb.Cells[i + topRow, tb.Columns.Count - 2].Value.ToString());
                        }

                        tb.Cells[topRow, tb.Columns.Count - 1].TextString = weight.ToString(); //获取i行j列数据
                        tb.Cells[topRow, tb.Columns.Count - 1].Contents[0].TextHeight = 2.5 * scale;
                        tb.Cells[topRow, tb.Columns.Count - 1].Alignment = CellAlignment.MiddleCenter;
                        tb.Cells[topRow, tb.Columns.Count - 1].Contents[0].Rotation = 0;
                        tb.Cells[topRow, tb.Columns.Count - 1].Borders.Horizontal.Margin = 2 * scale;
                        tb.Cells[topRow, tb.Columns.Count - 1].Contents[0].TextStyleId = id;

                        CellRange range = CellRange.Create(tb, topRow, tb.Columns.Count - 1, topRow + pr.Value - 1, tb.Columns.Count - 1);
                        cellList.Add(range);
                        topRow += pr.Value;
                    }
                    foreach (CellRange range in cellList)
                    {
                        tb.MergeCells(range);
                    }
                }
                #endregion


                tb.Layer = "细线";
                modelSpace.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);
                ext = ext.Add(new Extents2d(tb.Bounds.Value.MinPoint.Convert2D(), tb.Bounds.Value.MaxPoint.Convert2D()));
                List<Point2d> pList = new List<Point2d>();
                Point2d pt1 = startPoint.Convert2D();
                pList.Add(pt1);
                Point2d pt2 = pt1.Convert2D(0, -tb.Height);
                pList.Add(pt2);
                Point2d pt3 = pt1.Convert2D(tb.Width, -tb.Height);
                pList.Add(pt3);
                Point2d pt4 = pt1.Convert2D(tb.Width, 0);
                pList.Add(pt4);
                PolylinePloter.AddPolylineByList(db, ref ext, pList, "粗线");
                tr.Commit();

                tbHeight = tb.Height;
                tbWidth = tb.Width;
                //return tb.Height;
            }
        }

        public static DBObjectCollection CreatTable(Database db,Point2d midtop,ref System.Data.DataTable theTable)
        {
            DBObjectCollection ret = new DBObjectCollection();
            Table tb = new Table();

            double w = 12 * theTable.Columns.Count;
            double h = 6.5 * (theTable.Rows.Count+1);

            tb.SetSize(theTable.Rows.Count+1,theTable.Columns.Count);       // 设置几行几列
            tb.Position = midtop.Convert3D(-w*0.5);


            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                for (int i = 0; i < theTable.Columns.Count; ++i)
                {
                    if (theTable.Columns[i].Caption != "")
                    {
                        tb.Cells[0, i].TextString = theTable.Columns[i].Caption;
                    }
                    else
                    {
                        tb.Cells[0, i].TextString = theTable.Columns[i].ColumnName;
                    }
                    //获取i行j列数据
                    tb.Cells[0, i].TextHeight = 2.5;
                    tb.Cells[0, i].Alignment = CellAlignment.MiddleCenter;
                    tb.Cells[0, i].Borders.Horizontal.Margin = 1;
                    tb.Cells[0, i].TextStyleId = st["仿宋"];
                }

                for (int i = 0; i < theTable.Rows.Count; i++)
                {
                    for (int j = 0; j < theTable.Columns.Count; j++)
                    {

                        tb.Cells[i + 1, j].TextString = theTable.Rows[i][j].ToString();
                        tb.Cells[i + 1, j].TextHeight = 2.5;
                        tb.Cells[i + 1, j].Alignment = CellAlignment.MiddleCenter;
                        tb.Cells[i + 1, j].Borders.Horizontal.Margin = 1;
                        tb.Cells[i + 1, j].TextStyleId = st["仿宋"];
                    }

                }


                tb.SetRowHeight(6.5);
                tb.SetColumnWidth(12);
                tb.GenerateLayout();
                tb.Layer = "标注";
                ret.Add(tb);

                DBText title = new DBText()
                {
                    TextString = theTable.TableName,
                    Height = 3.0,
                    TextStyleId = st["仿宋"],
                    Position = midtop.Convert3D(0, 3),
                    HorizontalMode = TextHorizontalMode.TextMid,
                    VerticalMode = TextVerticalMode.TextBase,
                    AlignmentPoint = midtop.Convert3D(0, 3),
                    WidthFactor = ((TextStyleTableRecord)tr.GetObject(st["仿宋"], OpenMode.ForRead)).XScale,
                };
                ret.Add(title);
                ret.Add(PolylinePloter.CreatPloy4(midtop, w * 0.5, w * 0.5, h, "粗线"));

            }

            return ret;
        }

        /// <summary>
        /// 材料数量明细表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ext"></param>
        /// <param name="startPoint"></param>
        /// <param name="dt"></param>
        /// <param name="headName"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="tbHeight"></param>
        /// <param name="tbWidth"></param>
        /// <param name="txtStyle"></param>
        /// <param name="scale"></param>
        public static void DrawCapMaterialsTable(Database db, ref Extents2d ext, Point3d startPoint, System.Data.DataTable dt, List<string> headName,
            int rowNum, int colNum, ref double tbHeight,ref double tbWidth, string txtStyle, double scale)
        {
            double width = 0;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                ObjectId id;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                id = st[txtStyle];

                // 创建table 
                Table tb = new Table();
                tb.SetSize(rowNum, colNum);       // 设置几行几列
                tb.Position = startPoint;
                tb.GenerateLayout();

                // 设置表头
                for (int i = 0; i < colNum; ++i)
                {
                    tb.Cells[0, i].TextString = headName[i]; //获取i行j列数据
                    tb.Cells[0, i].Contents[0].TextHeight = 3 * scale;
                    tb.Cells[0, i].Alignment = CellAlignment.MiddleCenter;
                    tb.Cells[0, i].Contents[0].Rotation = 0;
                    tb.Cells[0, i].Borders.Horizontal.Margin = 2 * scale;
                    tb.Cells[0, i].Contents[0].TextStyleId = id;
                }
                tb.Rows[0].Height = 8 * scale;
                #region 数据排序
                // 数据排序
                Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    string strNo = dt.Rows[i]["name"].ToString();
                    if (dic.Count == 0)
                    {
                        List<int> list = new List<int>();
                        list.Add(i);
                        dic.Add(strNo, list);
                    }
                    else
                    {
                        if (!dic.ContainsKey(strNo))
                        {
                            List<int> list = new List<int>();
                            list.Add(i);
                            dic.Add(strNo, list);
                        }
                        else
                        {
                            List<int> list = new List<int>();
                            dic.TryGetValue(strNo, out list);
                            list.Add(i);
                            dic[strNo] = list;
                        }
                    }
                }
                #endregion

                #region 表格添加数据
                int index = 0;
                foreach (KeyValuePair<string, List<int>> pr in dic)
                {
                    // 设置表数据
                    for (int i = 0; i < pr.Value.Count; ++i)
                    {
                        string strNo = dt.Rows[pr.Value[i]]["name"].ToString();                                     // 钢筋编号
                        string strDiameter = dt.Rows[pr.Value[i]]["standard"].ToString();                         // 直径
                        string strProbably_length = dt.Rows[pr.Value[i]]["unit"].ToString();           // 大样
                        string strLength = dt.Rows[pr.Value[i]]["total"].ToString();                             // 单根长
                        string strNum = dt.Rows[pr.Value[i]]["summary"].ToString();                                // 数量

                        tb.Cells[index + i + 1, 0].TextString = strNo; //获取i行j列数据
                        tb.Cells[index + i + 1, 1].TextString = strDiameter; //获取i行j列数据
                        tb.Cells[index + i + 1, 2].TextString = strProbably_length; //获取i行j列数据
                        tb.Cells[index + i + 1, 3].TextString = strLength; //获取i行j列数据
                        tb.Cells[index + i + 1, 4].TextString = strNum; //获取i行j列数据
                    }
                    index += pr.Value.Count;
                }

                for (int i = 1; i < rowNum; ++i)
                {
                    tb.Rows[i].Height = 6;
                    for (int j = 0; j < colNum - 1; ++j)
                    {
                        if (tb.Cells[i, j].Contents != null && tb.Cells[i, j].Contents.Count > 0)
                        {
                            tb.Cells[i, j].Contents[0].TextHeight = 2.5 * scale;
                            tb.Cells[i, j].Contents[0].Rotation = 0;
                            tb.Cells[i, j].Contents[0].TextStyleId = id;
                        }
                        tb.Cells[i, j].Alignment = CellAlignment.MiddleCenter;
                        tb.Cells[i, j].Borders.Horizontal.Margin = 2 * scale;
                    }
                }
                #endregion

                // 设置列宽,行高
                for (int i = 0; i < colNum; ++i)
                {
                    if (i == 1 || i == 2)
                    {
                        tb.Columns[i].Width = 15 * scale;
                    }
                    else
                    {
                        tb.Columns[i].Width = 25 * scale;
                    }
                }

                for (int i = 1; i < rowNum; ++i)
                {
                    tb.Rows[i].Height = 6 * scale;
                }

                #region 列单元格合并
                // 列单元格合并
                int topRow = 1;
                int bottomRow = 1;
                int leftCol = 1;
                int rightCol = 1;
                string firstTxt = "";
                int indexRow = 1;

                List<CellRange> cellList = new List<CellRange>();

                foreach (KeyValuePair<string, List<int>> pr in dic)
                {
                    double weight = 0.0;
                    for (int i = 0; i < pr.Value.Count; ++i)
                    {
                        double v = 0; 
                        double.TryParse(tb.Cells[i + topRow, 3].Value.ToString(),out v);
                        weight +=v ;
                    }
                    tb.Cells[topRow, 4].TextString = weight.ToString(); //获取i行j列数据
                    tb.Cells[topRow, 4].Alignment = CellAlignment.MiddleCenter;
                    tb.Cells[topRow, 4].Borders.Horizontal.Margin = 2 * scale;
                    if (tb.Cells[topRow, 4].Contents != null && tb.Cells[topRow, 4].Contents.Count > 0)
                    {
                        tb.Cells[topRow, 4].Contents[0].Rotation = 0;
                        tb.Cells[topRow, 4].Contents[0].TextHeight = 2.5 * scale;
                        tb.Cells[topRow, 4].Contents[0].TextStyleId = id;
                    }

                    // 总重合计项
                    CellRange range = CellRange.Create(tb, topRow, 4, topRow + pr.Value.Count - 1, 4);
                    cellList.Add(range);

                    // 名称
                    CellRange range2 = CellRange.Create(tb, topRow, 0, topRow + pr.Value.Count - 1, 0);
                    cellList.Add(range2);

                    topRow += pr.Value.Count;
                }
                if (dt.Rows.Count > 0)
                {
                    // 合并单位
                    topRow = 1;
                    bottomRow = 1;
                    string unit = tb.Cells[1, 2].Value.ToString();
                    for (int i = 2; i < rowNum; ++i)
                    {
                        string str = tb.Cells[i, 2].Value.ToString();
                        if (str == unit)
                        {
                            bottomRow = i;
                            if (i == rowNum - 1)
                            {
                                CellRange range = CellRange.Create(tb, topRow, 2, bottomRow, 2);
                                cellList.Add(range);
                            }
                        }
                        else
                        {
                            CellRange range = CellRange.Create(tb, topRow, 2, bottomRow, 2);
                            cellList.Add(range);

                            unit = str;
                            topRow = i;
                            bottomRow = i;
                        }
                    }

                    // 合并表头
                    leftCol = 0;
                    rightCol = 0;
                    string title = tb.Cells[0, 0].Value.ToString();
                    for (int i = 1; i < colNum; ++i)
                    {
                        string str = tb.Cells[0, i].Value.ToString();
                        if (str == title)
                        {
                            rightCol = i;
                            if (i == colNum - 1)
                            {
                                CellRange range = CellRange.Create(tb, 0, leftCol, 0, rightCol);
                                cellList.Add(range);
                            }
                        }
                        else
                        {
                            CellRange range = CellRange.Create(tb, 0, leftCol, 0, rightCol);
                            cellList.Add(range);

                            title = str;
                            leftCol = i;
                            rightCol = i;
                        }
                    }
                    foreach (CellRange range in cellList)
                    {
                        tb.MergeCells(range);
                    }
                }
                #endregion
                tb.Layer = "细线";
                modelSpace.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);
                ext = ext.Add(new Extents2d(tb.Bounds.Value.MinPoint.Convert2D(), tb.Bounds.Value.MaxPoint.Convert2D()));

                List<Point2d> pList = new List<Point2d>();
                Point2d pt1 = startPoint.Convert2D();
                pList.Add(pt1);
                Point2d pt2 = pt1.Convert2D(0, -tb.Height);
                pList.Add(pt2);
                Point2d pt3 = pt1.Convert2D(tb.Width, -tb.Height);
                pList.Add(pt3);
                Point2d pt4 = pt1.Convert2D(tb.Width, 0);
                pList.Add(pt4);
                PolylinePloter.AddPolylineByList(db,ref ext, pList, "粗线");
                tr.Commit();

                tbHeight = tb.Height;
                tbWidth = tb.Width;
            }
        }

        public static void DraTable(Database db, ref Extents2d ext, Point3d startPoint,  List<string> headName, 
            int colNum, ref double tbHeight, ref double tbWidth, string txtStyle, double scale,Dictionary<int, List<string>> tbValue)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                ObjectId id;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                id = st[txtStyle];
                int rowNum = 7;
                // 创建table 
                Table tb = new Table();
                tb.SetSize(rowNum, colNum);       // 设置几行几列
                tb.Position = startPoint;
                tb.GenerateLayout();

                for (int i = 0; i < colNum; ++i)
                {
                    tb.Cells[0, i].TextString = headName[i]; //获取i行j列数据
                    tb.Cells[0, i].Contents[0].TextHeight = 2.5 * scale;
                    if (i == 0)
                    {
                        tb.Cells[0, i].Alignment = CellAlignment.MiddleLeft;
                    }
                    else
                    {
                        tb.Cells[0, i].Alignment = CellAlignment.MiddleCenter;
                    }
                    tb.Cells[0, i].Contents[0].Rotation = 0;
                    tb.Cells[0, i].Borders.Horizontal.Margin = 1 * scale;
                    tb.Cells[0, i].Contents[0].TextStyleId = id;
                }
                for (int m = 1; m < rowNum; m++)
                {
                    List<string> valueList = new List<string>();
                    try
                    {
                        valueList = tbValue[m];
                        if (valueList.Count > 0)
                        {
                            for (int i = 0; i < colNum; ++i)
                            {
                                tb.Cells[m, i].TextString = valueList[i]; //获取i行j列数据
                                tb.Cells[m, i].Contents[0].TextHeight = 2.5 * scale;
                                if (i == 0)
                                {
                                    tb.Cells[m, i].Alignment = CellAlignment.MiddleLeft;
                                }
                                else
                                {
                                    tb.Cells[m, i].Alignment = CellAlignment.MiddleCenter;
                                }
                                tb.Cells[m, i].Contents[0].Rotation = 0;
                                tb.Cells[m, i].Borders.Horizontal.Margin = 1 * scale;
                                tb.Cells[m, i].Contents[0].TextStyleId = id;
                            }
                        }
                    }catch { }
                }
                #region 表格添加数据

                #endregion

                // 设置列宽,行高
                for (int i = 0; i < colNum; ++i)
                {
                    if (i == 0)
                        tb.Columns[i].Width = 10*2.5 * scale;
                    else
                        tb.Columns[i].Width = 5* 2.5  * scale;
                }

                for (int i = 0; i < rowNum; ++i)
                {
                    tb.Rows[i].Height = 8 * scale;
                }

                tb.Layer = "细线";
                tb.ColorIndex = 4;
                modelSpace.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);
                ext = ext.Add(new Extents2d(tb.Bounds.Value.MinPoint.Convert2D(), tb.Bounds.Value.MaxPoint.Convert2D()));

                List<Point2d> pList = new List<Point2d>();
                Point2d pt1 = startPoint.Convert2D();
                pList.Add(pt1);
                Point2d pt2 = pt1.Convert2D(0, -tb.Height);
                pList.Add(pt2);
                Point2d pt3 = pt1.Convert2D(tb.Width, -tb.Height);
                pList.Add(pt3);
                Point2d pt4 = pt1.Convert2D(tb.Width, 0);
                pList.Add(pt4);
                PolylinePloter.AddPolylineByList(db, ref ext, pList, "粗线");
                tr.Commit();

                tbHeight = tb.Height;
                tbWidth = tb.Width;
            }
        }


        public static Table DrawCommonTable(Database db, ref Extents2d ext, Point3d startPoint, 
            Dictionary<string, List<string>> data, string txtStyle, int scale)
        {
            if (data == null || data.Count == 0)
            {
                return null;
            }
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                ObjectId id;
                TextStyleTable st = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                id = st[txtStyle];

                Table tb = new Table();

                // 计算行列
                List<string> list = data.Values.ElementAt(0);
                int rowNum = list.Count + 1;
                int colNum = data.Count;
                tb.SetSize(rowNum, colNum);       // 设置几行几列
                tb.Position = startPoint;
                tb.GenerateLayout();

                List<string> keyList = new List<string>();
                foreach (KeyValuePair<string, List<string>> kvp in data)
                {
                    keyList.Add(kvp.Key);
                }

                // 设置表头
                for (int i = 0; i < colNum; ++i)
                {
                    tb.Cells[0, i].TextString = keyList[i]; //获取i行j列数据
                    tb.Cells[0, i].Contents[0].TextHeight = 2.5 * scale;
                    tb.Cells[0, i].Alignment = CellAlignment.MiddleCenter;
                    tb.Cells[0, i].Contents[0].Rotation = 0;
                    tb.Cells[0, i].Borders.Horizontal.Margin = 2 * scale;
                    tb.Cells[0, i].Contents[0].TextStyleId = id;
                }
                tb.Rows[0].Height = 10 * scale;


                // 设置内容
                for(int i = 0; i < colNum; ++i)
                {
                    for (int j = 1; j < rowNum; ++j)
                    {
                        List<string> contxt = data.Values.ElementAt(i);
                        tb.Cells[j, i].TextString = contxt[j-1]; //获取i行j列数据

                        tb.Cells[j, i].Contents[0].TextHeight = 2.5 * scale;
                        tb.Cells[j, i].Alignment = CellAlignment.MiddleCenter;
                        tb.Cells[j, i].Contents[0].Rotation = 0;
                        tb.Cells[j, i].Borders.Horizontal.Margin = 2 * scale;
                        tb.Cells[j, i].Contents[0].TextStyleId = id;
                    }
                }


                // 设置列宽,行高
                for (int i = 0; i < colNum; ++i)
                {
                    if (i == 0 || i == 2)
                    {
                        tb.Columns[i].Width = 20 * scale;
                    }
                    else
                    {
                        tb.Columns[i].Width = 30 * scale;
                    }
                }

                for (int i = 1; i < rowNum; ++i)
                {
                    tb.Rows[i].Height = 6 * scale;
                }

                tb.Layer = "细线";
                tb.ColorIndex = 4;
                modelSpace.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);
                ext = ext.Add(new Extents2d(tb.Bounds.Value.MinPoint.Convert2D(), tb.Bounds.Value.MaxPoint.Convert2D()));

                List<Point2d> pList = new List<Point2d>();
                Point2d pt1 = startPoint.Convert2D();
                pList.Add(pt1);
                Point2d pt2 = pt1.Convert2D(0, -tb.Height);
                pList.Add(pt2);
                Point2d pt3 = pt1.Convert2D(tb.Width, -tb.Height);
                pList.Add(pt3);
                Point2d pt4 = pt1.Convert2D(tb.Width, 0);
                pList.Add(pt4);
                PolylinePloter.AddPolylineByList(db, ref ext, pList, "粗线");
                tr.Commit();

                return tb;
            }
        }
    }
}
