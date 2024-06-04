using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Core.Mod;
using StorageItemLimiterMod.Source.Helpers;
using StorageItemLimiterMod.Source.Trackers.LimitTrackers;
using Verse;

namespace StorageItemLimiterMod.Source.Patches.Storage;


[HarmonyPatch(typeof(StorageGroupUtility), nameof(StorageGroupUtility.SetStorageGroup), null)]
public static class StorageGroupUtility_SetStorageGroup_Patches
{
	public static void Postfix(IStorageGroupMember member, StorageGroup newGroup, bool removeIfEmpty = true)
	{
		if (StorageItemLimiterModSettings.bShouldShareLimitSettingsWhenLinked)
		{
			foreach (object selectedObject in Find.Selector.SelectedObjects)
			{
				StorageSettingsHelper.PasteFromObject(member as Building_Storage, selectedObject as Building_Storage, true);
			}
		}
	}
}

[HarmonyPatch(typeof(StorageGroup), nameof(StorageGroup.RemoveMember), null)]
public static class StorageGroup_RemoveMember_Patches
{
	public static void Postfix(IStorageGroupMember member, bool removeIfEmpty = true)
	{
		if (!StorageItemLimiterModSettings.bShouldKeepLimitSettingsWhenUnlinked)
		{
			foreach (object selectedObject in Find.Selector.SelectedObjects)
			{
				if (selectedObject == member)
				{
					if (member is Building_Storage storageBuilding)
					{
						StorageLimitTracker.ThingLimitsByStorage.Remove(storageBuilding);

						return;
					}
				}
			}
		}
	}
}

// [HarmonyPatch(typeof(StorageGroupUtility), nameof(StorageGroupUtility.StorageGroupMemberGizmos), null)]
// public static class StockpileZoneDeleteListener_ILKER
// {
// 	public static void Postfix(ref IEnumerable<Gizmo> __result, IStorageGroupMember member)
// 	{
// 		Command_Toggle command_Toggle = new Command_Toggle();
// 		command_Toggle.defaultLabel = "setShouldShareLimitSettings".Translate();
// 		command_Toggle.defaultDesc = "setShouldShareLimitSettingsDesc".Translate();
// 		//command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
// 		command_Toggle.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/LinkStorageSettings");;
// 		command_Toggle.isActive = () => true; //() => StorageSettingsHelper.GetShouldShareLimitSettings(member.StoreSettings);
// 		command_Toggle.toggleAction = delegate
// 		{
// 			StorageSettingsHelper.ToggleShouldShareLimitSettings(member.StoreSettings);
// 		};
//
// 		__result = __result.AddItem(command_Toggle);
// 	}
// }