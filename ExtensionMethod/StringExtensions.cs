namespace Qrame.CoreFX.Data.ExtensionMethod
{
	public static partial class StringExtensions
	{
		public static bool ToBooleanOrDefault(this string @this, bool defaultValue)
		{
			return ToBooleanOrDefault((object)@this, defaultValue);
		}

		public static bool ToBooleanOrDefault(this object @this, bool defaultValue)
		{
			bool result = defaultValue;

			if (@this != null)
			{
				try
				{
					switch (@this.ToString().ToLower())
					{
						case "yes":
						case "true":
						case "y":
						case "1":
							result = true;
							break;

						case "no":
						case "false":
						case "n":
						case "0":
							result = false;
							break;

						default:
							result = bool.Parse(@this.ToString());
							break;
					}
				}
				catch
				{
				}
			}

			return result;
		}
	}
}
