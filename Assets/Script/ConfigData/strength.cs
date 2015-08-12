using UnityEngine;
using System;
using System.IO;

public class strength
{
	///<summary> 强化等级 </summary>
	private int level;
	///<summary> 基础属性提升比率 </summary>
	private int attriRate;
	///<summary> 领主等级限制 </summary>
	private int roleLevel;
	///<summary> 真实成功率 </summary>
	private int successRatio;
	///<summary> 展示成功率 </summary>
	private int showSuccessRatio;
	///<summary> 极限次数 </summary>
	private int limitNum;
	///<summary> 保底次数 </summary>
	private int endNum;
	///<summary> 强化石Id </summary>
	private int stoneId;
	///<summary> 数量 </summary>
	private int stoneNum;
	///<summary> 消耗游戏币 </summary>
	private int consumeGameMoney;
	///<summary> 激活条数 </summary>
	private int activeNum;
}


public class strengthManager
{
	// Id的种子
	private static int idSeed;
	private static strength[] m_datas;


	public static void InitDatas(TextAsset _Txt)
	{
		MemoryStream fs = null;
		BinaryReader br = null;
		try
		{
			int rowCount = br.ReadInt16();
			m_datas = new strength[rowCount];
			for (int i = 0; i < rowCount; i++)
			{
				m_datas[i] = new strength();

				// TODO

			}
			br.Close();
			fs.Close();
		}
		catch(IOException e)
		{
			Debug.LogError("Read strength.bytes ERROR:" + e);
		}
	}

	public static strength GetData(int id)
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
