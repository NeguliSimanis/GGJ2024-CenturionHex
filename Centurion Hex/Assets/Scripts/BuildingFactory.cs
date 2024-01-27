using Assets.Scripts.Buildings;
using Assets.Scripts.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BuildingFactory
{
	public static Building CreateBuilding(Building.BuildingType buildingType)
	{
		switch (buildingType)
		{
			case Building.BuildingType.btSenate:
				return new Senate();
		}
		return null;
	}
}
