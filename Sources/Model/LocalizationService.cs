using System;
using System.Threading.Tasks;

namespace CMScoutIntrinsic {

    class LocalizationService {
        public LocalizationService() {
        }

        public async Task ExposeAsync() {
            await Task.FromResult(false);
        }

        public String Translate(String context, String source) {
            return source;
        }

    }

}
