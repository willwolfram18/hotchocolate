using System.Text.RegularExpressions;
using HotChocolate.Resolvers;

namespace HotChocolate.Directives.Http
{
    internal class UrlParser
    {
        private static readonly Regex _argumentPattern = new Regex(
            @"\{args\.([a-z]*)\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _resultPattern = new Regex(
            @"\{result\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Parse(string url, IDirectiveContext context)
        {


            return "";
        }

        private 
    }
}
