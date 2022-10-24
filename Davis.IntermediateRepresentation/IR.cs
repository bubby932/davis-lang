namespace Davis.IR {
	public enum Instruction : byte {
		OpPushConst,
		IntrinsicAdd,
		SimulatorIntrinsicPrint,
		SimulatorIntrinsicPrintNoPop
	}
}