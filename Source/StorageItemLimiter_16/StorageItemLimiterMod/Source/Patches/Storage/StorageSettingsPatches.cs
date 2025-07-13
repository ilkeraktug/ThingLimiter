using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Helpers;
using StorageItemLimiterMod.Source.Trackers.CopyTrackers;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace StorageItemLimiterMod.Source.Patches.Storage;

[HarmonyPatch(typeof(StorageSettingsClipboard), "Copy")]
public static class StorageSettingsPatches_CopyFrom_Patches
{
	public static void Postfix(StorageSettings s)
	{
		StorageCopyTracker.LastCopiedObject = s.owner;
	}
}

[HarmonyPatch(typeof(StorageSettingsClipboard), "PasteInto")]
public static class StorageSettingsPatches_PasteInto_Patches
{
	public static void Postfix(StorageSettings s)
	{
		if (StorageCopyTracker.LastCopiedObject is Zone_Stockpile copiedObjectStockpile)
		{
			StorageSettingsHelper.PasteFromObject(copiedObjectStockpile, s.owner as Zone_Stockpile);
		}
		else if (StorageCopyTracker.LastCopiedObject is Building_Storage copiedObjectStorage)
		{
			StorageSettingsHelper.PasteFromObject(copiedObjectStorage, s.owner as Building_Storage);
		}
	}
}

[HarmonyPatch(typeof(StorageSettingsClipboard), "CopyPasteGizmosFor")]
public static class StorageSettingsPatches_CopyPasteGizmosFor_Patches
{
	public static void Postfix(ref IEnumerable<Gizmo> __result, StorageSettings s)
	{
		Command_Toggle command_Toggle = new Command_Toggle();
		command_Toggle.defaultLabel = "setShouldCopyLimitSettings".Translate();
		command_Toggle.defaultDesc = "setShouldCopyLimitSettingsDesc".Translate();
		command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
		command_Toggle.icon = (Texture) ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");;
		command_Toggle.isActive = () => StorageSettingsHelper.GetShouldCopyLimitSettings(s);
		command_Toggle.toggleAction = delegate
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			StorageSettingsHelper.ToggleShouldCopyLimitSettings(s);
		};

		__result = __result.AddItem(command_Toggle);
	}
}