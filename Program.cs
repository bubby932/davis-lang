using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Davis.SyntaxTree;
using Davis.IR;
using System.Diagnostics;
using Davis.Compilation;

namespace Davis
{
	class Program
	{
		static bool s_Verbose = false;

		/// <summary>
		/// Usage: <PATH> <Input Source Root> <Output Source Binary> <Additional (bits, etc)>
		/// </summary>
		static void Main(string[] args)
		{
			// IO
			string input_path;
			string output_path;

			// Preprocessing
			string final_source;

			// IR
			byte[] IR;
			object[] Constants;

			// x86
			string addtl_args;
			string final_asm;

			// If we don't have enough arguments, query the user.
			if(args.Length < 2) {
				Console.WriteLine("Enter input path.");
				input_path = Console.ReadLine();
				Console.WriteLine("Enter output path.");
				output_path = Console.ReadLine();
				Console.WriteLine("Enter additional argument flags.");
				addtl_args = Console.ReadLine();
			} else {
				input_path = args[0];
				output_path = args[1];
				if(args.Length > 2) {
					addtl_args = args[2];
				} else {
					addtl_args = "";
				}
			}

			s_Verbose = addtl_args.Contains("--verbose");

			string source = File.ReadAllText(input_path);

			if(addtl_args.Contains("--raw-ir")) {
				(IR, Constants) = ParseIR(source);
			} else {
				PreprocessSource(in source, out final_source);

				CompileSourceToRepresentation(in final_source, out IR, out Constants);
			}

			if(addtl_args.Contains("--local-sim")) {
				Simulate(in IR, in Constants);
				return;
			} else {
				CompileToNasmX86(in IR, in Constants, out final_asm);
			}

			File.WriteAllText(output_path, final_asm);
		}

		static void PreprocessSource(in string source, out string processed) {
			Parser.Parser.Parse(source);
			List<Token> t = Parser.Parser.Output;
			Console.WriteLine(string.Join(",\n\n", t));

			processed = null;
		}

		static void CompileSourceToRepresentation(in string source, out byte[] IR, out object[] constants) {
			throw new NotImplementedException();
		}

		static (byte[], object[]) ParseIR(in string IR) {
			string[] lines = IR.Split('\n');

			List<byte> ir = new List<byte>();
			List<object> constants = new List<object>();

			for (int i = 0; i < lines.Length; i++) {
				string literal = lines[i];
				switch (literal) {
					case "PushConst": {
						ir.Add((byte)Instruction.OpPushConst);
						break;
					}
					case "IntrinsicAdd": {
						ir.Add((byte)Instruction.IntrinsicAdd);
						break;
					}
					case "SimulatorIntrinsicPrint": {
						ir.Add((byte)Instruction.SimulatorIntrinsicPrint);
						break;
					}
					case "SimulatorIntrinsicPrintNoPop": {
						ir.Add((byte)Instruction.SimulatorIntrinsicPrintNoPop);
						break;
					}
					case "Assembly":
					{
						ir.Add((byte)Instruction.Assembly);
						string raw = "";
						i++;
						while (lines[i] != "__EndAssembly")
						{
							if (i == lines.Length) throw new IndexOutOfRangeException("No __EndAssembly call in IR file.");
							raw += lines[i++] + "\n";
						}

						constants.Add(raw);
						break;
					}
					default: {
						if(int.TryParse(literal, out int result)) {
							constants.Add(result);
						} else {
							constants.Add(literal);
						}
						break;
					}
				}
			}

			return (ir.ToArray(), constants.ToArray());
		}

		static void CompileToNasmX86(in byte[] IR, in object[] constants, out string asm) {
			Compiler compiler = new Compiler(
				IR,
				constants
			);
			compiler.Compile();
			asm = compiler.Output;
		}

		static void Simulate(in byte[] IR, in object[] constants) {
			Console.WriteLine("[ Status ] Initializing emulator...");
			int ip = 0;

			Dictionary<int, object> constant_map = new Dictionary<int, object>();

			{
				Console.WriteLine("[ Status ] Mapping constants for faster lookup...");
				Stopwatch time = new Stopwatch();
				time.Start();

				int idx = 0;
				for (int instr = 0; instr < IR.Length; instr++)
				{
					if((Instruction)IR[instr] != Instruction.OpPushConst) continue;

					constant_map[instr] = constants[idx++];
				}

				time.Stop();
				Console.WriteLine($"[ Status ] Mapped constants to dictionary in {time.ElapsedMilliseconds}ms.");

				if(s_Verbose) {
					Console.WriteLine(
						"[ Verbose ] Constant mapping table:\n			" +
						string.Join(
							"\n			",
							constant_map
						)
					);
				}
			}

			Console.WriteLine("[ Status ] Initialized emulator env ok, setting up VM...");

			Stack<object> ProgramStack = new Stack<object>();

			Console.WriteLine("[ Status ] VM init ok, beginning execution.");

			for (; ip < IR.Length; ip++) {
				switch(IR[ip]) {
					case (byte)Instruction.OpPushConst: {
						ProgramStack.Push(constant_map[ip]);
						break;
					}
					case (byte)Instruction.IntrinsicAdd: {
						int a = (int)ProgramStack.Pop();
						int b = (int)ProgramStack.Pop();
						ProgramStack.Push(a + b);
						break;
					}
					case (byte)Instruction.SimulatorIntrinsicPrint: {
						object value = ProgramStack.Pop();
						Console.WriteLine($"[ Program ] {value}");
						break;
					}
					case (byte)Instruction.SimulatorIntrinsicPrintNoPop: {
						object value = ProgramStack.Peek();
						Console.WriteLine($"[ Program ] {value}");
						break;
					}
					default: throw new NotImplementedException($"Unrecognized instruction {(Instruction)IR[ip]}");
				}
			}

			Console.WriteLine("[ Status ] Program execution complete, exiting...");
		}
	}
}
