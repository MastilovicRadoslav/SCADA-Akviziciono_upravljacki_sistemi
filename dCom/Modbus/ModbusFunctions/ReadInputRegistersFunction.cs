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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)//pristupamo odredjenom parametru u poruci i ocitavamo vrijednost koja je poslata
        {
            //TO DO: IMPLEMENT
            throw new NotImplementedException();
        }
    }
}

//Typle - grupise dva tipa podatka (PointType, ushort)
//PointType - digitalni/analogni ulaz/izlaz
//ushort - adresa signala
//ushort - vrijednost koju smo ocitali sa simulatora
