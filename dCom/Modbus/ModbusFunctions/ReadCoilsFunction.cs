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
    public class ReadCoilsFunction : ModbusFunction//Digitalni izlaz
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
			ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;//u neku promjenljivu smjestamo kastovanu klasu ModbusReadCommandParameters koja nasledjuje baznu klasu ModbusCommandParameters da bi pristupili vrijednostima u njoj

			byte[] mdbRequest = new byte[12];//sabrali bajtove, imamo niz od 12 elemenata


			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, mdbRequest, 0, 2);//2 bajta za TransactionId
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, mdbRequest, 2, 2);//2 bajta za ProtocolId
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, mdbRequest, 4, 2);//2 bajta za Length

			mdbRequest[6] = CommandParameters.UnitId;//samo zalijepimo UnitId na svoju poziciju, ne treba ga kastovat jer je velicine jedan bajt, ne treba ga pretvarati u mrezni oblik jer jedan bajt se ne pretvara kad se salje kroz mrezu
			mdbRequest[7] = CommandParameters.FunctionCode;//samo zalijepimo FunctionCode na svoju poziciju

			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.StartAddress)), 0, mdbRequest, 8, 2);//2 bajta za StartAddress
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.Quantity)), 0, mdbRequest, 10, 2);//2 bajta za Quantity

			return mdbRequest;
		}
		//podatak uvijek stize u bajtima, sto znaci 8 bita

		/// <inheritdoc />
		public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
			Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] == CommandParameters.FunctionCode + 0x80)   //za Modbus prorokol
            {
                HandeException(response[8]);
            }
            else
            {
                int cnt = 0;
                ushort address = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
                ushort value;
                byte maska = 1;
                for (int i = 0; i < response[8]; i++)
                {
                    byte cntByte = response[9 + i];
                    for (int j = 0; j < 8; j++)
                    {
                        value = (ushort)(cntByte & maska);
                        cntByte >>= 1;
                        dictionary.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);
                        cnt++;
                        address++;
                        if (cnt == ((ModbusReadCommandParameters)CommandParameters).Quantity)
                        {
                            break;
                        }
                    }
                }
            }

            return dictionary;
        }
    }
}

//response[7] - pomocu njega ispitujemo da li je doslo do neke greske ili nije  - Function code
//response[8] - koliko ima bajtova pristiglih da prodjemo - Byte
//response[9] - data dio, pristigle vrijednosti - Data