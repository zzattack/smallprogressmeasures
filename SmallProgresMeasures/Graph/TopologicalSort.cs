using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures.Graph {
	class TopologicalSort {
		static LinkedList<List<Vertex>> L = new LinkedList<List<Vertex>>();
		static Queue<List<Vertex>> Q = new Queue<List<Vertex>>();
		static LinkedList<List<Vertex>> _sccs;

		public static IEnumerable<List<Vertex>> Sort(IEnumerable<List<Vertex>> sccs) {
			L.Clear();
			Q.Clear();
			_sccs = new LinkedList<List<Vertex>>(sccs);

			// mark sccs as unvisited
			foreach (var scc in sccs) scc[0].index = -1;

			// first the ones with no incoming edges
			//var finals = sccs.Where(scc => sccs.All(Sj => scc == Sj || !Sj.Any(v => v.Adj.Any(w => scc.Contains(w)))));
			//foreach (var scc in finals)
			//	visit1(scc);

			// first the ones with no outgoing edges
			var finals = sccs.Where(Si => Si.All(v => v.Adj.All(w => Si.Contains(w))));
			foreach (var scc in finals)
				visit2(scc);



			return L;
		}

		public static void visit1(List<Vertex> scc) {
			if (scc[0].index == -1) {
				scc[0].index = 0; // mark visited
				foreach (var v in scc)
					foreach (var w in v.Adj)
						visit1(w.SCC);
				L.AddLast(scc);
			}
		}

		public static void visit2(List<Vertex> scc) {
			if (scc[0].index != -1) return;
			scc[0].index = 0; // mark visited
			L.AddLast(scc);
			// now the sccs with an edge to this scc
			foreach (var v in scc)
				foreach (var w in v.Inc)
					visit2(w.SCC);
		}

	}
}
