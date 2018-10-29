
using System.Collections.Generic;

namespace HotChocolate.Directives.Http
{
    internal class UrlDecoder
    {
        // note: https://tools.ietf.org/html/rfc3986#section-2.2
        private static readonly Dictionary<string, string> _map =
            new Dictionary<string, string>
            {
                { "%20", " " },
                { "%3A", ":" },
                { "%2F", "/" },
                { "%3F", "?" },
                { "%23", "#" },
                { "%5B", "[" },
                { "%5D", "]" },
                { "%40", "@" },
                { "%21", "!" },
                { "%24", "$" },
                { "%26", "&" },
                { "%27", "'" },
                { "%28", "(" },
                { "%29", ")" },
                { "%2A", "*" },
                { "%2B", "+" },
                { "%2C", "," },
                { "%3B", ";" },
                { "%3D", "=" }
            };

        public string Decode(string input)
        {
            string output = "";

            foreach (char character in input)
            {
                string c = character.ToString();

                if (_map.ContainsKey(c))
                {
                    c = _map[c];
                }

                output = output + c;
            }

            return output;
        }
    }
}
