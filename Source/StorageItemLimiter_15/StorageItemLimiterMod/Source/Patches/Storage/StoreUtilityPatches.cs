using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Helpers;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.Patches.Storage;

[HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
[HarmonyPriority(Priority.First)]
public static class StoreUtility_TryFindBestBetterStoreCellFor_Patches
{
    private static void TryFindBestBetterStoreCellForWorker( 
        Thing t,
        Pawn carrier,
        Map map,
        Faction faction,
        ISlotGroup slotGroup,
        bool needAccurateResult,
        ref IntVec3 closestSlot,
        ref float closestDistSquared,
        ref StoragePriority foundPriority)
    {
        if (slotGroup == null || !slotGroup.Settings.AllowedToAccept(t))
            return;
        IntVec3 intVec3 = t.SpawnedOrAnyParentSpawned ? t.PositionHeld : carrier.PositionHeld;
        List<IntVec3> cellsList = slotGroup.CellsList;
        int count = cellsList.Count;
        int num = !needAccurateResult ? 0 : Mathf.FloorToInt((float) count * Rand.Range(0.005f, 0.018f));
        for (int index = 0; index < count; ++index)
        {
            IntVec3 c = cellsList[index];
            float horizontalSquared = (float) (intVec3 - c).LengthHorizontalSquared;
            if ((double) horizontalSquared <= (double) closestDistSquared && StoreUtility.IsGoodStoreCell(c, map, t, carrier, faction))
            {
                closestSlot = c;
                closestDistSquared = horizontalSquared;
                foundPriority = slotGroup.Settings.Priority;
                if (index >= num)
                    break;
            }
        }
    }
    
    public static bool Prefix(ref bool __result,
        Thing t,
        Pawn carrier,
        Map map,
        StoragePriority currentPriority,
        Faction faction,
        out IntVec3 foundCell,
        bool needAccurateResult = true)
    {
        List<SlotGroup> listInPriorityOrder = map.haulDestinationManager.AllGroupsListInPriorityOrder;
        if (listInPriorityOrder.Count == 0)
        {
            foundCell = IntVec3.Invalid;
            __result = false;
            return false;
        }
        
        StoragePriority foundPriority = currentPriority;
        float maxValue = (float) int.MaxValue;
        IntVec3 invalid = IntVec3.Invalid;
        int count = listInPriorityOrder.Count;
        
        for (int index = 0; index < count; ++index)
        {
            SlotGroup slotGroup = listInPriorityOrder[index];

            if (slotGroup.parent.GetType().FullName == "AdaptiveStorage.ThingClass")
            {
                foundCell = IntVec3.Invalid;
                return true;
            }
            
            if (!LimitSystemHelper.HasEnoughStorage(slotGroup.parent as Zone_Stockpile, t.def))
            {
                continue;
            }
            
            if (!LimitSystemHelper.HasEnoughStorage(slotGroup.parent as Building_Storage, t.def))
            {
                continue;
            }

            StoragePriority priority = slotGroup.Settings.Priority;
            if (priority >= foundPriority && priority > currentPriority)
                TryFindBestBetterStoreCellForWorker(t, carrier, map, faction, (ISlotGroup) slotGroup, needAccurateResult, ref invalid, ref maxValue, ref foundPriority);
            else
                break;
        }
        if (!invalid.IsValid)
        {
            foundCell = IntVec3.Invalid;
            __result = false;
            return false;
        }
        foundCell = invalid;
        __result = true;
        
        return false;
    }
}