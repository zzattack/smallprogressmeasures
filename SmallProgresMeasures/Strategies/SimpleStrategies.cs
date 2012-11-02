using System;
using System.Linq;
using System.Collections.Generic;

namespace SmallProgresMeasures.Strategies {
	delegate IEnumerable<Vertex> SimpleLiftingStrategy(StrategySet strategies, IEnumerable<Vertex> V);

	static class SimpleStrategies {

		public static LiftingStrategy InputOrder = new GenericLiftingStrategy((rho, V) => V);

		public static LiftingStrategy ReverseInputOrder = new GenericLiftingStrategy((rho, V) => {
			var ret = new List<Vertex>();
			ret.AddRange(V);
			ret.Reverse();
			return ret;
		});

		static readonly SimpleLiftingStrategy _randomOrder = (rho, V) => {
			var r = new Random();
			var vs = V.ToList(); // copy
			// shuffle
			int n = vs.Count();
			while (n > 1) {
				int k = r.Next(--n + 1);
				// swap k,n
				var value = vs[k];
				vs[k] = vs[n];
				vs[n] = value;
			}
			return vs;
		};
		
		public static LiftingStrategy RandomOrder = new GenericLiftingStrategy(_randomOrder, false);
		public static LiftingStrategy RandomOrder2 = new GenericLiftingStrategy(_randomOrder, true);

		public static LiftingStrategy MinEdges = new GenericLiftingStrategy(
			(rho, V) => V.OrderBy(v => v.Adj.Count()).ToList());

		public static LiftingStrategy MaxEdges = new GenericLiftingStrategy((rho, V) =>
			V.OrderByDescending(v => v.Adj.Count).ToList());

		public static LiftingStrategy MinPriority = new GenericLiftingStrategy((rho, V) =>
			V.OrderBy(v => v.Priority).ToList());

		public static LiftingStrategy MaxPriority = new GenericLiftingStrategy((rho, V) =>
			V.OrderByDescending(v => v.Priority).ToList());

		public static LiftingStrategy Hybrid = new GenericLiftingStrategy((rho, V) => {
			var ret = new List<Vertex>();
			// first the odd-nodes with odd priority and a self-loop
			ret.AddRange(V.Where(v => v.OwnerOdd && v.PriorityOdd && v.Adj.Contains(v)));

			// then the remaining odd nodes with even priority, by ascending priority
			ret.AddRange(V.Except(ret).Where(v => v.OwnerOdd && v.PriorityEven)
				.OrderBy(v => v.Priority));

			// and the then remaining odd nodes, by descending number of neighbours
			ret.AddRange(V.Except(ret).Where(v => v.OwnerOdd && v.PriorityOdd)
				.OrderByDescending(v => v.Adj.Count));

			// finally the even nodes, except those with a forceable
			// even loop, by increasing # of neighbours
			ret.AddRange(V.Where(v => v.OwnerEven && !(v.PriorityEven && v.Adj.Contains(v)))
				.OrderBy(v => v.Adj.Count));

			return ret;
		});
		
	}
}
