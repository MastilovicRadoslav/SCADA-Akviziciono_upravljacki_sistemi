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
    public class ReadInputRegistersFunction : ModbusFunction //Citanje analognog ulaza
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

		//(short)CommandParameters.TransactionId) - kastovanje u odgovarajucu velicinu polja za svaki signal
		//IPAddress.HostToNetworkOrder() - host --> network konverzija, prima vrijednosti int, short ... pa smo zato ovo gore prvo kastovali u short jer sa ushort moze doci do problema pri okretanju vrijednosti(bajtova)
		//BitConverter.GetBytes() - rukuje sa podacima short, int ... i pretvara ih u niz bajtova, funkcija i ugradjena metoda u funkciju
		//Buffer.BlockCopy(niz koji kopiramo, odakle u tom izvornom nizu krecemo sa kopiranjem, destinacija gdje kopiramo niz bajtova, na koju poziciju u nizu, kolika je velicina podatka koji kopiramo u bajtovima) - smjestanje bajtova u odgovarajuci niz koji smo napravili


		/// <inheritdoc />
		public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)//pristupamo odredjenom parametru u poruci i ocitavamo vrijednost koja je poslata
		{
			Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandeException(response[8]);
            }
            else
            {
                ushort adresa = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
                ushort value;
                for (int i = 0; i < response[8]; i = i + 2)
                {
                    value = BitConverter.ToUInt16(response, (i + 9));
                    value = (ushort)IPAddress.NetworkToHostOrder((short)value);
                    dictionary.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, adresa), value);
                    adresa++;
                }
            }

            return dictionary;
        }
    }
}

//Typle - grupise dva tipa podatka (PointType, ushort)
//PointType - digitalni/analogni ulaz/izlaz
//ushort - adresa signala
//ushort - vrijednost koju smo ocitali sa simulatora