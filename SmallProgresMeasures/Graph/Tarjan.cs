using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	internal class Tarjan {

		private static List<List<Vertex>> _stronglyConnectedComponents;
		private static Stack<Vertex> S;
		private static int index;
		private static ParityGame dg;

		public static List<List<Vertex>> DetectCycles(ParityGame g) {
			_stronglyConnectedComponents = new List<List<Vertex>>();
			foreach (var v in g.V) v.index = -1;

			index = 0;
			S = new Stack<Vertex>();
			dg = g;
			foreach (Vertex v in g.V) {
				if (v.index < 0) {
					StrongConnect(v);
				}
			}
			return _stronglyConnectedComponents;
		}

		private static void StrongConnect(Vertex firstVertex) {
			Stack<Vertex> stack = new Stack<Vertex>();
			stack.Push(firstVertex);


			while (stack.Count > 0) {
				Vertex v = stack.Pop();
				v.index = index;
				v.lowlink = index;
				index++;
				S.Push(v);

				foreach (Vertex w in v.Adj) {
					if (w.index < 0) {
						StrongConnect(w);
						v.lowlink = Math.Min(v.lowlink, w.lowlink);
					}
					else if (S.Contains(w)) {
						v.lowlink = Math.Min(v.lowlink, w.index);
					}
				}

				if (v.lowlink == v.index) {
					var scc = new List<Vertex>();
					Vertex w;
					do {
						w = S.Pop();
						scc.Add(w);
					} while (v != w);
					_stronglyConnectedComponents.Add(scc);
				}
			}

		}
	}
}
