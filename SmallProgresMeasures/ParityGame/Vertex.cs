using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	class Vertex {
		public int Id { get; set; }
		string _name;
		public string Name { get { return _name ?? Id.ToString(); } set { _name = value; } } // optional
		public Vertex(int id) {
			Id = id;
			Adj = new List<Vertex>();
			Inc = new List<Vertex>();
		}

		public int Priority { get; set; }
		public bool OwnerEven { get; set; }
		public bool OwnerOdd { get { return !OwnerEven; } }
		public bool PriorityOdd { get { return Priority % 2 == 1; } }
		public bool PriorityEven { get { return Priority % 2 == 0; } }
		public List<Vertex> Adj { get; set; } // adjacent vertices
		public List<Vertex> Inc { get; set; } // incoming vertices

		public override string ToString() {
			return string.Format("{0}{1} ({2}){3}",
				OwnerEven ? "<" : "[", string.IsNullOrEmpty(_name) ? Id.ToString() : Name,
				Priority, OwnerEven ? ">" : "]");
		}

		// used by algorihm to detect cycles
		internal int index = -1;
		internal int lowlink = -1;
		public int vindex = -1;
		public Vertex caller = null;

		public List<Vertex> SCC { get; set; } // reference to scc this belongs to
	}
}
