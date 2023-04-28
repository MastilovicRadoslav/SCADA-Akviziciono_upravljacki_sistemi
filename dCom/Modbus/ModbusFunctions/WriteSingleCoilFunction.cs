using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction //Digitalni izlaz
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
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
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusWriteCommandParameters)CommandParameters).OutputAddress)), 0, paket, 8, 2);//samo ovo drugacije
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusWriteCommandParameters)CommandParameters).Value)), 0, paket, 10, 2);//samo ovo drugacije

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
				ushort adresa = BitConverter.ToUInt16(response, (8));
				adresa = (ushort)IPAddress.NetworkToHostOrder((short)adresa);
				ushort value = BitConverter.ToUInt16(response, (10));
				value = (ushort)IPAddress.NetworkToHostOrder((short)value);
				ret.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, adresa), value);
			}

			return ret;

		}
    }
}