namespace ThingLimiterMod.Source.Helpers;

public enum HTMLColors
{
	Cyan,
	Green,
	Red,
	Coral
}
public static class StringHTMLHelper
{

	public static string GetColorCodeByEnum(HTMLColors color)
	{
		switch (color)
		{
			case HTMLColors.Cyan:
				return "#00D4FF";
			case HTMLColors.Green:
				return "#6aa84f";
			case HTMLColors.Red:
				return "#f44336";
			case HTMLColors.Coral:
				return "#f46049";
			default:
				return "#ffff00";
		}
	}
	
	public static string GetColorByCurrentNum(int CurrentNum, int MaxNum)
	{
		if (CurrentNum <= MaxNum)
		{
			return GetColorCodeByEnum(HTMLColors.Green); // Green
		}
		else
		{
			return GetColorCodeByEnum(HTMLColors.Red);
		}
	}
	public static string GetBoldString(string InputString)
	{
		return string.Format("<b>{0}</b>", InputString);
	}
	
	public static string GetColoredString(string InputString, string Color, bool bBolded = false)
	{
		return string.Format("<color={0}>{1}</color>", Color, bBolded ? GetBoldString(InputString) : InputString);
	}
	
	public static string GetColoredString(string InputString, HTMLColors Color, bool bBolded = false)
	{
		return string.Format("<color={0}>{1}</color>", GetColorCodeByEnum(Color), bBolded ? GetBoldString(InputString) : InputString);
	}
}