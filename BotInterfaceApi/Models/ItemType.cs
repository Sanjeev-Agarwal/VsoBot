using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotInterfaceApi.Models.WorkItem
{

    public class Rootobject
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Fields fields { get; set; }
        public _Links _links { get; set; }
        public string url { get; set; }
    }

    public class Fields
    {
        public string SystemAreaPath { get; set; }
        public string SystemTeamProject { get; set; }
        public string SystemIterationPath { get; set; }
        public string SystemWorkItemType { get; set; }
        public string SystemState { get; set; }
        public string SystemReason { get; set; }
        public string SystemAssignedTo { get; set; }
        public DateTime SystemCreatedDate { get; set; }
        public string SystemCreatedBy { get; set; }
        public DateTime SystemChangedDate { get; set; }
        public string SystemChangedBy { get; set; }
        public string SystemTitle { get; set; }
        public DateTime MicrosoftVSTSCommonClosedDate { get; set; }
        public string MicrosoftVSTSCommonClosedBy { get; set; }
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }
        public DateTime MicrosoftVSTSCommonActivatedDate { get; set; }
        public string MicrosoftVSTSCommonActivatedBy { get; set; }
        public int MicrosoftVSTSCommonPriority { get; set; }
        public int MicrosoftVSTSSchedulingOriginalEstimate { get; set; }
        public int MicrosoftVSTSSchedulingCompletedWork { get; set; }
    }

    public class _Links
    {
        public Self self { get; set; }
        public Workitemupdates workItemUpdates { get; set; }
        public Workitemrevisions workItemRevisions { get; set; }
        public Workitemhistory workItemHistory { get; set; }
        public Html html { get; set; }
        public Workitemtype workItemType { get; set; }
        public Fields1 fields { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Workitemupdates
    {
        public string href { get; set; }
    }

    public class Workitemrevisions
    {
        public string href { get; set; }
    }

    public class Workitemhistory
    {
        public string href { get; set; }
    }

    public class Html
    {
        public string href { get; set; }
    }

    public class Workitemtype
    {
        public string href { get; set; }
    }

    public class Fields1
    {
        public string href { get; set; }
    }

}