using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Helpers;
using Verse;
using Verse.AI;

namespace StorageItemLimiterMod.Source.Patches;

[HarmonyPatch(typeof(HaulAIUtility), "HaulToCellStorageJob")]
public static class HaulAIUtility_HaulToCellStorageJob_Patches
{
    public static void Postfix(ref Job __result, Pawn p, Thing t, IntVec3 storeCell, bool fitInStoreCell)
    {
        if (GridsUtility.GetZone(storeCell, p.Map) is Zone_Stockpile StockpileZone)
        {
            int LeftNum = LimitSystemHelper.GetRemainingSpace(StockpileZone, t.def);
            
            if (LeftNum == int.MaxValue)
            {
                return;
            }
            
            if (__result.count > LeftNum)
            {
                __result.count = LeftNum;
                return;
            }
        }

        ThingGrid thingGrid = p.Map.thingGrid;

        foreach (var thing in thingGrid.ThingsListAt(storeCell))
        {
            if (thing is Building_Storage storageBuilding)
            {
                int LeftNum = LimitSystemHelper.GetRemainingSpace(storageBuilding, t.def);
            
                if (LeftNum == int.MaxValue)
                {
                    return;
                }
            
                if (__result.count > LeftNum)
                {
                    __result.count = LeftNum;
                    return;
                }
            }
        }
    }
}
