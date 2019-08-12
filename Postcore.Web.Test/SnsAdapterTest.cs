using Postcore.Web.Core.Interfaces;
using Postcore.Web.Helpers;
using System.Collections.Generic;
using Xunit;

namespace Postcore.Web.Test
{
    public class SnsAdapterTest
    {
        [Fact]
        public void ToListTest()
        {
            ISnsAdapter adapter = new SnsAdapter();
            var message = "[\"Giant Panda\",\"Mammal\",\"Wildlife\",\"Animal\",\"Bear\"]";
            var expected = new List<string>()
            {
                "Giant Panda", "Mammal", "Wildlife", "Animal", "Bear"
            };

            var list = adapter.ToList(message);

            Assert.Equal(expected, list);
        }
    }
}
