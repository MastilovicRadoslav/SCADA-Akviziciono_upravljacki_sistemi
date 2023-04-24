using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
			//TO DO: IMPLEMENT
			//napraviti niz od 12 bajta
			//CommandParameters.
			//ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;    <-- ovo mogu koristit pa dolje ne kastovat u naslijedjenu klasu

			byte[] paket = new byte[12];//sabrali bajtove, imamo niz od 12 elemenata 
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, paket, 0, 2);//od koje kreces pozicije u izvornom nizu, gdje prekopiravas, na koju poziciju i velicina toga sto prekopiras
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, paket, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, paket, 4, 2);
			paket[6] = CommandParameters.UnitId;//samo zalijepimo UnitId na svoju poziciju
			paket[7] = CommandParameters.FunctionCode;//samo zalijepimo FunctionCode na svoju poziciju
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).StartAddress)), 0, paket, 8, 2);//jos jedno kastovanje jer su ova polja u naslijedjenoj klasi dodata
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).Quantity)), 0, paket, 10, 2);

			return paket;
			//return niz;
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
				for (int i = 0; i < response[8]; i++)
				{
					byte tempByte = response[9 + i];
					for (int j = 0; j < 8; j++)
					{
						value = (ushort)(tempByte & mask);
						tempByte >>= 1;
						ret.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_INPUT, adresa), value);
						cnt++;
						adresa++;
						if (cnt == ((ModbusReadCommandParameters)CommandParameters).Quantity)
						{
							break;
						}

					}
				}
			}

			return ret;


			/*
			ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;
			Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();

			ushort quantity = response[8];

			ushort value;
			Console.WriteLine(quantity);
			for (int i = 0; i < quantity; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					value = (ushort)(response[9 + i] & (byte)0x1);
					response[9 + i] /= 2;

					if (mdmReadCommParams.Quantity < (j + i * 8)) { break; }


					dic.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)(mdmReadCommParams.StartAddress + (j + i * 8))), value);
				}

			}
			return dic;

			*/









			//napraviti rijecnik
			//iterirati kroz data deo i kroz bite 
			//return rijecnik;

		}
    }
}