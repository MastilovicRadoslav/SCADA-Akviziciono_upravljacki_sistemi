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
		public AlarmType GetAlarmForAnalogPoint(double eguValue, IConfigItem configItem) //ANALOGNE VRIJEDNOSTI
		{	
			if (Out_of_range(eguValue, configItem))
				if (Check_alarm_limit(eguValue, configItem) == 1)
					return AlarmType.HIGH_ALARM; 	//izvan smo opsega gornje granice
				else if (Check_alarm_limit(eguValue, configItem) == -1)
					return AlarmType.LOW_ALARM;		//izvan smo opsega donje granice
				else
					return AlarmType.NO_ALARM;	    //nemamo alarma nikakvog
			else
				return AlarmType.REASONABILITY_FAILURE;  //prvi tip alarma oznacava da smo van EGU min i EGU max
		}

		private static bool Out_of_range(double eguvalue, IConfigItem configitem)	//EGU - limit
		{
			if (eguvalue < configitem.EGU_Min || eguvalue > configitem.EGU_Max)
				return false;
			else
				return true;

		}

		private static int Check_alarm_limit(double eguvalue, IConfigItem configitem)  //	HIGH/LOW - limit
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
		public AlarmType GetAlarmForDigitalPoint(ushort state, IConfigItem configItem)	 //DIGITALNE VRIJEDNOSTI
		{	//npr. senzor pritiska, opterecenja ... 
			if (state == configItem.AbnormalValue)	//nominalna vr. je 0 a abnormalna vr. je 1
				return AlarmType.ABNORMAL_VALUE;
			else
				return AlarmType.NO_ALARM;
		}
	}
}
