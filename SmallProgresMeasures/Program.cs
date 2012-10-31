using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PGSolver;

namespace SmallProgresMeasures {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			var p = new Parser(new Scanner());
			var tree = p.Parse(File.ReadAllText("../../sample2.pg"));
			var start = tree.Nodes[0];
			var pgame = new ParityGame(start.Nodes[0]);
			var solver = new Jurdzinsky(pgame);
			solver.Solve(LiftingStrategies.InputOrder);
		}
	}
}
