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
			if (StorageLimitTracker.ThingLimitsByZone.TryGetValue(StockpileZone.ID, out var limitDictionary))
			{
				StorageLimitTracker.ThingLimitsByZoneCopy.Add(StockpileZone.ID, limitDictionary);
				StorageLimitTracker.ThingLimitsByZoneCopy.Remove(StockpileZone.ID);
			}
		}
	}
}

[HarmonyPatch(typeof(Zone), nameof(Zone.PostRegister), null)]
public static class Zone_PostRegister_Patches
{
	public static void Postfix(Zone __instance)
	{
		if (__instance is Zone_Stockpile StockpileZone)
		{
			if (StorageLimitTracker.ThingLimitsByZoneCopy.TryGetValue(StockpileZone.ID, out var limitDictionary))
			{
				StorageLimitTracker.ThingLimitsByZone.Add(StockpileZone.ID, limitDictionary);
				StorageLimitTracker.ThingLimitsByZone.Remove(StockpileZone.ID);
			}
		}
	}
}

[HarmonyPatch(typeof(Building_Storage), nameof(Building_Storage.DeSpawn), null)]
public static class Building_Storage_DeSpawn_Patches
{
	public static void Postfix(Building_Storage __instance, DestroyMode mode)
	{
		if (__instance == null || mode == DestroyMode.WillReplace)
		{
			return;
		}

		StorageLimitTracker.ThingLimitsByStorage.Remove(__instance);
	}
}