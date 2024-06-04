using System.Collections.Generic;
using RimWorld;

namespace StorageItemLimiterMod.Source.Trackers.CopyTrackers;

public static class StorageCopyTracker
{
	public static Dictionary<StorageSettings, StorageGizmoSettings> StorageGizmoSettings =  new();

	public static object LastCopiedObject;
}

public class StorageGizmoSettings
{
	public bool bShouldCopyLimitSettings;

	public StorageGizmoSettings()
	{
		bShouldCopyLimitSettings = false;
	}
	public StorageGizmoSettings(bool ShouldCopyLimitSettings)
	{
		bShouldCopyLimitSettings = ShouldCopyLimitSettings;
	}
}