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
    public class ReadDiscreteInputsFunction : ModbusFunction//Digitalni ulaz
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
			ModbusReadCommandParameters mdmReadCommParams = this.CommandParameters as ModbusReadCommandParameters;//u neku promjenljivu smjestamo kastovanu klasu ModbusReadCommandParameters koja nasledjuje baznu klasu ModbusCommandParameters da bi pristupili vrijednostima u njoj

			byte[] mdbRequest = new byte[12];//sabrali bajtove, imamo niz od 12 elemenata


			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, mdbRequest, 0, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, mdbRequest, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, mdbRequest, 4, 2);

			mdbRequest[6] = CommandParameters.UnitId;//samo zalijepimo UnitId na svoju poziciju, ne treba ga kastovat jer je velicine jedan bajt, ne treba ga pretvarati u mrezni oblik jer jedan bajt se ne pretvara kad se salje kroz mrezu
			mdbRequest[7] = CommandParameters.FunctionCode;//samo zalijepimo FunctionCode na svoju poziciju

			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.StartAddress)), 0, mdbRequest, 8, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)mdmReadCommParams.Quantity)), 0, mdbRequest, 10, 2);

			return mdbRequest;
		}

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
			
			Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();
			
			if (response[7] == CommandParameters.FunctionCode + 0x80)
			{
				HandeException(response[8]);
			}
			else
			{
				int cnt = 0;
				ushort address = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
				ushort value;
				byte mask = 1;
				for (int i = 0; i < response[8]; i++)//izvlaci bajt po bajt
				{
					byte cntByte = response[9 + i];//prvi bajt, drugi bajt ..
					for (int j = 0; j < 8; j++)//od 0 do 8 i radi sa tim izvucenim bajtom, maskom i sa siftovanjem --> obrada svakog bajta, broj registara i za svaki od njih vrijednost koju ima --> 0 ili 1
					{
						value = (ushort)(cntByte & mask);//uzimamo vrijednost kada uradimo logicko i sa maskom koja je 1
						cntByte >>= 1;//siftujemo nas bajt za jedno mjesto u desno i uzimamo sledecu vrijednost bajta koju cemo opet sa logicko i uraditi sa maskom
						dictionary.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_INPUT, address), value);//ubacuj u Dictionary
						cnt++;//da bi znali da prekinemo izvrsavanje prije 
						address++;//ako je prva adresa npr 3000 onda je sledeca 3001 ...
						if (cnt == ((ModbusReadCommandParameters)CommandParameters).Quantity)//prekidamo  ako broj obradjenih signala bude jednak broju ukupnih signala	koji su proslijedjeni
						{
							break;//prekini ako nema vise signala i idi na drugi bajt ako ga ima
						}
					}
				}
			}

			return dictionary;

			//1 bit - jedna vrijednost
		}
	}
}