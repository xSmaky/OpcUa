﻿using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Collections.Generic;

namespace OpcUa.Client
{
    class Program
    {
        public static void Main()
        {
            using (var client = new OpcClient("opc.tcp://localhost:4840"))
            {
                client.Connect();
                Console.WriteLine("Connected");
                var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);
                while (true)
                {

                    Console.Write("(B/R/W) Browse/Read/Write nodes: ");

                    var input = Console.ReadLine();

                    switch (input)
                    {
                        case "B": Browse(client); break;
                        case "R": ReadNodes(client); break;
                        case "W": WriteNodes(client); break;
                        default: break;
                    }

                }
            }
        }

        public static void Browse(OpcClient client, int level = 0)
        {
            // Do not show Server node and its children
            OpcNodeInfo machineNode = client.BrowseNode("ns=2;s=Machine");

            var job = machineNode.Child("Job");

            foreach (var child in job.Children())
            {
                Console.WriteLine(child.AttributeValue(OpcAttribute.DisplayName));
            }

        }

        public static void ReadNodes(OpcClient client)
        {
            OpcReadNode[] commands = new OpcReadNode[] {
                        new OpcReadNode("ns=2;s=Machine/Job/Number"),
                        new OpcReadNode("ns=2;s=Machine/Job/Name"),
                        new OpcReadNode("ns=2;s=Machine/Job/Speed"),
                        new OpcReadNode("ns=2;s=Machine/Job/Test"),
            };

            IEnumerable<OpcValue> nodes = client.ReadNodes(commands);
            foreach (var node in nodes)
            {
                
                if (node.Status.IsGood)
                {
                    Console.WriteLine("* " + node);
                }
            }
        }

        public static void WriteNodes(OpcClient client)
        {
            
            OpcWriteNode[] commands = new OpcWriteNode[] {
                new OpcWriteNode("ns=2;s=Machine/Job/Number", OpcAttribute.DisplayName, new OpcText("Serial")),
                new OpcWriteNode("ns=2;s=Machine/Job/Name", OpcAttribute.DisplayName, new OpcText("Description")),
                new OpcWriteNode("ns=2;s=Machine/Job/Spee", OpcAttribute.DisplayName, new OpcText("Rotations per Second"))
            };

            OpcStatusCollection results = client.WriteNodes(commands);

            foreach (var result in results)
            {
                if (result.IsBad)
                {
                    Console.WriteLine("Failed to write: " + result.Description);
                }
                
            }
        }
    }
}