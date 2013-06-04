using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PGSolver;
using SmallProgresMeasures.Strategies;

namespace SmallProgresMeasures {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			var p = new Parser(new Scanner());

			var stratNames = new List<string> {
				"InputOrder", 
				"ReverseOrder",
				"Alternating",
				"RandomOrder",
				"RandomOrder2",
				"MinEdges",
				"MaxEdges",
				"MinPriority",
				"MaxPriority",
				"Hybrid",
				"SCC",
			};

			var strats = new List<LiftingStrategy> {
				SimpleStrategies.InputOrder,
				SimpleStrategies.ReverseInputOrder,
				new AlternatingStrategy(),
				SimpleStrategies.RandomOrder,
				SimpleStrategies.RandomOrder2,
				SimpleStrategies.MinEdges,
				SimpleStrategies.MaxEdges,
				SimpleStrategies.MinPriority,
				SimpleStrategies.MaxPriority,
				SimpleStrategies.Hybrid,
				new SCCLiftingStrategy(),
			};


			string[] games = { "elevator1", 
							 "elevator2" };

			//var resultsArray = new int[1, games.Count()];
			var resultsArray = new int[6, strats.Count()];

			for (int gameIdx = 0; gameIdx < games.Length; gameIdx++) {
				string gameName = games[gameIdx];
				int gameSizeIdx = 0;
				foreach (var input in Directory.GetFiles("../../data/elevator/", "*" + gameName + "*.gm")) {
					var pgame = ParityGame.ParseDirect(input);
					pgame.ConvertToSolitaire(true);

					var solver = new Jurdzinsky(pgame);
					//Log(Path.GetFileNameWithoutExtension(input));

					for (int stratIdx = 0; stratIdx < strats.Count; stratIdx++) {
						var strat = strats[stratIdx];
						sw.Restart();
						var result = solver.Solve(strat);
						sw.Stop();

						var evenWins = result.OrderBy(kvp => kvp.Key.Id).Where(kvp => kvp.Value);
						string winningVertices = string.Join(",", evenWins.Select(v => v.Key.Name).Take(5));

						Log("{0} in {1}ms: {2} for initial state", stratNames[stratIdx].PadRight(15),
						    sw.ElapsedMilliseconds.ToString().PadLeft(7), result[pgame.V[0]]);

						//resultsArray[gameSizeIdx, gameIdx] = (int) sw.ElapsedMilliseconds;
						resultsArray[gameSizeIdx, stratIdx] = (int) sw.ElapsedMilliseconds;

						logWriter.Flush();
					}
					gameSizeIdx++;
					//Log("".PadLeft(20, '-'));
				}

				// Print per strategy
				var sb = new StringBuilder();
				sb.AppendLine("% table for " + gameName);
				sb.AppendLine(@"\begin{table}");
				for (int j = 0; j < strats.Count(); j++) {
					sb.Append(stratNames[j]);
					for (int i = 0; i < 6; i++) {
						sb.Append(" & ");
						sb.Append(resultsArray[i, j].ToString(CultureInfo.InvariantCulture));
						sb.Append("ms");
					}
					sb.AppendLine(@"\\");
				}
				sb.AppendLine(@"\end{table}");
			
				/*// Print single strategy per game
					var sb = new StringBuilder();
					sb.AppendLine("% table for " + "SCC");
					sb.AppendLine(@"\begin{table}");
					for (int j = 0; j < games.Count(); j++) {
						sb.Append(games[j]);
						for (int i = 0; i < 1; i++) {
							sb.Append(" & ");
							sb.Append(resultsArray[i, j].ToString(CultureInfo.InvariantCulture));
							sb.Append("ms");
						}
						sb.AppendLine(@"\\");
					}
					sb.AppendLine(@"\end{table}");
					LogDirect(sb.ToString());*/

				LogDirect(sb.ToString());

				logWriter.Flush();
			}

			Console.ReadKey();
		}


		static void Log(string format = "", params object[] p) {
			string logstr = string.Format(format, p);
			LogDirect(logstr);
		}

		static void LogDirect(string logstr) {
			Console.WriteLine(string.Format("[{0:00000000}]\t\t{1}", sw.ElapsedMilliseconds, logstr));
			logWriter.WriteLine(logstr);
		}
		static Stopwatch sw = new Stopwatch();
		static TextWriter logWriter = new StreamWriter("console_" + DateTime.Now.ToString("ddMMyy-HHmmss") + ".log");
	}




}
