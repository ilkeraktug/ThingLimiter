using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ThingLimiterMod.Source.Helpers;
using UnityEngine;
using Verse;

namespace ThingLimiterMod.Source.Core.Mod

{
	[StaticConstructorOnStartup]
	public class ThingLimiterMod : Verse.Mod
	{
		public ThingLimiterMod(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("ilkeraktug.ThingLimiterMod");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			GetSettings<ThingLimiterModSettings>();
		}

		public override string SettingsCategory()
		{
			return "ThingLimiterMod";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			int rowCount = 8;
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
				    SettingsHelper.GetMouseButtonDisplayName(ThingLimiterModSettings.OpenMouseButton), 
				    0.6f, TextAnchor.MiddleLeft, 
				    null, "mouseInputButtonTooltip".Translate()))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				list.Add(new FloatMenuOption("leftMouseButton".Translate(), delegate
				{
					ThingLimiterModSettings.OpenMouseButton = 0;
				}));
				list.Add(new FloatMenuOption("rightMouseButton".Translate(), delegate
				{
					ThingLimiterModSettings.OpenMouseButton = 1;
				}));
				list.Add(new FloatMenuOption("middleMouseButton".Translate(), delegate
				{
					ThingLimiterModSettings.OpenMouseButton = 2;
				}));
				
				Find.WindowStack.Add(new FloatMenu(list));
			}
			
			listing_Standard.Gap();
			listing_Standard.Gap();
			listing_Standard.Label(StringHTMLHelper.GetBoldString("storageBuildings".Translate()));
			listing_Standard.GapLine();
			
			listing_Standard.CheckboxLabeled("shouldShareLimitSettingsWhenLinkedLabel".Translate(), ref ThingLimiterModSettings.bShouldShareLimitSettingsWhenLinked, "shouldShareLimitSettingsWhenLinkedTooltip".Translate());
			if (ThingLimiterModSettings.bShouldShareLimitSettingsWhenLinked)
			{
				listing_Standard.CheckboxLabeled("shouldKeepLimitSettingsWhenUnlinked".Translate(), ref ThingLimiterModSettings.bShouldKeepLimitSettingsWhenUnlinked, "shouldKeepLimitSettingsWhenUnlinkedTooltip".Translate());
			}
			else
			{
				ThingLimiterModSettings.bShouldKeepLimitSettingsWhenUnlinked = false;
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