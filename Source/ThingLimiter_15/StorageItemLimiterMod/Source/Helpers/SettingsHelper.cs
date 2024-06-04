using Verse;

namespace StorageItemLimiterMod.Source.Helpers;

public static class SettingsHelper
{
	public static string GetMouseButtonDisplayName(int mouseButton)
	{
		if (mouseButton == 0)
		{
			return "leftMouseButton".Translate();
		}
		else if (mouseButton == 1)
		{
			return "rightMouseButton".Translate();
		}
		else if (mouseButton == 2)
		{
			return"middleMouseButton".Translate();
		}

		return "unknownMouseButton".Translate();
	}
}