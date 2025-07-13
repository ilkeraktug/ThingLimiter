using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Trackers.LimitTrackers;
using Verse;

namespace StorageItemLimiterMod.Source.Patches;

[HarmonyPatch(typeof(Zone), nameof(Zone.Deregister), null)]
public static class Zone_Deregister_Patches
{
	public static void Postfix(Zone __instance)
	{
		if (__instance is Zone_Stockpile StockpileZone)
		{
			StorageLimitTracker.ThingLimitsByZone.Remove(StockpileZone);
		}
	}
}

[HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.DeSpawn), null)]
public static class Building_Storage_DeSpawn_Patches
{
	public static void Postfix(Building_Storage __instance, DestroyMode mode)
	{
		if (__instance == null)
		{
			return;
		}

		StorageLimitTracker.ThingLimitsByStorage.Remove(__instance);
	}
}