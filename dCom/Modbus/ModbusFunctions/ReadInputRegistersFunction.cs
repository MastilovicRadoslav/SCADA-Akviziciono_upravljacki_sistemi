using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()//pristupamo svim propertijima klase preko commandParameters i pakujemo u poruku
        {
			//TO DO: IMPLEMENT
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
		}

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)//pristupamo odredjenom parametru u poruci i ocitavamo vrijednost koja je poslata
        {
			//TO DO: IMPLEMENT


			ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;
			Dictionary<Tuple<PointType, ushort>, ushort> dic = new Dictionary<Tuple<PointType, ushort>, ushort>();

			ushort quantity = response[8];

			ushort value;

			int p1 = 7, p2 = 8;
			for (int i = 0; i < quantity / 2; i++)
			{
				byte port1 = response[p1 += 2];
				byte port2 = response[p2 += 2];

				value = (ushort)(port2 + (port1 << 8));
				dic.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, (ushort)(mdmReadCommParams.StartAddress + i)), value);


			}
			return dic;
		}
	}
}

//Typle - grupise dva tipa podatka (PointType, ushort)
//PointType - digitalni/analogni ulaz/izlaz
//ushort - adresa signala
//ushort - vrijednost koju smo ocitali sa simulatora
