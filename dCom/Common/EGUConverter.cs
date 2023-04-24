using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class EGUConverter
	{
		public static double ConvertToEGU(double scalingFactor, double deviation, ushort rawValue)
		{
			return scalingFactor * rawValue + deviation;
		}

		public static ushort ConvertToRaw(double scalingFactor, double deviation, double eguValue)
		{
			return (ushort)(Math.Round(((eguValue - deviation) / scalingFactor)));
		}
	}
}
