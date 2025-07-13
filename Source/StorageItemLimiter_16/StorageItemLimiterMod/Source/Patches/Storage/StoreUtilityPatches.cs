using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Helpers;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.Patches.Storage;

[HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
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

// [HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterNonSlotGroupStorageFor")]
// public static class StoreUtility_TryFindBestBetterNonSlotGroupStorageFor_Patches
// {
//     public static bool Prefix(ref bool __result,
//         Thing t,
//         Pawn carrier,
//         Map map,
//         StoragePriority currentPriority,
//         Faction faction,
//         out IHaulDestination haulDestination,
//         bool acceptSamePriority = false,
//         bool requiresDestReservation = true)
//     {
//         {
//             List<IHaulDestination> listInPriorityOrder =
//                 map.haulDestinationManager.AllHaulDestinationsListInPriorityOrder;
//             IntVec3 intVec3 = t.SpawnedOrAnyParentSpawned ? t.PositionHeld : carrier.PositionHeld;
//             float num = float.MaxValue;
//             StoragePriority storagePriority = StoragePriority.Unstored;
//             haulDestination = null;
//             for (int index = 0; index < listInPriorityOrder.Count; ++index)
//             {
//                 if (!(listInPriorityOrder[index] is ISlotGroupParent) &&
//                     (!(listInPriorityOrder[index] is Building_Grave) || t.CanBeBuried()))
//                 {
//                     if (!Helper.ZoneHasEnoughStorage(listInPriorityOrder[index] as Zone_Stockpile, t.def))
//                     {
//                 Log.Message("ILKER 100");
//                         continue;
//                     }
//                     
//                     if (!Helper.StorageHasEnoughStorage(listInPriorityOrder[index] as Building_Storage, t.def))
//                     {
//                 Log.Message("ILKER 200");
//                         continue;
//                     }
//                     
//                     StoragePriority priority = listInPriorityOrder[index].GetStoreSettings().Priority;
//                     if (priority >= storagePriority && (!acceptSamePriority || priority >= currentPriority) &&
//                         (acceptSamePriority || priority > currentPriority))
//                     {
//                         float squared = (float)intVec3.DistanceToSquared(listInPriorityOrder[index].Position);
//                         if ((double)squared <= (double)num && listInPriorityOrder[index].Accepts(t))
//                         {
//                             if (listInPriorityOrder[index] is Thing thing)
//                             {
//                                 if (carrier != null && thing.IsForbidden(carrier))
//                                 {
//                                     continue;
//                                 }
//                                 else if (faction != null && thing.IsForbidden(faction))
//                                 {
//                                     continue;
//                                 }
//
//                                 if (requiresDestReservation)
//                                 {
//                                     if (thing is IHaulEnroute enroute)
//                                     {
//                                         if (!map.reservationManager.OnlyReservationsForJobDef((LocalTargetInfo)thing,
//                                                 JobDefOf.HaulToContainer) ||
//                                             enroute.GetSpaceRemainingWithEnroute(t.def) <= 0)
//                                         {
//                                             continue;
//                                         }
//                                     }
//                                     else if (carrier != null)
//                                     {
//                                         if (!carrier.CanReserveNew((LocalTargetInfo)thing))
//                                         {
//                                             continue;
//                                         }
//                                     }
//                                     else if (faction != null &&
//                                              map.reservationManager.IsReservedByAnyoneOf((LocalTargetInfo)thing,
//                                                  faction))
//                                     {
//                                         continue;
//
//                                     }
//                                 }
//
//                                 if (carrier != null)
//                                 {
//                                     if (thing != null)
//                                     {
//                                         if (!carrier.Map.reachability.CanReach(intVec3, (LocalTargetInfo)thing,
//                                                 PathEndMode.ClosestTouch, TraverseParms.For(carrier)))
//                                         {
//                                             continue;
//                                         }
//                                     }
//                                     else if (!carrier.Map.reachability.CanReach(intVec3,
//                                                  (LocalTargetInfo)listInPriorityOrder[index].Position,
//                                                  PathEndMode.ClosestTouch, TraverseParms.For(carrier)))
//                                     {
//                                         continue;
//                                     }
//                                 }
//
//                                 num = squared;
//                                 storagePriority = priority;
//                                 haulDestination = listInPriorityOrder[index];
//                             }
//                             else
//                             {
//                                 break;
//                             }
//                         }
//                     }
//                 }
//             }
//
//             __result = haulDestination != null;
//             return false;
//         }
//     }
// }

// [HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStorageFor")]
// public static class StoreUtilityPatches
// {
//     public static bool Prefix(ref bool __result, Thing t, Pawn carrier, Map map, StoragePriority currentPriority, Faction faction, out IntVec3 foundCell, out IHaulDestination haulDestination, bool needAccurateResult = true)
//     {
//         IntVec3 foundCell2 = IntVec3.Invalid;
//         StoragePriority storagePriority = StoragePriority.Unstored;
//         if (StoreUtility.TryFindBestBetterStoreCellFor(t, carrier, map, currentPriority, faction, out foundCell2, needAccurateResult))
//         {
//             storagePriority = foundCell2.GetSlotGroup(map).Settings.Priority;
//         }
//         if (!StoreUtility.TryFindBestBetterNonSlotGroupStorageFor(t, carrier, map, currentPriority, faction, out var haulDestination2))
//         {
//             haulDestination2 = null;
//         }
//         
//         bool HasEnoughRoom1 = Helper.ZoneHasEnoughStorage(foundCell2.GetSlotGroup(map).parent as Zone_Stockpile, t);
//         bool HasEnoughRoom2 = Helper.ZoneHasEnoughStorage(haulDestination2 as Zone_Stockpile, t);
//
//         if (storagePriority == StoragePriority.Unstored && haulDestination2 == null)
//         {
//             foundCell = IntVec3.Invalid;
//             haulDestination = null;
//             __result = false;
//             
//             return false;
//         }
//         if (haulDestination2 != null && (storagePriority == StoragePriority.Unstored || (int)haulDestination2.GetStoreSettings().Priority > (int)storagePriority))
//         {
//             foundCell = IntVec3.Invalid;
//             haulDestination = haulDestination2;
//             __result = true;
//             
//             return false;
//         }
//         foundCell = foundCell2;
//         haulDestination = foundCell2.GetSlotGroup(map).parent;
//         __result = true;
//         
//         return false;
//     }
//     
//     public static void Postfix(Thing t, ref IHaulDestination haulDestination, ref bool __result)
//     {
//         return;
//         Log.Message("PostFix");
//         Log.Message(String.Format("__instance : {0}", haulDestination?.ToString()));
//         Log.Message(String.Format("Thing : {0}", t?.ToString()));
//         
//         if (haulDestination is Zone_Stockpile)
//         {
//             Zone_Stockpile stockpile = haulDestination as Zone_Stockpile;
//         
//
//             int ThingCount = 0;
//
//             ThingGrid thingGrid = stockpile.Map.thingGrid;
//             for (int index = 0; index < stockpile.cells.Count; ++index)
//             {
//                 Log.Message("ILKER100");
//                 List<Thing> thingList = thingGrid.ThingsListAt(stockpile.cells[index]);
//
//                 foreach (var thing in thingList)
//                 {
//                 Log.Message("ILKER200");
//                 Log.Message(String.Format("thing({0}), t({1})", thing.ToString(), t.ToString()));
//                     if (thing.def == t.def)
//                     {
//                 Log.Message("ILKER300");
//                         ThingCount += thing.stackCount;
//                     }
//                 }
//             }
//
//             //Log.Message(String.Format("t{0} ThingCount {1}", t.def.LabelCap.ToString(), ThingCount.ToString()));
//
//             int LimitSize = 0;
//
//             if (ThingDefContainer.ThingLimitsByZone.ContainsKey(stockpile))
//             {
//                 foreach (var VARIABLE in ThingDefContainer.ThingLimitsByZone[stockpile])
//                 {
//                     Log.Message(String.Format("VARIABLE({0})", VARIABLE.ToString()));
//                 }
//
//                 if (ThingDefContainer.ThingLimitsByZone[stockpile].ContainsKey(t.def.LabelCap))
//                 {
//                     LimitSize = ThingDefContainer.ThingLimitsByZone[stockpile][t.def.LabelCap];
//                 }
//             }
//
//             //Log.Message(String.Format("t{0} LimitSize {1}", t.def.LabelCap.ToString(), LimitSize.ToString()));
//
//             if (LimitSize <= 0)
//             {
//                 return;
//             }
//
//             if (ThingCount >= LimitSize)
//             {
//                 Log.Message(String.Format("100 Thing({0}, ThingCount{1}, LimitSize{2})", t.def.LabelCap.ToString(),
//                     ThingCount.ToString(), LimitSize.ToString()));
//                 __result = false;
//             }
//             else
//             {
//                 Log.Message(String.Format("200 Thing({0}, ThingCount{1}, LimitSize{2})", t.def.LabelCap.ToString(),
//                     ThingCount.ToString(), LimitSize.ToString()));
//             }
//
//             // if (ThingDefContainer.ThingSizeByZone.ContainsKey(__instance))
//             // {
//             //     if(ThingDefContainer.ThingSizeByZone[__instance].ContainsKey(t.def.LabelCap))
//             //     {
//             //         uint CurrentSize = ThingDefContainer.ThingSizeByZone[__instance][t.def.LabelCap];
//             //
//             //         if (CurrentSize >= LimitSize)
//             //         {
//             //             __result = false;
//             //         }
//             //     }
//             // }
//         }
//     }
// }
//
// [HarmonyPatch(typeof(StoreUtility), "IsInValidBestStorage")]
// public static class StoreUtilityPatches_IsInValidBestStorage
// {
//     public static bool Prefix(Thing t, ref bool __result)
//     {
//         return true;
//         IHaulDestination haulDestination = StoreUtility.CurrentHaulDestinationOf(t);
//
//         if (haulDestination == null)
//         {
//             __result = false;
//             return false;
//         }
//
//         if (StoreUtility.TryFindBestBetterStorageFor(t, null, t.MapHeld, haulDestination.GetStoreSettings().Priority,
//                 Faction.OfPlayer, out var _, out var _, needAccurateResult: false))
//         {
//             __result = false;
//             return false;
//         }
//
//         if (!haulDestination.Accepts(t))
//         {
//             __result = false;
//             return false;
//         }
//
//         __result = true;
//         return false;
//     }
// }