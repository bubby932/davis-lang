namespace Davis.IR
{
	public enum Instruction : byte
	{
		OpPushConst,
		IntrinsicAdd,
		Assembly,
		SimulatorIntrinsicPrint,
		SimulatorIntrinsicPrintNoPop
	}
}