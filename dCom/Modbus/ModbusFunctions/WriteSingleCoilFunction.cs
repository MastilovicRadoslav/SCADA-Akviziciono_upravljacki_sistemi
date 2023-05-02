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
    public class WriteSingleCoilFunction : ModbusFunction
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
			ModbusWriteCommandParameters mdmWriteCommParams = this.CommandParameters as ModbusWriteCommandParameters;//u neku promjenljivu smjestamo kastovanu klasu ModbusWriteCommandParameters koja nasledjuje baznu klasu ModbusCommandParameters da bi pristupili vrijednostima u njoj

			byte[] mdbRequest = new byte[12];//sabrali bajtove, imamo niz od 12 elemenata


			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, mdbRequest, 0, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, mdbRequest, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, mdbRequest, 4, 2);

			mdbRequest[6] = CommandParameters.UnitId;//samo zalijepimo UnitId na svoju poziciju, ne treba ga kastovat jer je velicine jedan bajt, ne treba ga pretvarati u mrezni oblik jer jedan bajt se ne pretvara kad se salje kroz mrezu
			mdbRequest[7] = CommandParameters.FunctionCode;//samo zalijepimo FunctionCode na svoju poziciju

			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmWriteCommParams.OutputAddress)), 0, mdbRequest, 8, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmWriteCommParams.Value)), 0, mdbRequest, 10, 2);

			return mdbRequest;
		}

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var ret = new Dictionary<Tuple<PointType, ushort>, ushort>();//povratna vrijednost

			if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandeException(response[8]);
            }
            else
            {
                ushort adresa = BitConverter.ToUInt16(response, (8));//adresa se nalazi od 8 do 10 bajta
				adresa = (ushort)IPAddress.NetworkToHostOrder((short)adresa);
                ushort value = BitConverter.ToUInt16(response, (10));//vrijednost se nalazi od 10 do 12 bajta
				value = (ushort)IPAddress.NetworkToHostOrder((short)value);
                ret.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, adresa), value);
            }

            return ret;
        }
    }
}