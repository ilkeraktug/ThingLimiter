using Verse;

namespace ThingLimiterMod.Source.Core.Mod;

public class ThingLimiterModSettings : ModSettings
{
	public static bool bShouldShareLimitSettingsWhenLinked = false;
	public static bool bShouldKeepLimitSettingsWhenUnlinked = false;

	public static int OpenMouseButton = 2;
	
	public override void ExposeData()
	{
		base.ExposeData();
		
		Scribe_Values.Look(ref bShouldShareLimitSettingsWhenLinked, "bShouldShareLimitSettingsWhenLinked", false, false);
		Scribe_Values.Look(ref bShouldKeepLimitSettingsWhenUnlinked, "bShouldKeepLimitSettingsWhenUnlinked", false, false);
		Scribe_Values.Look(ref OpenMouseButton, "OpenMouseButton", 2, false);
	}
}