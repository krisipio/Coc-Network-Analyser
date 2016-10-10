using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace coc_messages_csharp
{
    public class Definitions
    {
        public static JObject read()
        {
            JObject definitions = new JObject();

            JObject parent, current;
            foreach (string subdirectory in Directory.GetDirectories(Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), @"messages\definitions")))
            {
                Console.WriteLine(Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), @"messages\definitions"));
                if (Path.GetFileName(subdirectory) == "component")
                {
                    parent = new JObject();
                    definitions.Add("component", parent);
                }
                else
                {
                    parent = definitions;
                }
                foreach (string filename in Directory.GetFiles(subdirectory))
                {
                    current = JObject.Parse(File.ReadAllText(filename));
                    if (Path.GetFileName(subdirectory) == "component")
                    {
                        parent.Add((string)current["name"], current);
                    }
                    else
                    {
                        parent.Add((string)current["id"], current);
                    }
                }
            }
            return definitions;
        }
    }
}
