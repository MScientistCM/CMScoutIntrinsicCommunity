
namespace CMScoutIntrinsic {

    class Pair<TFirst, TSecond> {
        public Pair() {
        }

        public Pair(TFirst first, TSecond second) {
            First  = first;
            Second = second;
        }

        public TFirst  First  { get; set; }
        public TSecond Second { get; set; }
    }

}
