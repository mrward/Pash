using NUnit.Framework;
using System;
using System.Management.Automation;

namespace System.Management.Tests
{
    public class TestParent {
        private string _msg;
        public TestParent(string msg) { _msg = msg; }
        public override int GetHashCode() { return _msg.ToLower().GetHashCode(); }
        public override bool Equals(object obj)
        {
            var otherParent = obj as TestParent;
            if (otherParent == null)
            {
                throw new InvalidOperationException("Wrong object type to compare to");
            }
            return _msg.Equals(otherParent._msg);
        }
        public bool EqualsIgnoreCase(TestParent other)
        {
            return String.Compare(_msg, other._msg, true) == 0;
        }
    }

    public class TestChild : TestParent {
        private string _msg2;
        public TestChild(string msg, string msg2) : base(msg) { _msg2 = msg2; }
        public override int GetHashCode() { return _msg2.GetHashCode() + base.GetHashCode(); }
        public override bool Equals(object obj)
        {
            var otherChild = obj as TestChild;
            if (otherChild == null)
            {
                throw new InvalidOperationException("Wrong object type to compare to");
            }
            return _msg2.Equals(otherChild._msg2) && base.EqualsIgnoreCase(otherChild);
        }
    }

    [TestFixture]
    public class LanguagePrimitivesTests
    {

        [TestCase(3, typeof(int), (int) 3)]
        [TestCase(3, typeof(double), (double) 3)]
        [TestCase(3, typeof(string), "3")]
        [TestCase(3, typeof(int[]), new int[] { (int) 3 })]
        [TestCase(3, typeof(double[]), new double[] { (double) 3.0 })]
        [TestCase(3, typeof(string[]), new string[] { "3" })]
        [TestCase("3", typeof(string), "3")]
        [TestCase("3", typeof(int), (int) 3)]
        [TestCase("3", typeof(double), (double) 3.0)]
        [TestCase("3", typeof(string[]), new string[] { "3" })]
        [TestCase("3", typeof(int[]), new int[] { (int) 3 })]
        [TestCase("3", typeof(double[]), new double[] { (double) 3.0 })]
        public void ConvertToWorksWithBasicsAndArrayPacking(object obj, Type type, object expected)
        {
            var result = LanguagePrimitives.ConvertTo(obj, type);
            Assert.AreEqual(type, result.GetType());
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertToThrowsExceptionIfNotPossibleAtAll()
        {
            Assert.Throws(
                typeof(PSInvalidCastException),
                delegate() {
                    LanguagePrimitives.ConvertTo(new TestParent("foo"), typeof(int));
                }
            );
        }

        [Test]
        public void ConvertToThrowsExceptionIfNotPossibleToUpcast()
        {
            Assert.Throws(
                typeof(PSInvalidCastException),
                delegate() {
                    LanguagePrimitives.ConvertTo(new TestParent("foo"), typeof(TestChild));
                }
            );
        }

        [Test]
        public void ConvertToWorksForUpcast()
        {
            var result = LanguagePrimitives.ConvertTo((TestParent) new TestChild("foo", "bar"), typeof(TestChild));
            var expected = new TestChild("FOO", "bar");
            Assert.AreEqual(expected.GetType(), result.GetType());
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertToWorksForDowncast()
        {
            var result = LanguagePrimitives.ConvertTo(new TestChild("foo", "bar"), typeof(TestParent));
            var expected = new TestParent("foo");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertToCanHandleSwitchParameters()
        {
            var result = LanguagePrimitives.ConvertTo(3, typeof(SwitchParameter));
            var expected = new SwitchParameter(true);
            Assert.AreEqual(expected.GetType(), result.GetType());
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertToCanPackAsPSObject()
        {
            var result = LanguagePrimitives.ConvertTo(3, typeof(PSObject));
            var expected = PSObject.AsPSObject(3);
            Assert.AreEqual(expected.GetType(), result.GetType());
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertToCanPackAsPSObjectArray()
        {
            var result = LanguagePrimitives.ConvertTo(3, typeof(PSObject[]));
            var expected = new PSObject[] { PSObject.AsPSObject(3) };
            Assert.AreEqual(expected.GetType(), result.GetType());
            Assert.AreEqual(expected, result);
        }
    }
}

