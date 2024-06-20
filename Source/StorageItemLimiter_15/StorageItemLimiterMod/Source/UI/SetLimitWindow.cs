using System.Linq;
using RimWorld;
using StorageItemLimiterMod.Source.Core.Mod;
using StorageItemLimiterMod.Source.Helpers;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.UI;

public class SetLimitWindow : Window
{
	protected int currentValue;

	private bool focusedRenameField;

	private int startAcceptingInputAtFrame;

	protected readonly string thingType;

	private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;
	
	private string textFieldText;
	protected virtual int MaxNameLength => 28;
	
	private string infinity = "\u221E";
	public override Vector2 InitialSize => new Vector2(280f, 175f);

	private bool bFirstTime = true;
	public SetLimitWindow(string InThingType)
	{
		thingType = InThingType;
		currentValue = int.MaxValue;
		doCloseX = true;
		forcePause = true;
		closeOnAccept = false;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
		
		if (Find.Selector.SelectedZone is Zone_Stockpile stockpileZone)
		{
			currentValue = LimitSystemHelper.GetThingLimit(stockpileZone, thingType);
		}
		else if(Find.Selector.FirstSelectedObject is Building_Storage selectedStorage)
		{
			currentValue = LimitSystemHelper.GetThingLimit(selectedStorage, thingType);
		}
	}
	public void WasOpenedByHotkey()
	{
		startAcceptingInputAtFrame = Time.frameCount + 1;
	}

	protected AcceptanceReport ValueIsValid(int value)
	{
		return value >= 0;
	}
	
	public override void DoWindowContents(Rect inRect)
	{
		Text.Font = GameFont.Small;
		bool flag = false;
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
		{
			flag = true;
			Event.current.Use();
		}

		string ControlName = "SetLimitControlName";
		
		Rect rect = new Rect(inRect);
		Text.Font = GameFont.Small;
		rect.height = Text.LineHeight + 10f;

		string SetLimitThingString = (string)("setLimit".Translate()) + StringHTMLHelper.GetColoredString(thingType, HTMLColors.Cyan, true);
		
		Widgets.Label(rect, SetLimitThingString);
		Rect stackInfoRect = new Rect(rect.xMin, rect.yMin + rect.height - 5, rect.width, rect.height - 5.0f);
		
		int CurrentNumber = 0;
		int MaxNumber = int.MaxValue;

		if (Find.Selector.SelectedZone is Zone_Stockpile selectedZone)
		{
			CurrentNumber = LimitSystemHelper.GetCurrentThingCount(selectedZone, thingType);
			MaxNumber = LimitSystemHelper.GetThingLimit(selectedZone, thingType);
		}
		else if (Find.Selector.FirstSelectedObject is Building_Storage selectedStorage)
		{
			if (StorageItemLimiterModSettings.bShouldCountThingsAsOneLinkedStorage)
			{
				CurrentNumber = LimitSystemHelper.GetCurrentThingCountForAllLinkedStorages(selectedStorage, thingType);
			}
			else
			{
				CurrentNumber = LimitSystemHelper.GetCurrentThingCount(selectedStorage, thingType);
			}
			
			MaxNumber = LimitSystemHelper.GetThingLimit(selectedStorage, thingType);
		}

		string CurrentNumColor = StringHTMLHelper.GetColorByCurrentNum(CurrentNumber, MaxNumber);
		
		string CurrentCountInfoString = (string)("currentLimitPrefix".Translate()) + "\t" + StringHTMLHelper.GetColoredString(CurrentNumber.ToString(), CurrentNumColor) + " / " + StringHTMLHelper.GetBoldString(MaxNumber == int.MaxValue ? infinity : MaxNumber.ToString());
		Widgets.Label(stackInfoRect, CurrentCountInfoString);
		
		Text.Font = GameFont.Small;
		GUI.SetNextControlName(ControlName);

		if (textFieldText.NullOrEmpty() && bFirstTime)
		{
			textFieldText = currentValue == int.MaxValue ? infinity : currentValue.ToString();
		}
		
		textFieldText = Widgets.TextField(new Rect(0.0f, rect.height + stackInfoRect.height, inRect.width, 30f), textFieldText);
		
		bFirstTime = false;
		
		if (!focusedRenameField)
		{
			Verse.UI.FocusControl(ControlName, this);
			focusedRenameField = true;
		}
		
		bool bTextFieldOnlyContainsDigits = textFieldText.All(c => char.IsDigit(c) || c == '-' || c == infinity[0]);


		Rect ButtonRect = new Rect(15f, stackInfoRect.height + inRect.height - 35.0f - 10.0f - 15.0f,inRect.width - 15.0f - 15.0f, 30.0f);

		string tipText = (string)("setLimitWindowTip".Translate());
		if (!tipText.NullOrEmpty())
		{
			TooltipHandler.TipRegion(ButtonRect, (TipSignal)tipText);
		}
		
		if (!(Widgets.ButtonText(ButtonRect, "OK") || flag))
		{
			return;
		}

		if (!bTextFieldOnlyContainsDigits)
		{
			Messages.Message((string) (string)("setLimitWindowInvalidText".Translate()), MessageTypeDefOf.RejectInput, false);
			return;
		}
		
		if (!textFieldText.NullOrEmpty() && (textFieldText[0] == '-' || textFieldText[0] == infinity[0]))
		{
			currentValue = int.MaxValue;
		}
		else if (AcceptsInput && textFieldText.Length < MaxNameLength)
		{
			currentValue = int.Parse(textFieldText);
		}
		else if (!AcceptsInput)
		{
			((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();
		}

		int SetNum = ValueIsValid(currentValue) ? currentValue : int.MaxValue;
		
		if (Find.Selector.SelectedZone is Zone_Stockpile ZoneAsStockpile)
		{
			LimitSystemHelper.UpdateThingLimit(ZoneAsStockpile, thingType, SetNum);
		}
		else if (Find.Selector.FirstSelectedObject is Building_Storage SelectedObjectAsStorage)
		{
			LimitSystemHelper.UpdateThingLimit(SelectedObjectAsStorage, thingType, SetNum);

			if (StorageItemLimiterModSettings.bShouldShareLimitSettingsWhenLinked)
			{
				if (SelectedObjectAsStorage?.storageGroup?.members != null)
				{
					foreach (object member in SelectedObjectAsStorage?.storageGroup?.members)
					{
						StorageSettingsHelper.PasteFromObject(SelectedObjectAsStorage, member as Building_Storage, true);
					}
				}
			}
		}

		Find.WindowStack.TryRemove(this);
	}
}