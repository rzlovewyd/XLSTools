using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NPOI.HSSF.UserModel;
using System.Text;
using NPOI.SS.UserModel;
using System.Xml;

public class xlsTools
{
    static FileStream fs;
    static BinaryWriter bw;
	static string ConfigPath = Application.dataPath + Path.DirectorySeparatorChar + "XLS" + Path.DirectorySeparatorChar;
	static string ClassFilePath = Application.dataPath + Path.DirectorySeparatorChar + "Script" + Path.DirectorySeparatorChar + "ConfigData" + Path.DirectorySeparatorChar;
	static string BinaryFilePath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "XLSData" + Path.DirectorySeparatorChar;
	static string XMLConfigPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "XLSConfig" + Path.DirectorySeparatorChar + "XLSConfig.xml";

    [MenuItem("XLSXConfig/BuildAll")]
    static void ExportAll()
    {
		// 读取xml配置表
		XmlDocument xmlDoc = new XmlDocument();
		if(CheckFileExist(XMLConfigPath))
		{
			xmlDoc.Load(XMLConfigPath);
			// 得到跟节点xls
			XmlNode xn = xmlDoc.SelectSingleNode("config");
			// 得到根节点的所有子节点
			XmlNodeList xnl = xn.ChildNodes;
			foreach (XmlNode item in xnl)
			{
				string xlsName = (item as XmlElement).GetAttribute("name");
				string xlsFilePath = ConfigPath + xlsName + ".xls";

				if (!CheckFileExist(xlsFilePath)) continue;

                //打开myxls.xls文件
				FileStream fs_xls = File.OpenRead(xlsFilePath);
				//把xls文件中的数据写入wk中
				HSSFWorkbook wk = new HSSFWorkbook(fs_xls);

				XmlNodeList sheetList = item.ChildNodes;
				foreach (XmlNode sheet in sheetList)
				{
                    Main(sheet.InnerText, wk);
                }

				fs_xls.Close();
			}
			EditorUtility.DisplayDialog("提示", "导出成功！", "确定");
		}
    }
	[MenuItem("XLSXConfig/BuildSelect")]
	static void ExportSelect()
	{
		if(!CheckFileExist(XMLConfigPath)) return;
		// 读取xml配置表
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(XMLConfigPath);
		// 得到跟节点xls
		XmlNode xn = xmlDoc.SelectSingleNode("config");
		// 得到根节点的所有子节点
        XmlNodeList xnl = xn.ChildNodes;
        
        Object[] activeGOs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		foreach (Object o in activeGOs)
		{
			string path = AssetDatabase.GetAssetPath(o);
			if(!path.EndsWith(".xls")) continue;
			if(!path.Contains("/XLS/")) continue;

			//打开myxls.xls文件
			FileStream fs_xls = File.OpenRead(path);
			//把xls文件中的数据写入wk中
			HSSFWorkbook wk = new HSSFWorkbook(fs_xls);
            
            foreach (XmlNode item in xnl)
			{
				string xlsName = (item as XmlElement).GetAttribute("name");
				if(xlsName.Equals(o.name))
				{
					XmlNodeList sheetList = item.ChildNodes;
					foreach (XmlNode sheet in sheetList)
					{
	                    Main(sheet.InnerText, wk);
	                }
	                
	                fs_xls.Close();
				}
            }
        }
		EditorUtility.DisplayDialog("提示", "导出成功！", "确定");
    }
    
    static void Main(string sheetName, HSSFWorkbook wk)
	{
		if (!Directory.Exists(BinaryFilePath))
			Directory.CreateDirectory(BinaryFilePath);
        string bytesFilePath = BinaryFilePath + sheetName +".bytes";
		// 首先判断，文件是否已经存在
		if (File.Exists(bytesFilePath))
		{
			//如果文件已经存在，那么删除掉.
			File.Delete(bytesFilePath);
		}
		
		// FileMode.Create 指定操作系统应创建新文件。如果文件已存在，它将被覆盖。
		fs = new FileStream(bytesFilePath, FileMode.Create, FileAccess.Write);
		bw = new BinaryWriter(fs, Encoding.UTF8);
		
		ISheet BaseSheet = wk.GetSheet(sheetName);
		int _BaseSheetRowCount = BaseSheet.PhysicalNumberOfRows;
		
		int nCount = _BaseSheetRowCount - 2;
		bw.Write(nCount);
		
		// 写入数据以及生成实体类都要基于第一行row0,因为下面的行包含无用辅助列
		IRow row0 = BaseSheet.GetRow(0);
		CreateClass(sheetName, row0, BaseSheet.GetRow(1), BaseSheet.GetRow(2));
		for (int i = 2; i < _BaseSheetRowCount; i++) 
		{
			IRow curRow = BaseSheet.GetRow(i);
			WriteRowData(curRow, row0);
        }
        
        bw.Flush();
        bw.Close();
		fs.Close();
    }
    
    /// <summary>
    /// 将每行(第三行起)数据写入二进制文件.
    /// </summary>
    /// <param name="row">需写入的行数据.</param>
    /// <param name="row0">第一行作为参考,避免写入多余的辅助列.</param>
	static void WriteRowData(IRow row, IRow row0)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < row0.LastCellNum; i++)
		{
			// 是否是多余的辅助列
			if(string.IsNullOrEmpty(row0.Cells[i].StringCellValue))
			{
				continue;
			}

			ICell cell = row.GetCell(i);
			if(cell == null) continue;

			if(cell.CellType == CellType.Numeric)
			{
				sb.Append("; " + cell.NumericCellValue);
				bw.Write((int)cell.NumericCellValue);
			}
			else if(cell.CellType == CellType.String)
			{
				sb.Append("; " + cell.StringCellValue);
				bw.Write(cell.StringCellValue);
			}
			else if(cell.CellType == CellType.Formula)
			{
				if(cell.CachedFormulaResultType == CellType.Numeric)
				{
					sb.Append("; " + cell.NumericCellValue);
					bw.Write((int)cell.NumericCellValue);
				}
				else if(cell.CachedFormulaResultType == CellType.String)
				{
					sb.Append("; " + cell.StringCellValue);
					bw.Write(cell.StringCellValue);
				}
			}
		}

		Debug.Log(sb.ToString().Substring(2));
	}

	/// <summary>
	/// 生成实体类.
	/// </summary>
	/// <param name="className">类名.</param>
	/// <param name="row0">第一行用于生成字段名.</param>
	/// <param name="row1">第二行用于生成注释.</param>
	/// <param name="row2">第三行用于生成字段类型.</param>
	static void CreateClass(string className, IRow row0, IRow row1, IRow row2)
	{
		// 排错
		if (!Directory.Exists(ClassFilePath))
			Directory.CreateDirectory(ClassFilePath);
		if(row0 == null || row1 == null || row2 == null) 
		{
			Debug.LogError("参数有误,无法生成"+ className +"对应的实体类");
			return;
		}
		if(row0.RowNum != 0 || row1.RowNum != 1 || row2.RowNum != 2)
		{
			Debug.LogError("参数有误,无法生成"+ className +"对应的实体类");
			return;
		}

		FileStream fs_class = new FileStream(ClassFilePath + className + ".cs", FileMode.Create);
		StreamWriter sw = new StreamWriter(fs_class,System.Text.Encoding.UTF8);
		sw.WriteLine("using UnityEngine;");
		sw.WriteLine("using System;");
		sw.WriteLine("using System.IO;");
		sw.WriteLine("");
		sw.WriteLine("public class " + className);
		sw.WriteLine("{");

		// 遍历第1,2,3行数据,生成实体类并且找出需要排除的列(通过第一行row0)
		for (int i = 0; i < row0.LastCellNum; i++)
		{
			if(!string.IsNullOrEmpty(row0.GetCell(i).StringCellValue))
			{
				string type = "string";
				if(row2.GetCell(i).CellType == CellType.Numeric)
				{
					type = "int";
				}
				else if(row2.GetCell(i).CellType == CellType.Formula)
				{
					if(row2.GetCell(i).CachedFormulaResultType == CellType.Numeric)
					{
						type = "int";
					}
				}

				sw.WriteLine("\t///<summary> " + row1.GetCell(i).StringCellValue + " </summary>");
				sw.WriteLine("\tprivate " + type + " " + row0.GetCell(i).StringCellValue + ";");
				//Debug.Log(row0.Cells[i].StringCellValue);
			}
		}
		sw.WriteLine("}");
		sw.WriteLine("");
		sw.WriteLine("");

		// DataManager
		sw.WriteLine("public class " + className + "Manager");
		sw.WriteLine("{");
		sw.WriteLine("\t// Id的种子");
		sw.WriteLine("\tprivate static int idSeed;");
		sw.WriteLine("\tprivate static " + className + "[] m_datas;");
		sw.WriteLine("");
		sw.WriteLine("");
		sw.WriteLine("\tpublic static void InitDatas(TextAsset _Txt)");
		sw.WriteLine("\t{");
		sw.WriteLine("\t\tMemoryStream fs = null;");
		sw.WriteLine("\t\tBinaryReader br = null;");
		sw.WriteLine("\t\ttry");
		sw.WriteLine("\t\t{");
		sw.WriteLine("\t\t\tint rowCount = br.ReadInt16();");
		sw.WriteLine("\t\t\tm_datas = new " + className + "[rowCount];");
		sw.WriteLine("\t\t\tfor (int i = 0; i < rowCount; i++)");
		sw.WriteLine("\t\t\t{");
		sw.WriteLine("\t\t\t\tm_datas[i] = new " + className + "();");
		sw.WriteLine("");
		sw.WriteLine("\t\t\t\t// TODO");
		sw.WriteLine("");
		sw.WriteLine("\t\t\t}");
		sw.WriteLine("\t\t\tbr.Close();");
		sw.WriteLine("\t\t\tfs.Close();");
		sw.WriteLine("\t\t}");
		sw.WriteLine("\t\tcatch(IOException e)");
		sw.WriteLine("\t\t{");
		sw.WriteLine("\t\t\tDebug.LogError(\"Read " + className + ".bytes ERROR:\" + e);");
		sw.WriteLine("\t\t}");
		sw.WriteLine("\t}");
		sw.WriteLine("");
		sw.WriteLine("\tpublic static " + className + " GetData(int id)");
		sw.WriteLine("\t{");
		sw.WriteLine("\t\tint indexId = id - idSeed;");
		sw.WriteLine("\t\tif(indexId > 0 && indexId < m_datas.Length)");
		sw.WriteLine("\t\t{");
		sw.WriteLine("\t\t\treturn m_datas[indexId];");
		sw.WriteLine("\t\t}");
		sw.WriteLine("\t\tDebug.LogError(\"can't find data where id = \" + id);");
		sw.WriteLine("\t\treturn null;");
		sw.WriteLine("\t}");

		sw.WriteLine("}");

		sw.Flush();
		sw.Close();
		fs_class.Close(); 
	}

    static bool CheckFileExist(string path)
    {
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError(path + " 不存在！");
            EditorUtility.DisplayDialog("提示", path + " 不存在！", "确定");
            return false;
        }
        return true;
    }
}
