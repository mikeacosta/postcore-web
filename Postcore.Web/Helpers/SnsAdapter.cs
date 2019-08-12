using Postcore.Web.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Postcore.Web.Helpers
{
    public class SnsAdapter : ISnsAdapter
    {
        public List<string> ToList(string message)
        {
            List<string> labels = new List<string>();

            try
            {
                var array = message.Split('[', ']')[1].Split(",");

                foreach(var label in array)
                    labels.Add(label.Replace("\"", ""));
            }
            catch
            {
                labels.Add($"unable to process: {message}");
            }

            return labels;
        }
    }
}