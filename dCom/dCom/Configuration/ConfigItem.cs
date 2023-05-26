using Common;
using System;
using System.Collections.Generic;

namespace dCom.Configuration
{
    internal class ConfigItem : IConfigItem
	{
		#region Fields

		private PointType registryType;
		private ushort numberOfRegisters;
		private ushort startAddress;
		private ushort decimalSeparatorPlace;
		private ushort minValue;
		private ushort maxValue;
		private ushort defaultValue;
		private string processingType;
		private string description;
		//ovo ispod moramo preuzeti sami
		private int acquisitionInterval;  //akvizicija interval
		private double scalingFactor;	//skaliranje
		private double deviation;		//odstupanje
		private double egu_max;			//EGU min, minimalna vr u inzenjerskim jedinicama
		private double egu_min;			//EGU max  maximalna vr u inzenjerskim jedinicama
		private ushort abnormalValue;	//abnormalno stanje
		private double highLimit;	    //HIGH limir - gornja granica
		private double lowLimit;		//LOW limit - donja granica
        private int secondsPassedSinceLastPoll;	 //brojac onaj za akviziciju

		#endregion Fields

		#region Properties

		public PointType RegistryType
		{
			get
			{
				return registryType;
			}

			set
			{
				registryType = value;
			}
		}

		public ushort NumberOfRegisters
		{
			get
			{
				return numberOfRegisters;
			}

			set
			{
				numberOfRegisters = value;
			}
		}

		public ushort StartAddress
		{
			get
			{
				return startAddress;
			}

			set
			{
				startAddress = value;
			}
		}

		public ushort DecimalSeparatorPlace
		{
			get
			{
				return decimalSeparatorPlace;
			}

			set
			{
				decimalSeparatorPlace = value;
			}
		}

		public ushort MinValue
		{
			get
			{
				return minValue;
			}

			set
			{
				minValue = value;
			}
		}

		public ushort MaxValue
		{
			get
			{
				return maxValue;
			}

			set
			{
				maxValue = value;
			}
		}

		public ushort DefaultValue
		{
			get
			{
				return defaultValue;
			}

			set
			{
				defaultValue = value;
			}
		}

		public string ProcessingType
		{
			get
			{
				return processingType;
			}

			set
			{
				processingType = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
			}
		}

		public int AcquisitionInterval
		{
			get
			{
				return acquisitionInterval;
			}

			set
			{
				acquisitionInterval = value;
			}
		}

		public double ScaleFactor
		{
			get
			{
				return scalingFactor;
			}
			set
			{
				scalingFactor = value;
			} 
		}

		public double Deviation
		{
			get
			{
				return deviation;
			}

			set
			{
				deviation = value;
			}
		}

		public double EGU_Max
		{
			get
			{
				return egu_max;
			}

			set
			{
				egu_max = value;
			}
		}

		public double EGU_Min
		{
			get
			{
				return egu_min;
			}

			set
			{
				egu_min = value;
			}
		}

		public ushort AbnormalValue
		{
			get
			{
				return abnormalValue;
			}

			set
			{
				abnormalValue = value;
			}
		}

		public double HighLimit
		{
			get
			{
				return highLimit;
			}

			set
			{
				highLimit = value;
			}
		}

		public double LowLimit
		{
			get
			{
				return lowLimit;
			}

			set
			{
				lowLimit = value;
			}
		}

        public int SecondsPassedSinceLastPoll //koliko je sekundi proslo od prehodne akvizicije signala
        {
            get
            {
                return secondsPassedSinceLastPoll;
            }

            set
            {
                secondsPassedSinceLastPoll = value;
            }
        }

        #endregion Properties

        public ConfigItem(List<string> configurationParameters)
		{
			RegistryType = GetRegistryType(configurationParameters[0]);
			int temp;
			double doubleTemp;
			Int32.TryParse(configurationParameters[1], out temp);
			NumberOfRegisters = (ushort)temp;
			Int32.TryParse(configurationParameters[2], out temp);
			StartAddress = (ushort)temp;
			Int32.TryParse(configurationParameters[3], out temp);
			DecimalSeparatorPlace = (ushort)temp;
			Int32.TryParse(configurationParameters[4], out temp);
			MinValue = (ushort)temp;
			Int32.TryParse(configurationParameters[5], out temp);
			MaxValue = (ushort)temp;
			Int32.TryParse(configurationParameters[6], out temp);
			DefaultValue = (ushort)temp;
			ProcessingType = configurationParameters[7];
			Description = configurationParameters[8].TrimStart('@');
            if (configurationParameters[9].Equals("#"))		//taraba
            {
                AcquisitionInterval = 1;	//interval akvizicije
            }
            else
            {
                Int32.TryParse(configurationParameters[9], out temp);
                AcquisitionInterval = temp;
            }
			//PARSIRAMO 4 VRIJEDNOSTI STO SMO UNIJELI, ONE SU VEZANE ZA KONVERZIJU IZ SIROVIH VRIJEDNOSTI U INZENJERSKE VRIJEDNOSTI I OBRNUTO
			if (configurationParameters[10].Equals("#"))	//taraba
			{
				ScaleFactor = 1;
			}
			else
			{
				Double.TryParse(configurationParameters[10], out doubleTemp);
				ScaleFactor = doubleTemp;
			}

			if (configurationParameters[11].Equals("#"))
			{
				Deviation = 0;
			}
			else
			{
				Double.TryParse(configurationParameters[11], out doubleTemp);
				Deviation = doubleTemp;
			}
			if (configurationParameters[12].Equals("#"))
			{
				EGU_Min = 0;
			}
			else
			{
				Double.TryParse(configurationParameters[12], out doubleTemp);
				EGU_Min = doubleTemp;
			}

			if (configurationParameters[13].Equals("#"))
			{
				EGU_Max = 1;
			}
			else
			{
				Double.TryParse(configurationParameters[13], out doubleTemp);
				EGU_Max = doubleTemp;
			}
			if (configurationParameters[14].Equals("#"))
			{
				AbnormalValue = 0; // zbog alarma 
			}
			else
			{
				Int32.TryParse(configurationParameters[14], out temp);
				AbnormalValue = (ushort)temp;
			}

			if (configurationParameters[15].Equals("#"))
			{
				LowLimit = 0;
			}
			else
			{
				Double.TryParse(configurationParameters[15], out doubleTemp);
				LowLimit = doubleTemp;
			}
			if (configurationParameters[16].Equals("#"))
			{
				HighLimit = 0;
			}
			else
			{
				Double.TryParse(configurationParameters[16], out doubleTemp);
				HighLimit = doubleTemp;
			}
		}

		private PointType GetRegistryType(string registryTypeName)
		{
			PointType registryType;
			switch (registryTypeName)
			{
				case "DO_REG":
					registryType = PointType.DIGITAL_OUTPUT;
					break;

				case "DI_REG":
					registryType = PointType.DIGITAL_INPUT;
					break;

				case "IN_REG":
					registryType = PointType.ANALOG_INPUT;
					break;

				case "HR_INT":
					registryType = PointType.ANALOG_OUTPUT;
					break;

				default:
					registryType = PointType.HR_LONG;
					break;
			}
			return registryType;
		}
	}
}