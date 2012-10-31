using System;
using System.Collections.Generic;
using System.Linq;

namespace SmallProgresMeasures {
	class Jurdzinsky {
		public readonly ParityGame pg;
		//public readonly DTuple MMaxOdd;
		public readonly DTuple MMaxEven;
		public readonly DTuple MTop;
		int d; // max priority + 1

		public delegate Vertex LiftingStrategy(StrategySet strategies, Jurdzinsky j);

		public Jurdzinsky(ParityGame pg) {
			this.pg = pg;
			// determine maximum values for dtuple
			d = pg.V.Max(m => m.Priority) + 1; // d = max{ p(v) | v in V} + 1.
			MMaxEven = new DTuple(d);
			for (int i = 1; i < d; i += 2)
				MMaxEven[i] = pg.V.Count(v => v.Priority == i);
			//MMaxOdd = new DTuple(d);
			//for (int i = 0; i < d; i += 2)
			//	MMaxOdd[i] = pg.V.Count(v => v.Priority == i);
			MTop = new DTuple(d);
			for (int i = 0; i < d; i++)
				MTop[i] = int.MaxValue;
		}

		/// <returns>a dictionary indicating for each node whether a winning strategy exists for even</returns>
		public Dictionary<Vertex, bool> Solve(LiftingStrategy liftStrat) {
			var rho = new StrategySet();
			foreach (var v in pg.V)
				rho[v] = new DTuple(d);

			// calculate fixpoint
			while (true) {
				// lifting strategy determines which vertex to lift
				Vertex v = liftStrat(rho, this);
				if (v == null) // we can't lift anything, i.e. no more progress in fixpoint calculation
					break;
				else {
					// repeatedly lift chosen vertex
					DTuple old;
					do {
						old = rho[v];
						rho[v] = Lift(v, rho);
					} while (rho[v] > old);
				}
			}

			var ret = new Dictionary<Vertex, bool>();
			foreach (var entry in rho) ret[entry.Key] = entry.Value != MTop;
			return ret;
		}

		private DTuple Lift(Vertex v, StrategySet rho) {
			var ps = new List<DTuple>();
			foreach (var edge in pg.EdgesFrom(v)) {
				ps.Add(Prog(rho, edge.From, edge.To));
			}
			// return smallest tuple if v belongs to even, else largest
			ps.Sort();
			return v.OwnerEven ? ps.First() : ps.Last();
		}

		public DTuple Prog(StrategySet rho, Vertex v, Vertex w) {
			var ret = new DTuple(d);
			var rho_w = rho[w];
			if (v.Priority % 2 == 0) {
				// return the least M >= (up to p(v)) rho
				// easily constructed by copying values up to p(v) and padding with 0
				if (rho_w.IsTop) ret = MTop;
				else for (int i = 0; i < v.Priority; i++)
					ret[i] = rho_w[i];
			}
			else {
				// return the least M > (up to p(v)) rho[w]
				// constructed by taking the first elem before p(v) that can be incremented, MTop otherwise
				for (int i = v.Priority; i >= 0; i--) {
					if (rho_w[i] < MMaxEven[i]) {
						ret[i] = rho_w[i] + 1;
						// leave stuff in front same as rho_w
						for (int j = 0; j < i; j++)
							ret[j] = rho_w[j];
						// and set remainder to 0
						for (int j = i + 1; j < d; j++)
							ret[j] = 0;
						break;
					}
					else if (i == 0) // nothing could be incremented
						ret = MTop;
					else // copy this val, try to incr. next
						ret[i] = rho_w[i];
				}
			}
			return ret;
		}


	}
}
