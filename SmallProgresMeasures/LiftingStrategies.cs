using System;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	static class LiftingStrategies {
		static Random r = new Random();

		public static Jurdzinsky.LiftingStrategy Random = delegate(StrategySet rho, Jurdzinsky j) {
			var vs = rho.Keys.ToList();
			vs = vs.Where(v => IsLiftable(v, rho, j)).ToList();
			return vs.Count == 0 ? null : vs[r.Next(vs.Count)];
		};

		public static Jurdzinsky.LiftingStrategy InputOrder = delegate(StrategySet rho, Jurdzinsky j) {
			var vs = rho.Keys.ToList();
			return vs.FirstOrDefault(v => IsLiftable(v, rho, j));
		};

		private static bool IsLiftable(Vertex v, StrategySet rho, Jurdzinsky j) {
			// returns whether calling Prog() on any edge makes any progress

			// in case v belongs to even, w take the minimum progress, so every must give progress;
			// otherwise picking the minimum one that gives progress still does nothing)
			if (v.OwnerEven)
				return j.pg.EdgesFrom(v).All(e => j.Prog(rho, v, e.To) > rho[v]);
			// else it's fine if taking any edge gives any progress, as we take the maximum progress
			else
				return j.pg.EdgesFrom(v).Any(e => j.Prog(rho, v, e.To) > rho[v]);
		}

	}
}
