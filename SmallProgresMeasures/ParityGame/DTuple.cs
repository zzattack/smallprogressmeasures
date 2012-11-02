using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SmallProgresMeasures {
	internal class LiftValue {
		public Vertex v;
		public int value;
		public LiftValue(Vertex v, int val) {
			this.v = v;
			value = val;
		}
	}

	class DTuple : List<int>, IComparable<DTuple> {
		public DTuple(int size) {
			while (size-- > 0)
				Add(0);
		}


		#region operators >=, >, <, <=
		public bool GreaterUpto(DTuple other, int p) {
			for (int i = 0; i < p; i++) {
				if (this[i] > other[i]) return true;
				else if (this[i] < other[i]) return false;
				else ; // loop checks next element
			}
			return false; // if matched up to here, then equal and not greater
		}
		public bool GreaterOrEqualUpto(DTuple other, int p) {
			for (int i = 0; i < p; i++) {
				if (this[i] > other[i]) return true;
				else if (this[i] < other[i]) return false;
				else ; // loop checks next element
			}
			return true; // if matched up to here, then equal
		}
		public bool EqualTo(DTuple other) {
			for (int i = 0; i < Count; i++) {
				if (this[i] != other[i]) return false;
			}
			return true; // if matched up to here, then equal
		}
		public bool SmallerThenUpto(DTuple other, int p) {
			return !GreaterOrEqualUpto(other, p);
		}
		public bool SmallerOrEqualUpto(DTuple other, int p) {
			return !GreaterUpto(other, p);
		}
		public static bool operator >(DTuple a, DTuple b) {
			return a.GreaterUpto(b, a.Count);
		}
		public static bool operator <(DTuple a, DTuple b) {
			return a.SmallerThenUpto(b, a.Count);
		}
		public static bool operator >=(DTuple a, DTuple b) {
			return a.GreaterOrEqualUpto(b, a.Count);
		}
		public static bool operator <=(DTuple a, DTuple b) {
			return a.SmallerOrEqualUpto(b, a.Count);
		}
		public static bool operator ==(DTuple a, DTuple b) {
			return a.EqualTo(b); // liever lui dan moe
		}
		public static bool operator !=(DTuple a, DTuple b) {
			return !(a == b);
		}
		public bool IsTop {
			get { return this.Any(d => d == int.MaxValue); }
		}

		#endregion


		public int CompareTo(DTuple other) {
			if (this < other) return -1;
			else if (this == other) return 0;
			else return 1;
		}

		public override string ToString() {
			if (this.Any(i => i == int.MaxValue)) return "T";
			else return string.Format("({0})", string.Join(",", this.Select(i => i.ToString(CultureInfo.InvariantCulture))));
		}

	}
}
