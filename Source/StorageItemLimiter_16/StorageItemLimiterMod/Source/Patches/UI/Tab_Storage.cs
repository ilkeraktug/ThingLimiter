using HarmonyLib;
using RimWorld;
using StorageItemLimiterMod.Source.Core.Mod;
using StorageItemLimiterMod.Source.Helpers;
using StorageItemLimiterMod.Source.UI;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.Patches.UI;

[HarmonyPatch(typeof(ITab_Storage), "FillTab")]
public class Tab_Storage_FillTab
{
	public static void Postfix(ITab_Storage __instance)
	{
		if (StorageItemLimiterModSettings.bShowClearLimitSettingsButton)
		{
			float TopAreaHeight = 35.0f;
		
			Rect rect2 = new Rect(180.0f, 10f, 95.0f, TopAreaHeight - 6f);
			
			Text.Font = GameFont.Tiny;
			if (Widgets.ButtonText(rect2, StringHTMLHelper.GetColoredString((string)"clearItemLimitsLabel".Translate("\n"), HTMLColors.Red)))
			{
				if (StorageItemLimiterModSettings.bShowWarningWhenClearLimitSettingsClicked)
				{
					string FirstObjectLabel = "";
					
					if (Find.Selector.FirstSelectedObject is Thing selectedObject)
					{
						FirstObjectLabel = selectedObject.Label;
					}
					else if (Find.Selector.FirstSelectedObject is Zone selectedZone)
					{
						FirstObjectLabel = selectedZone.label;
					}
					
					string WarningLabel = (string)"clearItemLimitsWarningLabel".Translate() + 
					                      (string)StringHTMLHelper.GetColoredString(StringHTMLHelper.RemoveDigits(FirstObjectLabel), HTMLColors.Cyan);


					
					Find.WindowStack.Add(new Dialog_ClearLimitSettingsWarning(WarningLabel, 
						StringHTMLHelper.GetColoredString("clearItemLimitsWarningYesButton".Translate(), HTMLColors.Red, true),
						"clearItemLimitsWarningTip".Translate(),
						delegate
						{
							LimitSystemHelper.RemoveLimitSettings(Find.Selector.FirstSelectedObject);
						}));
				}
				else
				{
					LimitSystemHelper.RemoveLimitSettings(Find.Selector.FirstSelectedObject);
				}
			}
		}
	}
}