using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	class Edge {
		public Vertex From { get; set; }
		public Vertex To { get; set; }
		public Edge(Vertex from, Vertex to) {
			From = from;
			To = to;
		}

		public override string ToString() {
			return string.Format("{0}->{1}", From, To);
		}
	}
}
