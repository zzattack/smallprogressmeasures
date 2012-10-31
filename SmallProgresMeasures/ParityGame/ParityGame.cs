using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PGSolver;

namespace SmallProgresMeasures {
	class ParityGame {
		public List<Vertex> V;
		public List<Edge> E;

		public ParityGame(PGSolver.ParseNode pgNode) {
			V = new List<Vertex>();
			E = new List<Edge>();

			// in order to maintain input order in-tact for the InputOrder lifting strategy,
			// we'll prepare a reference to each node name first
			pgNode.Nodes.Where(n => n.Token.Type == TokenType.NodeSpec).ToList()
				.ForEach(v => V.Add(new Vertex(v.Nodes[0].Token.Text))); 

			foreach (var node in pgNode.Nodes.Where(n => n.Token.Type == TokenType.NodeSpec)) {
				Vertex v = GetV(node.Nodes[0].Token.Text);
				if (node.Nodes[4].Token.Type == TokenType.STRING)
					v.Name = node.Nodes[4].Token.Text.Trim('"');

				v.Priority = int.Parse(node.Nodes[1].Token.Text);
				v.OwnerEven = node.Nodes[2].Token.Text == "0";
				// add edges
				foreach (string to in node.Nodes[3].Nodes.Where(n => n.Token.Type != TokenType.COMMA).Select(n => n.Token.Text))
					E.Add(new Edge(v, GetV(to)));
			}
		}

		public Vertex GetV(string id) {
			var v = V.FirstOrDefault(n => n.Id == id);
			if (v == null) {
				v = new Vertex(id);
				V.Add(v);
			}
			return v;
		}
		
		internal IEnumerable<Edge> EdgesFrom(Vertex v) {
			return E.Where(e => e.From == v);
		}
	}
}
