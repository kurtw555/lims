﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimsServer.Entities
{
    public class Workflow
    {
        //private string BaseNetworkPath = @"\\AA\ORD\ORD\PRIV";
        public string id { get; set; }
        public string name { get; set; }
        public string processor { get; set; }
        public string inputFolder { get; set; }
        public string outputFolder { get; set; }
        //Interval in minutes
        public int interval { get; set; }
        public string message { get; set; }

        public bool active { get; set; }

        public Workflow() { }

        public void Update(Workflow wf)
        {
            this.id = wf.id;
            this.name = wf.name;
            this.processor = wf.processor;
            this.inputFolder = wf.inputFolder;
            this.outputFolder = wf.outputFolder;
            this.interval = wf.interval;
            this.message = "";
            this.active = true;
        }

    }
}
