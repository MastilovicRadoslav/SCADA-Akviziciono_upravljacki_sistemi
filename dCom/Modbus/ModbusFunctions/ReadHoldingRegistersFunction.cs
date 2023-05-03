using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction //Citanje analognog izlaza
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
			ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;

			byte[] mdbRequest = new byte[12];


			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, mdbRequest, 0, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, mdbRequest, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, mdbRequest, 4, 2);

			mdbRequest[6] = CommandParameters.UnitId;
			mdbRequest[7] = CommandParameters.FunctionCode;

			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.StartAddress)), 0, mdbRequest, 8, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.Quantity)), 0, mdbRequest, 10, 2);

			return mdbRequest;
		}

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)//pristupamo odredjenim bajtovima i ocitavamo vrijednosti
        {
			Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] == CommandParameters.FunctionCode + 0x80)//da li je doslo do greske prilikom komunikacije izmedju simulatora i SCADA stanice, poruka onda nije validna za nas
			{
                HandeException(response[8]);
            }
            else
            {
                ushort address = ((ModbusReadCommandParameters)CommandParameters).StartAddress;//adresa sa koje citamo
                ushort value;//vrijednost
                for (int i = 0; i < response[8]; i += 2)//u Byte imamo podatak koliko Byte imamo ukupno, ushort - 2 bajta
                {
                    value = BitConverter.ToUInt16(response, (i + 9));//pristupamo Date dijelu
                    value = (ushort)IPAddress.NetworkToHostOrder((short)value);//izvlacimo iz njega jedan ushort, sa hostovanjem
					dictionary.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, address), value);//ubacujemo u Dictionary, koji tip registra smo azurirali, koja je njegova adresa i koje je njegova nova vrijednost
                    address++;//sledeca adresa
                }
            }

            return dictionary;
        }
    }
}