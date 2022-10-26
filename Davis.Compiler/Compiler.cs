using System;
using System.Collections.Generic;
using Davis.IR;

namespace Davis.Compilation
{
	public class Compiler
	{
		private byte[] _Instructions;
		private object[] _Constants;
		private string _After;
		private int _Strings = 0;

		private Dictionary<int, object> _ConstantTable = new Dictionary<int, object>();

		public string Output = "";

		private void Write(string r) => Output += r + '\n';
		private void WriteAfter(string r) => _After += r + '\n';

		public Compiler(byte[] instructions, object[] constants)
		{
			_Instructions = instructions;
			_Constants = constants;

			int i = 0;
			for (int instr = 0; instr < instructions.Length; instr++)
			{
				if (instructions[instr] != (byte)Instruction.OpPushConst && instructions[instr] != (byte)Instruction.Assembly) continue;

				_ConstantTable[instr] = constants[i++];
			}
		}

		public void Compile()
		{
			Write("bits 16 ; 16-Bit Real Mode");

			for (int i = 0; i < _Instructions.Length; i++)
			{
				switch ((Instruction)_Instructions[i])
				{
					case Instruction.OpPushConst:
						{
							WriteConstant(_ConstantTable[i]);
							break;
						}
					case Instruction.IntrinsicAdd:
						{
							Write("pop rbx");
							Write("pop rax");
							Write("add rax, rax, rbx");
							Write("push rax");
							break;
						}
					case Instruction.Assembly:
						{
							Write("\n");
							Write((string)_ConstantTable[i]);
							break;
						}


					case Instruction.SimulatorIntrinsicPrintNoPop:
					case Instruction.SimulatorIntrinsicPrint: throw new CompilationError("Cannot use simulator instrinsics in a compiled context - did you #if your simulator code improperly?");

					default: throw new NotImplementedException("Invalid instruction for compiled context.");
				}
			}

			Write("\n; ------- Constants ------");
			Write(_After);
		}

		private void WriteConstant(object value)
		{
			switch (value)
			{
				case int integer:
					{
						Write($"push 0x{integer:X}");
						break;
					}
				case string str:
					{
						string label = $"string_{_Strings++}";
						Write($"push {label}");
						WriteAfter($"{label}: db '{str}', 0");
						break;
					}
				default: throw new CompilationError("Cannot insert a constant of type " + value.GetType().Name);
			}
		}
	}


	[Serializable]
	public class CompilationError : Exception
	{
		public CompilationError() { }
		public CompilationError(string message) : base(message) { }
		public CompilationError(string message, Exception inner) : base(message, inner) { }
		protected CompilationError(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}