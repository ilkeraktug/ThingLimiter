using System.Collections.Generic;
using RimWorld;

namespace ThingLimiterMod.Source.Trackers.LimitTrackers;

public static class StorageLimitTracker
{
    public static Dictionary<Zone_Stockpile, StorageLimitDictionary> ThingLimitsByZone =  new();

    public static Dictionary<Building_Storage, StorageLimitDictionary> ThingLimitsByStorage =  new();
}

public class StorageLimitDictionary
{
    public Dictionary<string, int> m_DataHolder;
    public StorageLimitDictionary()
    {
        m_DataHolder = new Dictionary<string, int>();
    }
    public StorageLimitDictionary(Dictionary<string, int> otherDictionary)
    {
        m_DataHolder = otherDictionary;
    }
}