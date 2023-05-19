using System;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for engineering unit conversion.
    /// </summary>
    public class EGUConverter
	{
        /// <summary>
        /// Converts the point value from raw to EGU form.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor.</param>
        /// <param name="deviation">The deviation</param>
        /// <param name="rawValue">The raw value.</param>
        /// <returns>The value in engineering units.</returns>
		public double ConvertToEGU(double scalingFactor, double deviation, ushort rawValue)    //Konverzija kad vrijednost ide iz postorjenja n SCADA stanicu, tj. kada konvertujemo iz sirovih vrijednosti u inzenjerske vrijednosti
		{
			return rawValue * scalingFactor + deviation;    //skaliranje, odstupanje, sirova vrijednost
		}

        /// <summary>
        /// Converts the point value from EGU to raw form.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor.</param>
        /// <param name="deviation">The deviation.</param>
        /// <param name="eguValue">The EGU value.</param>
        /// <returns>The raw value.</returns>
		public ushort ConvertToRaw(double scalingFactor, double deviation, double eguValue)     //Konverzija kad vrijednost ide iz SCADA stanice u postrojenje, tj. kada iz inzenjerskih prebacujemo u sirove vrijednosti
        {
			return (ushort)((eguValue + deviation) / scalingFactor);  //skaliranje, odsutpanje, inzenjerska vrijednost
		}
	}
}
