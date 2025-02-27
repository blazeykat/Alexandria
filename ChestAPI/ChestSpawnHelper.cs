﻿using Dungeonator;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Alexandria.ChestApi
{
	public static class ChestSpawnHelper
	{
		public static void Init()
		{
			//var ConfigureOnPlacementHook = new Hook(
			//		typeof(FloorChestPlacer).GetMethod("ConfigureOnPlacement", BindingFlags.Instance | BindingFlags.Public),
			//		typeof(ChestSpawnHelper).GetMethod("ConfigureOnPlacementHook", BindingFlags.Static | BindingFlags.Public));
		}

		public static void RegisterCustomChest(Chest chest, float weight = 1)
		{

		}

		//d 0.25
		//c 0.32
		//b 0.2
		//a 0.09
		//s 0.04


		public static void ConfigureOnPlacementHook(Action<FloorChestPlacer, RoomHandler> orig, FloorChestPlacer self, RoomHandler room)
		{


			FloorRewardData rewardDataForFloor = GameManager.Instance.RewardManager.GetRewardDataForFloor2(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId);
			bool forceDChanceZero = StaticReferenceManager.DChestsSpawnedInTotal >= 2;

			var chest = GetTargetCustomChest(UnityEngine.Random.value);
			if (chest)
			{
				self.OverrideChestPrefab = chest;
				self.UseOverrideChest = true;
				DungeonPrerequisite dungeonPrerequisite = new DungeonPrerequisite();
				dungeonPrerequisite.requireTileset = true;
				dungeonPrerequisite.requiredTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
				dungeonPrerequisite.prerequisiteType = DungeonPrerequisite.PrerequisiteType.TILESET;
				self.OverrideChestPrereq = dungeonPrerequisite;
			}
			orig(self, room);
		}

		public static Chest GetTargetCustomChest(float fran)
		{
			//float currentMagnificence = GameManager.Instance.RewardManager.CurrentRewardData.DetermineCurrentMagnificence(isGenerationForMagnificence);

			float dumbFuckingChestNumber = 0;
			foreach (var chest in customChests)
			{
				dumbFuckingChestNumber += chest.Value.Second;
				if (fran < dumbFuckingChestNumber)
				{
					return chest.Value.First;
				}
			}
			return null;
		}

		public static FloorRewardData GetRewardDataForFloor2(this RewardManager rewardManager, GlobalDungeonData.ValidTilesets targetTileset)
		{
			FloorRewardData floorRewardData = null;
			for (int i = 0; i < rewardManager.FloorRewardData.Count; i++)
			{
				if ((rewardManager.FloorRewardData[i].AssociatedTilesets | targetTileset) == rewardManager.FloorRewardData[i].AssociatedTilesets)
				{
					floorRewardData = rewardManager.FloorRewardData[i];
				}
			}
			if (floorRewardData == null)
			{
				floorRewardData = rewardManager.FloorRewardData[0];
			}
			return floorRewardData;
		}


		public static Dictionary<string, Tuple<Chest, float>> customChests = new Dictionary<string, Tuple<Chest, float>>();

	}
}
