using Common;
using Modbus.FunctionParameters;
using Modbus.ModbusFunctions;

namespace Modbus
{
    /// <summary>
    /// Class containing logic for creating modbus functions.
    /// </summary>
    public class FunctionFactory
	{
        /// <summary>
        /// Creates a modbus fonction with the forwarded parameters.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        /// <returns></returns>
		public static IModbusFunction CreateModbusFunction(ModbusCommandParameters commandParameters)
		{
			switch ((ModbusFunctionCode)commandParameters.FunctionCode)
			{
				case ModbusFunctionCode.READ_COILS:
					return new ReadCoilsFunction(commandParameters);

				case ModbusFunctionCode.READ_DISCRETE_INPUTS:
					return new ReadDiscreteInputsFunction(commandParameters);

				case ModbusFunctionCode.READ_INPUT_REGISTERS:
					return new ReadInputRegistersFunction(commandParameters);

				case ModbusFunctionCode.READ_HOLDING_REGISTERS:
					return new ReadHoldingRegistersFunction(commandParameters);

				case ModbusFunctionCode.WRITE_SINGLE_COIL:
					return new WriteSingleCoilFunction(commandParameters);

				case ModbusFunctionCode.WRITE_SINGLE_REGISTER:
					return new WriteSingleRegisterFunction(commandParameters);

				default:
					return null;
			}
		}
	}
}