using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallProgresMeasures.Graph;

namespace SmallProgresMeasures.Strategies {
	abstract class LiftingStrategy {
		protected ParityGame pg;
		protected bool progress;

		public virtual void Initialize(ParityGame pg) {
			this.pg = pg;
			progress = true;
		}
		public void Progress() {
			progress = true;
		}
		public abstract IEnumerable<Vertex> GetBatch();
	}

	class GenericLiftingStrategy : LiftingStrategy {
		SimpleLiftingStrategy simple;
		List<Vertex> cachedResult;
		bool cached;

		public GenericLiftingStrategy(SimpleLiftingStrategy simple, bool cached = true) {
			this.simple = simple;
			this.cached = cached;
		}

		public override void Initialize(ParityGame pg) {
			base.Initialize(pg);
			if (cached)
				cachedResult = simple.Invoke(null, pg.V).ToList();
		}

		public override IEnumerable<Vertex> GetBatch() {
			if (!progress) return null; // no progress was made, so we're done
			progress = false;
			if (cached) return cachedResult;
			else return simple.Invoke(null, pg.V);
		}
	}

	class SCCLiftingStrategy : LiftingStrategy {
		private IEnumerable<List<Vertex>> _sccs;
		IEnumerator<List<Vertex>> _sccEnumerator;

		public override void Initialize(ParityGame pg) {
			_sccs = TarjanIterative.DetectCycles(pg);
			_sccs = TopologicalSort.Sort(_sccs);
			progress = true;
			_sccEnumerator = _sccs.GetEnumerator();
			_sccEnumerator.MoveNext(); // move to first
		}

		public override IEnumerable<Vertex> GetBatch() {
			// if we made no progress in the current SCC, we move on to the 
			// next one in the toplogical sort
			if (!progress)
				if (!_sccEnumerator.MoveNext())
					// but if there's nothing left
					return null;
			// reset progress
			progress = false;
			return _sccEnumerator.Current;
		}
	}

	class AlternatingStrategy : LiftingStrategy {
		bool _forward;
		List<Vertex> _forwardList, _backwardList; 
		public override void Initialize(ParityGame pg) {
			progress = true;
			_forward = false;
			_forwardList = pg.V.ToList();
			_backwardList = pg.V.ToList();
			_backwardList.Reverse();
		}

		public override IEnumerable<Vertex> GetBatch() {
			// if we made no progress in the current SCC, we move on to the 
			// next one in the toplogical sort
			if (!progress) return null;
			progress = false;
			_forward = !_forward;
			return _forward ? _forwardList : _backwardList;
		}
	}

}
