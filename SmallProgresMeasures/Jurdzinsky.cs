using System.Collections.Generic;
using System.Linq;
using SmallProgresMeasures.Strategies;

namespace SmallProgresMeasures {
	class Jurdzinsky {
		//public readonly DTuple MMaxOdd;
		public DTuple MMaxEven;
		public readonly DTuple MTop;
		int d; // max priority + 1
		ParityGame pg;

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
			liftStrat.Initialize(this.pg);
			var rho = new StrategySet();
			foreach (var v in pg.V)
				rho[v] = new DTuple(d);

			// calculate fixpoint
			var workBatch = liftStrat.GetBatch();
			while (workBatch != null && workBatch.Count() > 0) {
				// lifting strategy determines which order in which to lift vertices
				foreach (var v in workBatch) {
					// repeatedly lift vertex v
					DTuple old;
					int numIteration = 0;
					do {
						if (numIteration++ > 0)
							liftStrat.Progress(); // tell strategy we made progress
						old = rho[v];
						Lift(v, rho);
					} while (rho[v] > old);
				}
				// continue with next batch
				workBatch = liftStrat.GetBatch();
			}

			var ret = new Dictionary<Vertex, bool>();
			foreach (var entry in rho) ret[entry.Key] = entry.Value != MTop;
			return ret;
		}

		private void Lift(Vertex v, StrategySet rho) {
			var ps = v.Adj.Select(w => Prog(rho, v, w)).OrderBy(d => d);
			rho[v] = v.OwnerEven ? ps.First() : ps.Last();
		}

		public DTuple Prog(StrategySet rho, Vertex v, Vertex w) {
			var rho_w = rho[w];
			var ret = new DTuple(rho_w.Count);
			if (v.PriorityEven) {
				// return the least m >= (up to p(v)) rho
				// easily constructed by copying values up to p(v) and padding with 0
				if (rho_w.IsTop) ret = MTop;
				else for (int i = 0; i < v.Priority; i++)
						ret[i] = rho_w[i];
			}
			else {
				// return the least m > (up to p(v)) rho[w]
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
