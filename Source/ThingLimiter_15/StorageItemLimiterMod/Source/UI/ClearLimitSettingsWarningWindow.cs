using System;
using UnityEngine;
using Verse;

namespace StorageItemLimiterMod.Source.UI;

public class Dialog_ClearLimitSettingsWarning : Window
{
	private string title;
	private string confirm;
	private string tip;
	
	private Action onConfirm;
	private static readonly Vector2 ButtonSize = new Vector2(120f, 32f);

	public Dialog_ClearLimitSettingsWarning(string title, Action onConfirm)
		: this(title, (string) "Confirm".Translate(), string.Empty, onConfirm)
	{
	}

	public Dialog_ClearLimitSettingsWarning(string title, string confirm, Action onConfirm)
		: base()
	{
		this.title = title;
		this.confirm = confirm;
		this.onConfirm = onConfirm;
		this.forcePause = true;
		this.closeOnAccept = false;
		this.closeOnClickedOutside = true;
		this.absorbInputAroundWindow = true;
	}
	
	public Dialog_ClearLimitSettingsWarning(string title, string confirm, string tip, Action onConfirm)
		: base()
	{
		this.title = title;
		this.confirm = confirm;
		this.tip = tip;
		this.onConfirm = onConfirm;
		this.forcePause = true;
		this.closeOnAccept = false;
		this.closeOnClickedOutside = true;
		this.absorbInputAroundWindow = true;
	}

	public override Vector2 InitialSize => new Vector2(280f, 150f);

	public override void DoWindowContents(Rect inRect)
	{
		Text.Font = GameFont.Small;
		bool flag = false;
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
		{
			flag = true;
			Event.current.Use();
		}
		Rect rect1 = inRect with
		{
			width = (float) ((double) inRect.width / 2.0 - 5.0),
			yMin = (float) ((double) inRect.yMax - (double) Dialog_ClearLimitSettingsWarning.ButtonSize.y - 10.0)
		};
		Rect rect2 = inRect with
		{
			xMin = rect1.xMax + 10f,
			yMin = (float) ((double) inRect.yMax - (double) Dialog_ClearLimitSettingsWarning.ButtonSize.y - 10.0)
		};
		Rect rect3 = inRect;
		rect3.y += 4f;
		rect3.yMax = rect2.y - 10f;

		if (!tip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect2, tip);
		}
		
		using (new TextBlock(TextAnchor.UpperCenter))
			Widgets.Label(rect3, this.title);
		if (Widgets.ButtonText(rect1, (string) "Cancel".Translate()))
			Find.WindowStack.TryRemove((Window) this);
		if (!(Widgets.ButtonText(rect2, this.confirm) | flag))
			return;
		Action onConfirm = this.onConfirm;
		if (onConfirm != null)
			onConfirm();
		Find.WindowStack.TryRemove((Window) this);
	}
}