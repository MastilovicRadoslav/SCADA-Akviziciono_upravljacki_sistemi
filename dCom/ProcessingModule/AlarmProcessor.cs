using Common;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for alarm processing.
    /// </summary>
    public class AlarmProcessor
	{
        /// <summary>
        /// Processes the alarm for analog point.
        /// </summary>
        /// <param name="eguValue">The EGU value of the point.</param>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForAnalogPoint(double eguValue, IConfigItem configItem)
		{
			if (Out_of_range(eguValue, configItem))
				if (Check_alarm_limit(eguValue, configItem) == 1)
					return AlarmType.HIGH_ALARM;
				else if (Check_alarm_limit(eguValue, configItem) == -1)
					return AlarmType.LOW_ALARM;
				else
					return AlarmType.NO_ALARM;
			else
				return AlarmType.REASONABILITY_FAILURE;
		}

		private static bool Out_of_range(double eguvalue, IConfigItem configitem)
		{
			if (eguvalue < configitem.EGU_Min || eguvalue > configitem.EGU_Max)
				return false;
			else
				return true;

		}

		private static int Check_alarm_limit(double eguvalue, IConfigItem configitem)
		{

			if (eguvalue <= configitem.LowLimit)
				return -1;
			else if (eguvalue == configitem.HighLimit)
				return 1;
			else
				return 0;

		}

		/// <summary>
		/// Processes the alarm for digital point.
		/// </summary>
		/// <param name="state">The digital point state</param>
		/// <param name="configItem">The configuration item.</param>
		/// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForDigitalPoint(ushort state, IConfigItem configItem)
		{
			if (state == configItem.AbnormalValue)
				return AlarmType.ABNORMAL_VALUE;
			else
				return AlarmType.NO_ALARM;
		}
	}
}
