using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read discrete inputs functions/requests.
    /// </summary>
    public class ReadDiscreteInputsFunction : ModbusFunction //Digitalni ulaz
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDiscreteInputsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadDiscreteInputsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
			//TO DO: IMPLEMENT

			byte[] paket = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, paket, 0, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, paket, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, paket, 4, 2);
			paket[6] = CommandParameters.UnitId;
			paket[7] = CommandParameters.FunctionCode;
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).StartAddress)), 0, paket, 8, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).Quantity)), 0, paket, 10, 2);

			return paket;
		}

		/// <inheritdoc />
		public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
			//TO DO: IMPLEMENT


			var ret = new Dictionary<Tuple<PointType, ushort>, ushort>();

			if (response[7] == CommandParameters.FunctionCode + 0x80)
			{
				HandeException(response[8]);
			}
			else
			{
				int cnt = 0;
				ushort adresa = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
				ushort value;
				byte mask = 1;
				for (int i = 0; i < response[8]; i++)//izvlaci bajt po bajt
				{
					byte tempByte = response[9 + i];//prvi bajt, drugi bajt ..
					for (int j = 0; j < 8; j++)//od 0 do 8 i radi sa tim izvucenim bajtom, maskom i sa siftovanjem --> obrada svakog bajta
					{
						value = (ushort)(tempByte & mask);//uzimamo vrijednost kada uradimo logicko i sa maskom koja je 1
						tempByte >>= 1;//siftujemo nas bajt za jedno mjesto u desno i uzimamo sledecu vrijednost bajta koju cemo opet sa logicko i uraditi sa maskom
						ret.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_INPUT, adresa), value);//ubacuj u Dictionary
						cnt++;//da bi znali da prekinemo izvrsavanje prije 
						adresa++;//ako je prva adresa npr 3000 onda je sledeca 3001 ...
						if (cnt == ((ModbusReadCommandParameters)CommandParameters).Quantity)//prekidamo  ako broj obradjenih signala bude jednak broju ukupnih signala
						{
							break;//prekini ako nema vise signala
						}
					}
				}
			}

			return ret;

			 //1 bit - jedna vrijednost

		}
    }
}