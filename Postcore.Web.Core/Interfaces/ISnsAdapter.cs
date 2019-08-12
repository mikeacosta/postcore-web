using System.Collections.Generic;

namespace Postcore.Web.Core.Interfaces
{
    public interface ISnsAdapter
    {
        List<string> ToList(string message);
    }
}
