using System.ComponentModel.DataAnnotations;
using System;

namespace MainServer.Attributes
{

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public class NotNullText : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is String str)
			{
				if (str == "null")
					return false;
			}
			return true;
		}
	}
}
