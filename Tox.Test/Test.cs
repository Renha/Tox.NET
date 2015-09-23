using NUnit.Framework;
using System;
using Tox;
using System.Net;

namespace Tox.Test
{
    [TestFixture()]
    public class ToxTest
    {
        [Test()]
        public void BasicTest()
        {
            var tox1 = new Tox(true);
            tox1.Start();

            tox1.Bootstrap(new IPEndPoint(IPAddress.Parse("178.62.250.138"), 33445), "788236D34978D1D5BD822F0A5BEBD2C53C64CC31CD3149350EE27D4D9A2F9B6B");
            tox1.Bootstrap(new IPEndPoint(IPAddress.Parse("144.76.60.215"), 33445), "04119E835DF3E78BACF0F84235B300546AF8B936F035185E2A8E9E0A67C8924F");
            tox1.Bootstrap(new IPEndPoint(IPAddress.Parse("23.226.230.47"), 33445), "A09162D68618E742FFBCA1C2C70385E6679604B2D80EA6E84AD0996A1AC8A074");
            tox1.Bootstrap(new IPEndPoint(IPAddress.Parse("192.210.149.121"), 33445), "F404ABAA1C99A9D37D61AB54898F56793E1DEF8BD46B1038B9D822E8460FAB67");

            System.Threading.Thread.Sleep(120000);
            tox1.Stop();
        }
    }
}
