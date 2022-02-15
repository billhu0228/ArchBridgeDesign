using Model;
using System;
using System.Data;
using System.IO;

namespace BOQInterface
{
    public class BOQTable : DataTable
    {
        public BOQTable()
        {
            TableName = "主拱BOQ";
            DataColumn column;

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int16");
            column.ColumnName = "ID";
            column.Caption = "编号";
            column.Unique = false;
            Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Material";
            column.Caption = "材料";
            Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "Location";
            column.Caption = "位置";
            Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int16");
            column.ColumnName = "Number";
            column.Caption = "数";
            Columns.Add(column);   
            
            column = new DataColumn();
            column.DataType = Type.GetType("System.Double");
            column.ColumnName = "Quantity";
            column.Caption = "量";
            Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Double");
            column.ColumnName = "Length";
            column.Caption = "长";
            Columns.Add(column);
        }

        internal void AddItem(eMemberType loc, eMaterial mat, int num, double quantity,double length)
        {
            DataRow row;
            row = NewRow();
            row["ID"] =Rows.Count;
            row["Material"] = EnumHelper.GetDescription(typeof(eMaterial), mat);
            row["Location"] = EnumHelper.GetDescription(typeof(eMemberType), loc);
            row["Number"] = num;
            row["Quantity"] = quantity;
            row["Length"] = length;

            Rows.Add(row);
        }

        public void SaveCSV( string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < Columns.Count; i++)
            {
                data +=Columns[i].ColumnName.ToString();
                if (i < Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < Columns.Count; j++)
                {
                    data += Rows[i][j].ToString();
                    if (j < Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
        }


    }
}
 