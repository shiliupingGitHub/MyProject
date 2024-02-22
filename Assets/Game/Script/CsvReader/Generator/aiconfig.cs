using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSVHelper;
[System.Serializable]
public class aiconfig {
	public 	int	id;
	public 	int	allfrienddis;
	public 	int	allenemydis;
	public 	int	nearfrienddis;
	public 	int	nearenemydis;
	public 	int	movevalue;
	public 	int	melee;
	public 	int	defensevalue;
	public 	int	defeat;
	public 	int	attackvalue;
	public 	int	noback;
	public 	int	damage;
	public 	int	heal;
	public 	int	plusvalue;
	public 	int	healvalue;
	public 	int	clean;
	public 	int	cleanvalue;
	public 	int	dispel;
	public 	int	dispelvalue;
	public 	int	reborn;
	public 	int	addbuff;
	public 	int	defenserange;
	public 	int	enemyHP_per;
	public 	int	param_back;
	public 	int	param_side;
	public 	int	nearfrienddis_zero;
	public 	int	fury_add;
	public 	int	fury_add_full;
	public 	int	enemyMov;
	public 	int	harm_unit_distent;
	public 	int	harm_unit_atk;
	public 	int	unharm_unit_distent;
	public 	int	unharm_unit_atk;
	public 	int	enemySpeed;
	static Dictionary<int,aiconfig> mDic = null;
 	 public static Dictionary<int,aiconfig>  dic {
			get 
 				 {
					if ( mDic == null) {
							mDic = new  Dictionary<int,aiconfig>();
							 CsvHelper.ReadConfig("aiconfig",OnLoad);
							}
					 return mDic;
				}
	 }
	public static void OnLoad(List<CsvRow> rows)
	{
		for(int i = 3; i < rows.Count; i++)
		{
			CsvRow r = rows[i];
			 if (string.IsNullOrEmpty(r.LineText)) continue;
			aiconfig e = new aiconfig ();
		 if(r.Count >0)
			e.id= CsvHelper.Toint(r[0]);

		 if(r.Count >1)
			e.allfrienddis= CsvHelper.Toint(r[1]);

		 if(r.Count >2)
			e.allenemydis= CsvHelper.Toint(r[2]);

		 if(r.Count >3)
			e.nearfrienddis= CsvHelper.Toint(r[3]);

		 if(r.Count >4)
			e.nearenemydis= CsvHelper.Toint(r[4]);

		 if(r.Count >5)
			e.movevalue= CsvHelper.Toint(r[5]);

		 if(r.Count >6)
			e.melee= CsvHelper.Toint(r[6]);

		 if(r.Count >7)
			e.defensevalue= CsvHelper.Toint(r[7]);

		 if(r.Count >8)
			e.defeat= CsvHelper.Toint(r[8]);

		 if(r.Count >9)
			e.attackvalue= CsvHelper.Toint(r[9]);

		 if(r.Count >10)
			e.noback= CsvHelper.Toint(r[10]);

		 if(r.Count >11)
			e.damage= CsvHelper.Toint(r[11]);

		 if(r.Count >12)
			e.heal= CsvHelper.Toint(r[12]);

		 if(r.Count >13)
			e.plusvalue= CsvHelper.Toint(r[13]);

		 if(r.Count >14)
			e.healvalue= CsvHelper.Toint(r[14]);

		 if(r.Count >15)
			e.clean= CsvHelper.Toint(r[15]);

		 if(r.Count >16)
			e.cleanvalue= CsvHelper.Toint(r[16]);

		 if(r.Count >17)
			e.dispel= CsvHelper.Toint(r[17]);

		 if(r.Count >18)
			e.dispelvalue= CsvHelper.Toint(r[18]);

		 if(r.Count >19)
			e.reborn= CsvHelper.Toint(r[19]);

		 if(r.Count >20)
			e.addbuff= CsvHelper.Toint(r[20]);

		 if(r.Count >21)
			e.defenserange= CsvHelper.Toint(r[21]);

		 if(r.Count >22)
			e.enemyHP_per= CsvHelper.Toint(r[22]);

		 if(r.Count >23)
			e.param_back= CsvHelper.Toint(r[23]);

		 if(r.Count >24)
			e.param_side= CsvHelper.Toint(r[24]);

		 if(r.Count >25)
			e.nearfrienddis_zero= CsvHelper.Toint(r[25]);

		 if(r.Count >26)
			e.fury_add= CsvHelper.Toint(r[26]);

		 if(r.Count >27)
			e.fury_add_full= CsvHelper.Toint(r[27]);

		 if(r.Count >28)
			e.enemyMov= CsvHelper.Toint(r[28]);

		 if(r.Count >29)
			e.harm_unit_distent= CsvHelper.Toint(r[29]);

		 if(r.Count >30)
			e.harm_unit_atk= CsvHelper.Toint(r[30]);

		 if(r.Count >31)
			e.unharm_unit_distent= CsvHelper.Toint(r[31]);

		 if(r.Count >32)
			e.unharm_unit_atk= CsvHelper.Toint(r[32]);

		 if(r.Count >33)
			e.enemySpeed= CsvHelper.Toint(r[33]);

			dic[e.id] = e;
		}

	}
}
