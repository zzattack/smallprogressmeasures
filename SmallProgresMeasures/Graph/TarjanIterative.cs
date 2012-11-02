using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	class TarjanIterative {

		private static List<List<Vertex>> _sccs = new List<List<Vertex>>();
		private static Stack<Vertex> _tarStack = new Stack<Vertex>();
		private static Dictionary<Vertex, bool> _onStack = new Dictionary<Vertex, bool>();

		public static List<List<Vertex>> DetectCycles(ParityGame g) {
			_sccs.Clear();
			_tarStack.Clear();
			_onStack.Clear();
			foreach (var v in g.V) v.index = -1;

			int index = 0;
			foreach (var v in g.V)
				_onStack[v] = false;
			for (int n = 0; n < g.V.Length; n++) {
				if (g.V[n].index == -1) {
					tarjan_iter(g.V[n], ref index);
				}
			}
			return _sccs;
		}

		static void tarjan_iter(Vertex u, ref int index) {
			u.index = index;
			u.lowlink = index;
			index++;
			u.vindex = 0;
			_tarStack.Push(u);
			u.caller = null;           //Equivalent to the node from which the recursive call would spawn.
			_onStack[u] = true;
			Vertex last = u;
			while (true) {
				if (last.vindex < last.Adj.Count) {       //Equivalent to the check in the for-loop in the recursive version
					Vertex w = last.Adj[last.vindex];
					last.vindex++;                                   //Equivalent to incrementing the iterator in the for-loop in the recursive version
					if (w.index == -1) {
						w.caller = last;
						w.vindex = 0;
						w.index = index;
						w.lowlink = index;
						index++;
						_tarStack.Push(w);
						_onStack[w] = true;
						last = w;
					}
					else if (_onStack[w] == true) {
						last.lowlink = Math.Min(last.lowlink, w.index);
					}
				}
				else {  //Equivalent to the nodeSet iterator pointing to end()
					if (last.lowlink == last.index) {
						Vertex top = _tarStack.Pop();
						var scc = new List<Vertex>() { top };
						top.SCC = scc;
						_onStack[top] = false;
						int size = 1;

						while (top.Id != last.Id) {
							top = _tarStack.Pop();
							top.SCC = scc;
							scc.Add(top);
							_onStack[top] = false;
							size++;
						}
						_sccs.Add(scc);
					}

					Vertex newLast = last.caller;   //Go up one recursive call
					if (newLast != null) {
						newLast.lowlink = Math.Min(newLast.lowlink, last.lowlink);
						last = newLast;
					}
					else {
						//We've seen all the nodes
						break;
					}
				}
			}
		}


	}
}
