namespace TwinsUpdateBetaFunctionApp.Model
{
    public class Patch
    {
        public string value { get; set; }

        public string path { get; set; }

        public string op { get; set; }
    }
}
