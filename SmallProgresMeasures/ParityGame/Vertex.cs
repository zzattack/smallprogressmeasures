using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	class Vertex {
		public string Id { get; set; }
		public string Name { get; set; } // optional
		public Vertex(string id) {
			Id = id;
			Name = "";
		}

		public int Priority { get; set; }
		public bool OwnerEven { get; set; }

		public bool OwnerOdd {
			get { return !OwnerEven; }
			set { OwnerEven = !value; }
		}

		public override string ToString() {
			return string.Format("{0}{1} ({2}){3}", 
				OwnerEven ? "<" : "[", string.IsNullOrEmpty(Name) ? Id : Name,
				Priority, OwnerEven ? ">" : "]");
		}
	}
}
