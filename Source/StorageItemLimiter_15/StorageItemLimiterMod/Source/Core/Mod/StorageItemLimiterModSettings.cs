using Verse;

namespace StorageItemLimiterMod.Source.Core.Mod;

public class StorageItemLimiterModSettings : ModSettings
{
	public static bool bShouldCountThingsAsOneLinkedStorage = true;
	public static bool bShouldShareLimitSettingsWhenLinked = true;
	public static bool bShouldKeepLimitSettingsWhenUnlinked = false;

	public static int OpenMouseButton = 2;

	public static bool bShowClearLimitSettingsButton = true;
	public static bool bShowWarningWhenClearLimitSettingsClicked = true;
	
	public override void ExposeData()
	{
		base.ExposeData();
		
		Scribe_Values.Look(ref bShouldCountThingsAsOneLinkedStorage, "bShouldCountThingsAsOneLinkedStorage", true, false);
		Scribe_Values.Look(ref bShouldShareLimitSettingsWhenLinked, "bShouldShareLimitSettingsWhenLinked", true, false);
		Scribe_Values.Look(ref bShouldKeepLimitSettingsWhenUnlinked, "bShouldKeepLimitSettingsWhenUnlinked", false, false);
		Scribe_Values.Look(ref OpenMouseButton, "OpenMouseButton", 2, false);
		Scribe_Values.Look(ref bShowClearLimitSettingsButton, "bShowClearLimitSettingsButton", true, false);
		Scribe_Values.Look(ref bShowWarningWhenClearLimitSettingsClicked, "bShowWarningWhenClearLimitSettingsClicked", true, false);
	}
}