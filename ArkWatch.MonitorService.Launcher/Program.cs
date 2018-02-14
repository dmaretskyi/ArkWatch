using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Storage;
using Topshelf;

namespace ArkWatch.MonitorService.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>                                   
            {
                x.Service<MonitorService.Monitor>(s =>                                  
                {
                    s.ConstructUsing(name => new Monitor(new JsonStorageProvider("storage.json"), new HistoryStorageProvider("history/")));                
                    s.WhenStarted(m => m.Start());                         
                    s.WhenStopped(m => m.Stop());                          
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.SetDescription("Monitors players on ARK servers and records history");                  
                x.SetDisplayName("ARK Player Monitor");                                 
                x.SetServiceName("ArkPlayerMonitor");                                 
            });                                                            

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
