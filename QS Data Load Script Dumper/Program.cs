using Qlik.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QS_Data_Load_Script_Dumper
{
    class Program
    {
        static void Main(string[] args)
        {
            string dumpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Scripts");

            using (var location = ConnectWithNTLMProxy("https://qlikserver1.domain.local"))
            {
                // Dumps the data load script for all apps accessible by the current user context.
                foreach (var entry in location.GetAppIdentifiers())
                {
                    Console.WriteLine(entry.AppName);

                    IAppIdentifier appId = location.AppWithIdOrDefault(entry.AppId);
                    ISession session = Session.WithApp(appId, SessionType.Default);
                    IApp app = location.App(appId, session, noData: true);

                    Directory.CreateDirectory(dumpPath);
                    File.WriteAllText(path: Path.Combine(dumpPath, $"{entry.AppName}.txt"), app.GetScript());
                }
            }
            
            Console.WriteLine($"\nDump location: {dumpPath}\n");

            // Pauses console before exiting.
            Console.Write("Press any key to exit . . .");
            Console.ReadKey(intercept: true);
        }

        private static ILocation ConnectWithNTLMProxy(string server)
        {
            ILocation location = Location.FromUri(new Uri(server));

            location.AsNtlmUserViaProxy(proxyUsesSsl: true);
            return location;
        }
    }
}
