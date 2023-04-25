using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
			//TO DO: IMPLEMENT
			byte[] paket = new byte[12];//sabrali bajtove, imamo niz od 12 elemenata 
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, paket, 0, 2);//od koje kreces pozicije u izvornom nizu, gdje prekopiravas, na koju poziciju i velicina toga sto prekopiras
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, paket, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, paket, 4, 2);
			paket[6] = CommandParameters.UnitId;//samo zalijepimo UnitId na svoju poziciju
			paket[7] = CommandParameters.FunctionCode;//samo zalijepimo FunctionCode na svoju poziciju
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusWriteCommandParameters)CommandParameters).OutputAddress)), 0, paket, 8, 2);//jos jedno kastovanje jer su ova polja u naslijedjenoj klasi dodata
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusWriteCommandParameters)CommandParameters).Value)), 0, paket, 10, 2);

			return paket;
		}

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
			//TO DO: IMPLEMENT
			var ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
			var address = BitConverter.ToUInt16(response, 8);
			var value = BitConverter.ToUInt16(response, 10);

			address = (ushort)IPAddress.NetworkToHostOrder((short)address);
			value = (ushort)IPAddress.HostToNetworkOrder((short)value);

			ret.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, address), value);

			return ret;
		}
    }
}