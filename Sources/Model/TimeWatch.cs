using System;
using System.Diagnostics;

namespace CMScoutIntrinsic {

    struct TimeWatch : IDisposable {
        public TimeWatch(String description) {
            _description = description;
            _stopwatch   = new Stopwatch();

            _stopwatch.Start();
        }

        public void Dispose() {
            _stopwatch.Stop();

            Debug.WriteLine("{0} time: {1} ms", _description, _stopwatch.Elapsed.TotalMilliseconds);
        }



        private String    _description;
        private Stopwatch _stopwatch;
    }

}
