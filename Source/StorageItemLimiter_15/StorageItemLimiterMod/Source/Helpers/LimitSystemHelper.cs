using System;
using System.Collections.Generic;
using RimWorld;
using StorageItemLimiterMod.Source.Trackers.CopyTrackers;
using StorageItemLimiterMod.Source.Trackers.LimitTrackers;
using Verse;

namespace StorageItemLimiterMod.Source.Helpers;

public static class LimitSystemHelper
{
    public static int GetCurrentThingCount(Zone_Stockpile __instance, ThingDef thingDef)
    {
        if (__instance == null)
        {
            return 0;
        }
        
        int ThingCount = 0;
        
        ThingGrid thingGrid = __instance.Map.thingGrid;
        for (int index = 0; index < __instance.cells.Count; ++index)
        {
            List<Thing> thingList = thingGrid.ThingsListAt(__instance.cells[index]);
            
            foreach (var thing in thingList)
            {
                if (thing.def == thingDef)
                {
                    ThingCount += thing.stackCount;
                }
            }
        }

        return ThingCount;
    }
    public static int GetCurrentThingCount(Zone_Stockpile __instance, string thingLabel)
    {
        if (__instance == null)
        {
            return 0;
        }
        
        int ThingCount = 0;
        
        ThingGrid thingGrid = __instance.Map.thingGrid;
        for (int index = 0; index < __instance.cells.Count; ++index)
        {
            List<Thing> thingList = thingGrid.ThingsListAt(__instance.cells[index]);
            foreach (var thing in thingList)
            {
                if (thing.LabelNoCount.Equals(thingLabel, StringComparison.OrdinalIgnoreCase))
                {
                    ThingCount += thing.stackCount;
                }
            }
        }

        return ThingCount;
    }
    public static int GetThingLimit(Zone_Stockpile __instance, string thingLabel)
    {
        if (__instance == null)
        {
            return int.MaxValue;
        }
        
        int LimitSize = int.MaxValue;
        
        if (StorageLimitTracker.ThingLimitsByZone.ContainsKey(__instance))
        {
            if(StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder.ContainsKey(thingLabel))
            {
                LimitSize = StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder[thingLabel];
            }
        }

        return LimitSize;
    }
    public static int GetThingLimit(Zone_Stockpile __instance, ThingDef thingDef)
    {
        if (__instance == null)
        {
            return int.MaxValue;
        }
        
        int LimitSize = int.MaxValue;
        
        if (StorageLimitTracker.ThingLimitsByZone.ContainsKey(__instance))
        {
            if(StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder.ContainsKey(thingDef.LabelCap))
            {
                LimitSize = StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder[thingDef.LabelCap];
            }
        }

        return LimitSize;
    }
    public static bool HasEnoughStorage(Zone_Stockpile __instance, ThingDef thingDef)
    {
        int LeftNum = GetRemainingSpace(__instance, thingDef);

        return LeftNum == int.MaxValue || LeftNum > 0;
    }
    public static int GetRemainingSpace(Zone_Stockpile __instance, ThingDef thingDef)
    {
        if (thingDef == null || __instance == null)
        {
            return int.MaxValue;
        }
        
        int LimitSize = int.MaxValue;
        
        if (StorageLimitTracker.ThingLimitsByZone.ContainsKey(__instance))
        {
            if(StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder.ContainsKey(thingDef.LabelCap))
            {
                LimitSize = StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder[thingDef.LabelCap];
            }
        }

        if (LimitSize == int.MaxValue)
        {
            return int.MaxValue;
        }
        
        int ThingCount = 0;
        
        ThingGrid thingGrid = __instance.Map.thingGrid;
        for (int index = 0; index < __instance.cells.Count; ++index)
        {
            List<Thing> thingList = thingGrid.ThingsListAt(__instance.cells[index]);
            
            foreach (var thing in thingList)
            {
                if (thing.def == thingDef)
                {
                    ThingCount += thing.stackCount;
                }
            }
        }

        return LimitSize - ThingCount;
    }
    public static void UpdateThingLimit(Zone_Stockpile __instance, ThingDef thingDef, int Limit)
    {
        UpdateThingLimit(__instance, thingDef.LabelCap, Limit);
    }
    public static void UpdateThingLimit(Zone_Stockpile __instance, string thingDef, int Limit)
    {
        if (StorageLimitTracker.ThingLimitsByZone.ContainsKey(__instance))
        {
            if (StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder.ContainsKey(thingDef))
            {
                StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder[thingDef] = Limit;
            }
            else
            {
                StorageLimitTracker.ThingLimitsByZone[__instance].m_DataHolder.Add(thingDef, Limit);
            }
        }
        else
        {
            var MyDictionary = new Dictionary<string, int>();
            MyDictionary.Add(thingDef, Limit);
            StorageLimitTracker.ThingLimitsByZone.Add(__instance, new StorageLimitDictionary(MyDictionary));
        }
    }
    public static int GetCurrentThingCount(Building_Storage __instance, ThingDef thingDef)
    {
        int ThingCount = 0;
        
        if (__instance == null)
        {
            return ThingCount;
        }
        
        ThingGrid thingGrid = __instance.MapHeld.thingGrid;
        foreach (var slotCell in __instance.AllSlotCells())
        {
            List<Thing> thingList = thingGrid.ThingsListAt(slotCell);
            
            foreach (var thing in thingList)
            {
                if (thing.def == thingDef)
                {
                    ThingCount += thing.stackCount;
                }
            }
        }

        return ThingCount;
    }
    public static int GetCurrentThingCount(Building_Storage __instance, string thingLabel)
    {
        int ThingCount = 0;
        
        if (__instance == null)
        {
            return ThingCount;
        }
        
        ThingGrid thingGrid = __instance.MapHeld.thingGrid;
        foreach (var slotCell in __instance.AllSlotCells())
        {
            List<Thing> thingList = thingGrid.ThingsListAt(slotCell);
            
            foreach (var thing in thingList)
            {
                if (thing.LabelNoCount.Equals(thingLabel, StringComparison.OrdinalIgnoreCase))
                {
                    ThingCount += thing.stackCount;
                }
            }
        }

        return ThingCount;
    }
    public static int GetThingLimit(Building_Storage __instance, string thingLabel)
    {
        int LimitSize = int.MaxValue;
        
        if (__instance == null)
        {
            return LimitSize;
        }

        if (StorageLimitTracker.ThingLimitsByStorage.ContainsKey(__instance))
        {
            if(StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder.ContainsKey(thingLabel))
            {
                LimitSize = StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder[thingLabel];
            }
        }

        return LimitSize;
    }
    public static int GetThingLimit(Building_Storage __instance, ThingDef thingDef)
    {
        int LimitSize = int.MaxValue;
        
        if (__instance == null)
        {
            return LimitSize;
        }

        if (StorageLimitTracker.ThingLimitsByStorage.ContainsKey(__instance))
        {
            if(StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder.ContainsKey(thingDef.LabelCap))
            {
                LimitSize = StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder[thingDef.LabelCap];
            }
        }

        return LimitSize;
    }
    public static int GetRemainingSpace(Building_Storage __instance, ThingDef thingDef)
    {
        if (thingDef == null || __instance == null)
        {
            return int.MaxValue;
        }
        
        int LimitSize = GetThingLimit(__instance, thingDef);
        
        if (LimitSize == int.MaxValue)
        {
            return int.MaxValue;
        }
        
        int ThingCount = GetCurrentThingCount(__instance, thingDef);

        return LimitSize - ThingCount;
    }
    public static bool HasEnoughStorage(Building_Storage __instance, ThingDef thingDef)
    {
        int LeftNum = GetRemainingSpace(__instance, thingDef);

        return LeftNum == int.MaxValue || LeftNum > 0;
    }
    public static void UpdateThingLimit(Building_Storage __instance, ThingDef thingDef, int Limit)
    {
        UpdateThingLimit(__instance, thingDef.LabelCap, Limit);
    }
    public static void UpdateThingLimit(Building_Storage __instance, string thingDef, int Limit)
    {
        if (StorageLimitTracker.ThingLimitsByStorage.ContainsKey(__instance))
        {
            if (StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder.ContainsKey(thingDef))
            {
                StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder[thingDef] = Limit;
            }
            else
            {
                StorageLimitTracker.ThingLimitsByStorage[__instance].m_DataHolder.Add(thingDef, Limit);
            }
        }
        else
        {
            var MyDictionary = new Dictionary<string, int>();
            MyDictionary.Add(thingDef, Limit);
            StorageLimitTracker.ThingLimitsByStorage.Add(__instance, new StorageLimitDictionary(MyDictionary));
        }
    }
    
    public static void RemoveLimitSettings(object __instance)
    {
        if (__instance == null)
        {
            return;
        }
        
        if (__instance is Zone_Stockpile StockpileZone)
        {
            StorageLimitTracker.ThingLimitsByZone.Remove(StockpileZone);
        }
        else if (__instance is Building_Storage storageBuilding)
        {
            StorageLimitTracker.ThingLimitsByStorage.Remove(storageBuilding);
        }
    }

}

public static class StorageSettingsHelper
{
    public static void PasteFromObject(Zone_Stockpile sourceObject, Zone_Stockpile targetObject)
    {
        if (targetObject == null || !GetShouldCopyLimitSettings(sourceObject.settings))
        {
            return;
        }
        
        if (!StorageLimitTracker.ThingLimitsByZone.ContainsKey(sourceObject))
        {
            return;
        }
        if(StorageLimitTracker.ThingLimitsByZone.ContainsKey(targetObject))
        {
            StorageLimitTracker.ThingLimitsByZone[targetObject] = StorageLimitTracker.ThingLimitsByZone[sourceObject];
        }
        else
        {
            StorageLimitTracker.ThingLimitsByZone.Add(targetObject, StorageLimitTracker.ThingLimitsByZone[sourceObject]);
        }
    }
    
    public static void PasteFromObject(Building_Storage sourceObject, Building_Storage targetObject, bool bFromSetStorages = false)
    {
        if (targetObject == null || sourceObject == null)
        {
            return;
        }

        if (!bFromSetStorages && !GetShouldCopyLimitSettings(sourceObject.settings))
        {
            return;
        }
        
        if (targetObject == sourceObject)
        {
            return;
        }
        
        if (!StorageLimitTracker.ThingLimitsByStorage.ContainsKey(sourceObject))
        {
            return;
        }
        
        if(StorageLimitTracker.ThingLimitsByStorage.ContainsKey(targetObject))
        {
            StorageLimitTracker.ThingLimitsByStorage[targetObject] = StorageLimitTracker.ThingLimitsByStorage[sourceObject];
        }
        else
        {
            StorageLimitTracker.ThingLimitsByStorage.Add(targetObject, StorageLimitTracker.ThingLimitsByStorage[sourceObject]);
        }
    }

    public static bool GetShouldCopyLimitSettings(StorageSettings storageSettings)
    {
        if (StorageCopyTracker.StorageGizmoSettings.ContainsKey(storageSettings))
        {
            return StorageCopyTracker.StorageGizmoSettings[storageSettings].bShouldCopyLimitSettings;
        }

        return false;
    }
    
    public static void ToggleShouldCopyLimitSettings(StorageSettings storageSettings)
    {
        if (StorageCopyTracker.StorageGizmoSettings.ContainsKey(storageSettings))
        {
            Log.Message(string.Format("First{0}", StorageCopyTracker.StorageGizmoSettings[storageSettings].bShouldCopyLimitSettings.ToString()));
            StorageCopyTracker.StorageGizmoSettings[storageSettings].bShouldCopyLimitSettings = !StorageCopyTracker.StorageGizmoSettings[storageSettings].bShouldCopyLimitSettings;
            Log.Message(string.Format("Second{0}", StorageCopyTracker.StorageGizmoSettings[storageSettings].bShouldCopyLimitSettings.ToString()));
        }
        else
        {
            StorageCopyTracker.StorageGizmoSettings.Add(storageSettings, new StorageGizmoSettings(true));
        }
    }
}