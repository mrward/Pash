using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ReferenceTests.Language
{
    [TestFixture]
    public class FileRedirectionTests : ReferenceTestBase
    {
        private string GenerateTempFileName(string fileName = "outputfile.txt")
        {
            string fullPath = Path.Combine(Path.GetTempPath(), fileName);
            AddCleanupFile(fullPath);
            return fullPath;
        }

        [Test]
        public void OutputStreamToFile()
        {
            string fileName = GenerateTempFileName();
            ReferenceHost.Execute(new string[] {
                "$i = 10",
                "$i > " + fileName});

            Assert.AreEqual(NewlineJoin("10"), NewlineJoin(ReadLinesFromFile(fileName)));
        }

        [Test]
        public void OutputStreamToFileNameInSingleQuotes()
        {
            string fileName = GenerateTempFileName();
            ReferenceHost.Execute("'foobar' > '" + fileName + "'");

            Assert.AreEqual(NewlineJoin("foobar"), NewlineJoin(ReadLinesFromFile(fileName)));
        }

        [Test]
        public void CommandOutputToFile()
        {
            string fileName = GenerateTempFileName();
            ReferenceHost.Execute("get-command | ? { $_.name -eq 'where-object' } | % { $_.name } > " + fileName);

            Assert.AreEqual(NewlineJoin("Where-Object"), NewlineJoin(ReadLinesFromFile(fileName)));
        }
    }
}
