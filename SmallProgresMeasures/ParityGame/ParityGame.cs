using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PGSolver;

namespace SmallProgresMeasures {
	class ParityGame {
		public Vertex[] V;
		public List<Edge> E;
		private ParityGame() { }

		public ParityGame(ParseNode pgNode) {
			V = new Vertex[int.Parse(pgNode.Nodes[0].Nodes[1].Token.Text) + 1];
			//E = new List<Edge>();

			foreach (var node in pgNode.Nodes.Where(n => n.Token.Type == TokenType.NodeSpec)) {
				Vertex v = GetV(int.Parse((node.Nodes[0].Token.Text)));
				if (node.Nodes[4].Token.Type == TokenType.STRING)
					v.Name = node.Nodes[4].Token.Text.Trim('"');

				v.Priority = int.Parse(node.Nodes[1].Token.Text);
				v.OwnerEven = node.Nodes[2].Token.Text == "0";
				// add edges
				foreach (string to in node.Nodes[3].Nodes.Where(n => n.Token.Type != TokenType.COMMA).Select(n => n.Token.Text)) {
					var w = GetV(int.Parse(to));
					//E.Add(new Edge(v, w));
					v.Adj.Add(w);
					w.Inc.Add(v);
				}
			}
		}

		public Vertex GetV(int id) {
			var v = V[id];
			if (v == null) {
				v = new Vertex(id);
				V[id] = v;
			}
			return v;
		}

		public static ParityGame ParseDirect(string PGfile) {
			var sr = new StreamReader(PGfile);
			var ret = new ParityGame();
			string firstline = sr.ReadLine();

			int numNodes = 1 + int.Parse(firstline.Split(' ')[1].TrimEnd(';'));
			ret.V = new Vertex[numNodes];
			//ret.E = new List<Edge>();
			while (!sr.EndOfStream) {
				string line = sr.ReadLine();
				string[] tokens = line.TrimEnd(';').Split(' ');
				Vertex v = ret.GetV(int.Parse(tokens[0]));
				v.Priority = int.Parse(tokens[1]);
				v.OwnerEven = tokens[2] == "0";
				foreach (var succ in tokens[3].Split(',')) {
					var w = ret.GetV(int.Parse(succ));
					//ret.E.Add(new Edge(v, w));
					v.Adj.Add(w);
					w.Inc.Add(v);
				}
				if (tokens.Length >= 5)
					v.Name = tokens[4].Trim('"');
			}
			return ret;
		}

		public void ConvertToSolitaire(bool forEven) {
			// in order to convert this game to a solitaire game we need a
			// (historyless) strategy for the given player. we chose as strategy to 
			// always just visit the first edge from the adjacency list. converting
			// to a solitaire game then just requires removing all other edges
			var vs = V.Where(v => v.OwnerEven == forEven);
			foreach (var v in vs) {
				//var toRemove = v.Adj.GetRange(1, v.Adj.Count - 1);
				//E.RemoveAll(e => toRemove.Contains(e.To));
				for (int i = 1; i < v.Adj.Count; i++)
					v.Adj[i].Inc.Remove(v);
				v.Adj.RemoveRange(1, v.Adj.Count - 1);
			}
		}


	}
}
