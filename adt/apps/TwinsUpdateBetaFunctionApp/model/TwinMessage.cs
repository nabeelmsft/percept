using System.Collections.Generic;

namespace TwinsUpdateBetaFunctionApp.Model
{
    public class TwinMessage
    {
        public string modelId { get; set; }

        public IList<Patch> patch { get; set; }
    }
}
