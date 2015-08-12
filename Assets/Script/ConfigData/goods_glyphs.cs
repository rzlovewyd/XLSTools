using UnityEngine;
using System;
using System.IO;

public class goods_glyphs
{
	///<summary> id </summary>
	private int id;
	///<summary> 名称 </summary>
	private string name;
	///<summary> 物品分类 </summary>
	private int goodsType;
	///<summary> 图标id </summary>
	private int imageId;
	///<summary> 描述 </summary>
	private string desc;
	///<summary> 回收获得属性类型 </summary>
	private int recycleAttriType;
	///<summary> 回收获得属性值 </summary>
	private int recycleAttriValue;
	///<summary> 物品品质 </summary>
	private int qualityType;
	///<summary> 等级需求 </summary>
	private int lvLimit;
	///<summary> 获取关卡 </summary>
	private string gainBattleId;
	///<summary> 获取描述 </summary>
	private string gainDesc;
	///<summary> 基础属性类型 </summary>
	private int attriType;
	///<summary> 基础属性值 </summary>
	private int attriValue;
	///<summary> 孔id(0-5) </summary>
	private int holeId;
	///<summary> 套装id </summary>
	private int suitId;
	///<summary> 交易次数 </summary>
	private int tradeNum;
	///<summary> 传承游戏币 </summary>
	private int inheritGameMoney;
	///<summary> 熔炼值 </summary>
	private int meltExp;
	///<summary> 拆卸消耗钻石 </summary>
	private int apartGoldMoney;
	///<summary> 洗练消耗熔炼值 </summary>
	private int baptizeMeltExp;
	///<summary> 附加战斗力 </summary>
	private int addBattleScore;
}


public class goods_glyphsManager
{
	// Id的种子
	private static int idSeed;
	private static goods_glyphs[] m_datas;


	public static void InitDatas(TextAsset _Txt)
	{
		MemoryStream fs = null;
		BinaryReader br = null;
		try
		{
			int rowCount = br.ReadInt16();
			m_datas = new goods_glyphs[rowCount];
			for (int i = 0; i < rowCount; i++)
			{
				m_datas[i] = new goods_glyphs();

				// TODO

			}
			br.Close();
			fs.Close();
		}
		catch(IOException e)
		{
			Debug.LogError("Read goods_glyphs.bytes ERROR:" + e);
		}
	}

	public static goods_glyphs GetData(int id)
	{
		int indexId = id - idSeed;
		if(indexId > 0 && indexId < m_datas.Length)
		{
			return m_datas[indexId];
		}
		Debug.LogError("can't find data where id = " + id);
		return null;
	}
}
