using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPC.ConsumerProducer
{
    internal class ObjLog
    {
       public string LogLevel {  get; set; }
       public string Message { get; set; }
       public ObjLog(string LogLevel, string Message) {
            this.LogLevel = LogLevel;
            this.Message = Message;
        }
    }
}
