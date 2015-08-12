using UnityEngine;
using System;
using System.IO;

public class equip_list
{
	///<summary> 模板ID </summary>
	private int id;
	///<summary> 名称 </summary>
	private string name;
	///<summary> 物品类型 </summary>
	private int goodsType;
	///<summary> 图标ID </summary>
	private int imageId;
	///<summary> 物品描述 </summary>
	private string desc;
	///<summary> 回收获得属性类型 </summary>
	private int recycleAttriType;
	///<summary> 回收获得属性值 </summary>
	private int recycleAttriValue;
	///<summary> 物品品质 </summary>
	private int qualityType;
	///<summary> 使用等级 </summary>
	private int lvLimit;
	///<summary> 获取关卡 </summary>
	private string gainBattleId;
	///<summary> 获取描述 </summary>
	private string gainDesc;
	///<summary> 属性ID1 </summary>
	private int attriId1;
	///<summary> 属性值1 </summary>
	private int attriValue1;
	///<summary> 附魔每级增加属性1 </summary>
	private int enchantAttri1;
	///<summary> 属性ID2 </summary>
	private int attriId2;
	///<summary> 属性值2 </summary>
	private int attriValue2;
	///<summary> 附魔每级增加属性2 </summary>
	private int enchantAttri2;
	///<summary> 属性ID3 </summary>
	private int attriId3;
	///<summary> 属性值3 </summary>
	private int attriValue3;
	///<summary> 附魔每级增加属性3 </summary>
	private int enchantAttri3;
	///<summary> 属性ID4 </summary>
	private int attriId4;
	///<summary> 属性值4 </summary>
	private int attriValue4;
	///<summary> 附魔每级增加属性4 </summary>
	private int enchantAttri4;
	///<summary> 属性ID5 </summary>
	private int attriId5;
	///<summary> 属性值5 </summary>
	private int attriValue5;
	///<summary> 附魔每级增加属性5 </summary>
	private int enchantAttri5;
	///<summary> 被吃增加附魔值 </summary>
	private int enchant;
	///<summary> 是否为合成类装备 </summary>
	private int materail;
}


public class equip_listManager
{
	// Id的种子
	private static int idSeed;
	private static equip_list[] m_datas;


	public static void InitDatas(TextAsset _Txt)
	{
		MemoryStream fs = null;
		BinaryReader br = null;
		try
		{
			int rowCount = br.ReadInt16();
			m_datas = new equip_list[rowCount];
			for (int i = 0; i < rowCount; i++)
			{
				m_datas[i] = new equip_list();

				// TODO

			}
			br.Close();
			fs.Close();
		}
		catch(IOException e)
		{
			Debug.LogError("Read equip_list.bytes ERROR:" + e);
		}
	}

	public static equip_list GetData(int id)
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
