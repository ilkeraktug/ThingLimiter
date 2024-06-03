using HarmonyLib;
using ThingLimiterMod.Source.Core.Mod;
using ThingLimiterMod.Source.UI;
using UnityEngine;
using Verse;

namespace ThingLimiterMod.Source.Patches.UI;

[HarmonyPatch(typeof(Listing_Tree), "LabelLeft")]
public class LeftLabel_Patched
{
  protected static float XAtIndentLevel_ILKER(int indentLevel, float nestIndentWidth) => (float) indentLevel * nestIndentWidth;

  public static bool IsThingRow(string label, string tip)
  {
    if (label.NullOrEmpty())
    {
      return false;
    }

    bool bIsFirstCharacterAsterisk = label[0] == '*';

    return !bIsFirstCharacterAsterisk && !tip.NullOrEmpty();
  }

  public static bool Prefix(Listing_Tree __instance, float ___curY,
    string label,
    string tipText,
    int indentLevel,
    float widthOffset,
    Color? textColor,
    float leftOffset)
  {
    float LabelWidth = __instance.ColumnWidth - 26f;

    Rect rect = new Rect(0.0f, ___curY, __instance.ColumnWidth, __instance.lineHeight);
    rect.xMin = XAtIndentLevel_ILKER(indentLevel, __instance.nestIndentWidth) + 18f + leftOffset;
    
    //Widgets.DrawHighlightIfMouseover(rect);
    
    Rect limitWindowRect = rect;
    
    if (!tipText.NullOrEmpty())
    {
      if (Mouse.IsOver(rect))
        GUI.DrawTexture(rect, (Texture)TexUI.HighlightTex);
      TooltipHandler.TipRegion(rect, (TipSignal)tipText);
    }
    
    Text.Anchor = TextAnchor.MiddleLeft;
    GUI.color = textColor ?? Color.white;
    rect.width = LabelWidth - rect.xMin + widthOffset;
    rect.yMax += 5f;
    rect.yMin -= 5f;
    Widgets.Label(rect, label.Truncate(rect.width));
    Text.Anchor = TextAnchor.UpperLeft;
    GUI.color = Color.white;

    int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);
    
    if (Input.GetMouseButtonDown(ThingLimiterModSettings.OpenMouseButton) && Mouse.IsOver(limitWindowRect) && IsThingRow(label, tipText))
    {
      Find.WindowStack.Add(new SetLimitWindow(label));
    }

    return false;
  }
}