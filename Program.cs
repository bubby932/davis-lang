using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Davis.Arch;

namespace Davis
{
	class Program
	{
		/// <remarks>
		/// Usage: <PATH> <Input Source Root> <Output Source Binary> <Additional (bits, etc)>
		/// </remarks>
		static void Main(string[] args)
		{
			// IO
			string input_path;
			string output_path;

			// Preprocessing
			string final_source;

			// IR
			byte[] IR;

			// x86
			Bits num_mode;
			string addtl_args;
			string final_asm;

			// If we don't have enough arguments, query the user.
			if(args.Length < 3) {
				Console.WriteLine("Enter input path.");
				input_path = Console.ReadLine();
				Console.WriteLine("Enter output path.");
				output_path = Console.ReadLine();
				Console.WriteLine("Enter additional argument flags.");
				string addtl Console.ReadLine();
			} else {
				input_path = args[1];
				output_path = args[2];
				if(args.Length > 3) {
					addtl_args = args[3];
				} else {
					addtl_args = "";
				}
			}

			string source = File.ReadAllText(input_path);

			PreprocessSource(in source, out final_source);

			CompileSourceToRepresentation(in final_source, out IR);
		}

		static void PreprocessSource(in string source, out string processed) {
			throw new NotImplementedException();
		}

		static void CompileSourceToRepresentation(in string source, out byte[] IR) {
			throw new NotImplementedException();
		}

		static void CompileToNasmX86(in byte[] IR, out string asm) {
			throw new NotImplementedException();
		}
	}
}
