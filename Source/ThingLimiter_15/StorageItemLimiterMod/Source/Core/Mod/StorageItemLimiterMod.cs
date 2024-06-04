using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using StorageItemLimiterMod.Source.Helpers;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.Core.Mod

{
	[StaticConstructorOnStartup]
	public class StorageItemLimiterMod : Verse.Mod
	{
		public StorageItemLimiterMod(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("ilkeraktug.StorageItemLimiterMod");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			GetSettings<StorageItemLimiterModSettings>();
		}

		public override string SettingsCategory()
		{
			return "StorageItemLimiterMod";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			int rowCount = 12;
			Listing_Standard listing_Standard = new Listing_Standard();
			Rect viewRect = new Rect(0f, 0f, inRect.width, rowCount * 26f);
			viewRect.xMax *= 0.9f;
			
			listing_Standard.Begin(viewRect);
			GUI.EndGroup();
			Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

			
			listing_Standard.Label(StringHTMLHelper.GetBoldString("inputHeadline".Translate()));
			listing_Standard.GapLine();
			
			if (listing_Standard.ButtonTextLabeledPct(
				    "mouseInputButtonLabel".Translate(), 
				    SettingsHelper.GetMouseButtonDisplayName(StorageItemLimiterModSettings.OpenMouseButton), 
				    0.6f, TextAnchor.MiddleLeft, 
				    null, "mouseInputButtonTooltip".Translate()))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.Add(new FloatMenuOption("leftMouseButton".Translate(), delegate
				{
					StorageItemLimiterModSettings.OpenMouseButton = 0;
				}));
				list.Add(new FloatMenuOption("rightMouseButton".Translate(), delegate
				{
					StorageItemLimiterModSettings.OpenMouseButton = 1;
				}));
				list.Add(new FloatMenuOption("middleMouseButton".Translate(), delegate
				{
					StorageItemLimiterModSettings.OpenMouseButton = 2;
				}));
				
				Find.WindowStack.Add(new FloatMenu(list));
			}
			
			listing_Standard.Gap();
			listing_Standard.Gap();
			listing_Standard.Label(StringHTMLHelper.GetBoldString("clearLimitSettingsHeadline".Translate()));
			listing_Standard.GapLine();
			
			listing_Standard.CheckboxLabeled("bShowClearLimitSettingsButton".Translate(), ref StorageItemLimiterModSettings.bShowClearLimitSettingsButton, "bShowClearLimitSettingsButtonTooltip".Translate());
			if (StorageItemLimiterModSettings.bShowClearLimitSettingsButton)
			{
				listing_Standard.CheckboxLabeled("bShowWarningWhenClearLimitSettingsClicked".Translate(), ref StorageItemLimiterModSettings.bShowWarningWhenClearLimitSettingsClicked, "bShowWarningWhenClearLimitSettingsClickedTooltip".Translate());
			}
			else
			{
				StorageItemLimiterModSettings.bShowWarningWhenClearLimitSettingsClicked = true;
				Rect newRect = new Rect(0, listing_Standard.CurHeight, inRect.width * 0.9f, 21.0f);
				if (Mouse.IsOver(newRect))
				{
					GUI.DrawTexture(newRect, (Texture)TexUI.HighlightTex);
				}

				listing_Standard.Label("bShowWarningWhenClearLimitSettingsClickedFalse".Translate());
			}
			
			listing_Standard.Gap();
			listing_Standard.Gap();
			listing_Standard.Label(StringHTMLHelper.GetBoldString("storageBuildingsHeadline".Translate()));
			listing_Standard.GapLine();
			
			listing_Standard.CheckboxLabeled("shouldShareLimitSettingsWhenLinkedLabel".Translate(), ref StorageItemLimiterModSettings.bShouldShareLimitSettingsWhenLinked, "shouldShareLimitSettingsWhenLinkedTooltip".Translate());
			if (StorageItemLimiterModSettings.bShouldShareLimitSettingsWhenLinked)
			{
				listing_Standard.CheckboxLabeled("shouldKeepLimitSettingsWhenUnlinked".Translate(), ref StorageItemLimiterModSettings.bShouldKeepLimitSettingsWhenUnlinked, "shouldKeepLimitSettingsWhenUnlinkedTooltip".Translate());
			}
			else
			{
				StorageItemLimiterModSettings.bShouldKeepLimitSettingsWhenUnlinked = false;
				Rect newRect = new Rect(0, listing_Standard.CurHeight, inRect.width * 0.9f, 21.0f);
				if (Mouse.IsOver(newRect))
				{
					GUI.DrawTexture(newRect, (Texture)TexUI.HighlightTex);
				}
				
				TooltipHandler.TipRegion(newRect, (TipSignal)"shouldKeepLimitSettingsWhenUnlinkedFalseTooltip".Translate());
				
				listing_Standard.Label("shouldKeepLimitSettingsWhenUnlinkedFalse".Translate());
			}

			Widgets.EndScrollView();
		}

		public static Vector2 scrollPosition;
	}
}