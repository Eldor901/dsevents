using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using dsevents.Models;

namespace dsevents
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = readJsonFileFromArgument();
            List<string> events = new List<string>();
            
            ModelJson jsonModel = JsonSerializer.Deserialize<ModelJson>(line);

            try
            {
                if (args[0] == "past")
                {
                    findPastElements(args[1], jsonModel, events);
                }else if (args[0] == "future")
                {
                    findFutureElements(args[1], jsonModel, events);
                }else if (args[0] == "concurrent")
                {
                    List<string> tempEvents = new List<string>();
                    findPastElements(args[1], jsonModel, tempEvents);
                    findFutureElements(args[1], jsonModel, tempEvents);
                    findConcurrentElements(events, tempEvents, jsonModel);
                }
                else
                {
                    throw new InvalidExpressionException("Command not found");
                }
            
                PrintList(events, args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
           
            }
            
        }

        private static string readJsonFileFromArgument()
        {
            string json = "";
            string line = "";
            while(line != null)
            {
                line = Console.ReadLine();
                json += line;
            }

            return json;
        }

        private static void findPastElements(string elem, ModelJson jsonModel, List<string> events)
        {
            for (int i = 0; i < jsonModel.Events.Count; i++)
            {
                if (jsonModel.Events[i].ID == elem)
                {
                    for (int j = i; jsonModel.Events[j].Seq != 1; j--)
                    {
                        if (j - 1 >= 0)
                        {
                              events.Add(jsonModel.Events[j - 1].ID);
                        }

                        if (jsonModel.Events[j].ChannelID != null)
                        {
                            for (int k = 0; k < jsonModel.Events.Count; k++)
                            {
                                if (jsonModel.Events[j].ChannelID == jsonModel.Events[k].ChannelID &&
                                    jsonModel.Events[j].ID != jsonModel.Events[k].ID)
                                {
                                    if (isSituatedLeft(k, jsonModel))
                                    {
                                       events.Add(jsonModel.Events[k].ID);
                                       findPastElements(jsonModel.Events[k].ID, jsonModel, events);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private static bool isSituatedLeft(int pos1, ModelJson jsonModel)
        {
            string channel = jsonModel.Events[pos1].ChannelID;
            string process = jsonModel.Events[pos1].ProcessID;

            for (int i = 0; i < jsonModel.Channels.Count; i++)
            {
                if (jsonModel.Channels[i].ID == channel)
                {
                    if (jsonModel.Channels[i].From == process)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void findFutureElements(string elem, ModelJson jsonModel, List<string> events)
        {
            for (int i = 0; i < jsonModel.Events.Count; i++)
            {
                if (jsonModel.Events[i].ID == elem)
                {
                    
                        for (int j = i; jsonModel.Events[i].ProcessID == jsonModel.Events[j].ProcessID; j++)
                        {
                            
                            
                            if (j <= jsonModel.Events.Count + 1)
                            {
                                events.Add(jsonModel.Events[j].ID);
                            }

                            if (jsonModel.Events[j].ChannelID != null)
                            {
                                for (int k = 0; k < jsonModel.Events.Count; k++)
                                {
                                    if (jsonModel.Events[j].ChannelID == jsonModel.Events[k].ChannelID &&
                                        jsonModel.Events[j].ID != jsonModel.Events[k].ID)
                                    {
                                        if (!isSituatedLeft(k, jsonModel))
                                        {
                                            events.Add(jsonModel.Events[k].ID);
                                            findFutureElements(jsonModel.Events[k].ID, jsonModel, events);
                                        }
                                    }
                                }
                            }
                            
                            if (j + 1  >= jsonModel.Events.Count)
                            {
                                break;
                            }
                        }
                }
            }
        }

        private static void findConcurrentElements(List<string> events, List<string> tempEvents, ModelJson jsonModel)
        {
            for (int i = 0; i < jsonModel.Events.Count; i++)
            {
                if (!tempEvents.Contains(jsonModel.Events[i].ID))
                {
                    events.Add(jsonModel.Events[i].ID);
                }
            }
        }


        private static void PrintList(List<string> events, string elem)
        {
            events.Remove(elem);
            var uniques = events.Distinct().ToArray();


            foreach (var str in uniques)
            {
                Console.WriteLine(str);
            }

        }

    }
}