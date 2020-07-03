using System.Collections;
using System.Collections.Generic;
using dsevents.Models;

namespace dsevents
{
    public class ModelJson
    {
        public IList<Process> Processes { get; set;}
        public IList<Channel> Channels {get; set;}
        public IList<Message> Messages {get; set;}
        public IList<Event> Events {get; set;} 
    }
}