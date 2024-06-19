using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Trackers.CopyTrackers;
using StorageItemLimiterMod.Source.Trackers.LimitTrackers;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.Patches.Expose;

[HarmonyPatch(typeof(Zone_Stockpile), nameof(Zone_Stockpile.ExposeData), null)]
public static class StockpileZone_ExposeData_Patcher
{
	public static void Postfix(Zone_Stockpile __instance)
	{
		if (!StorageLimitTracker.ThingLimitsByZone.ContainsKey(__instance))
		{
			StorageLimitTracker.ThingLimitsByZone.Add(__instance, new StorageLimitDictionary());
		}
		else if (StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder == null)
		{
			StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder = new();
		}
		
		Scribe_Collections.Look(ref StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder, "ThingLimitsByZone", LookMode.Value, LookMode.Value);
	}
}

[HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.ExposeData), null)]
public static class Building_Storage_ExposeData_Patcher
{
	public static void Postfix(Building_Storage __instance)
	{
		if (!StorageLimitTracker.ThingLimitsByStorage.ContainsKey(__instance))
		{
			StorageLimitTracker.ThingLimitsByStorage.Add(__instance, new StorageLimitDictionary());
		}
		else if (StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder == null)
		{
			StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder = new();
		}
		
		Scribe_Collections.Look(ref StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder, "ThingLimitsByStorage", LookMode.Value, LookMode.Value);
	}
}

[HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData), null)]
public static class StorageSettings_ExposeData_Patcher
{
	public static void Postfix(StorageSettings __instance)
	{
		if (!StorageCopyTracker.StorageGizmoSettings.ContainsKey(__instance))
		{
			StorageCopyTracker.StorageGizmoSettings.Add(__instance, new StorageGizmoSettings());
		}

		Scribe_Values.Look(ref StorageCopyTracker.StorageGizmoSettings[__instance].bShouldCopyLimitSettings, "bShouldCopyLimitSettings", false, false);
	}
}