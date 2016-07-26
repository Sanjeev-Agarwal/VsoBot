using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotInterfaceApi.Models.TFS
{
    public class Helper
    {
        public string Tasks {
            get {

                return "* Task id: 682005 *Define workflow to read from blob and send to topic*, [Task url:](https://microsoftit.visualstudio.com/OneITVSO/Mktg-MKS-BP-Marketing%20Budget%20Planning/_workItems?_a=edit&id=682005) * Task id: 554422 *Context improvement in Foreach*, [Task url:](https://microsoftit.visualstudio.com/OneITVSO/Mktg-MKS-BP-Marketing%20Budget%20Planning/_workItems?_a=edit&id=554422)";
            }
        }

        public string Bugs
        {
            get
            {

                return "* Bug id: 908471 *Max Concurrent Call Issue for Certain - ME web jobs*, [Bug url:](https://microsoftit.visualstudio.com/OneITVSO/Mktg-MKS-BP-Marketing%20Budget%20Planning/_workItems?_a=edit&id=908471) * Bug id: 961626 *Correct scripting scripting library not referred in HttpTrigger web api *, [Bug url:](https://microsoftit.visualstudio.com/OneITVSO/Mktg-MKS-BP-Marketing%20Budget%20Planning/_workItems?_a=edit&id=961626)";
            }
        }
    }
    public class Rootobject
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }
        public Column[] columns { get; set; }
        public Sortcolumn[] sortColumns { get; set; }
        public Workitem[] workItems { get; set; }
    }

    public class Column
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Sortcolumn
    {
        public Field field { get; set; }
        public bool descending { get; set; }
    }

    public class Field
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Workitem
    {
        public int id { get; set; }
        public string url { get; set; }
    }

}