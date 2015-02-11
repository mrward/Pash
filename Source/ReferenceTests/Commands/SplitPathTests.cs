﻿// Copyright (C) Pash Contributors. License: GPL/BSD. See https://github.com/Pash-Project/Pash/
using NUnit.Framework;
using System;
using System.IO;

namespace ReferenceTests.Commands
{
    [TestFixture]
    public class SplitPathTests : ReferenceTestBase
    {
        [Test]
        public void OneParentFolder()
        {
            string result = ReferenceHost.Execute(string.Format("Split-Path 'parent{0}child'", Path.DirectorySeparatorChar));

            Assert.AreEqual("parent" + Environment.NewLine, result);
        }

        [Test]
        public void OneParentFolderOutputIsStringType()
        {
            string result = ReferenceHost.Execute(string.Format("(Split-Path 'parent{0}child').GetType().Name", Path.DirectorySeparatorChar));

            Assert.AreEqual("String" + Environment.NewLine, result);
        }

        [Test]
        public void TwoParentFolders()
        {
            string result = ReferenceHost.Execute(string.Format("Split-Path parent1{0}child,parent2{0}child", Path.DirectorySeparatorChar));

            Assert.AreEqual(string.Format("parent1{0}parent2{0}", Environment.NewLine), result);
        }

        [Test]
        public void PathFromPipelineHasOneParentFolder()
        {
            string result = ReferenceHost.Execute(string.Format("'parent{0}child' | Split-Path", Path.DirectorySeparatorChar));

            Assert.AreEqual("parent" + Environment.NewLine, result);
        }

        [Test]
        public void FullFilePathIncludingDrive()
        {
            string fullPath = CreateFile(String.Empty, ".txt");
            string directory = Path.GetDirectoryName(fullPath);
            string result = ReferenceHost.Execute(string.Format("Split-Path '{0}'", fullPath));

            Assert.AreEqual(directory + Environment.NewLine, result);
        }

        [Test]
        public void Leaf()
        {
            string command = string.Format("Split-Path -Leaf parent{0}child.txt", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual("child.txt" + Environment.NewLine, result);
        }

        [Test]
        public void LeafOutputIsStringType()
        {
            string command = string.Format("(Split-Path -Leaf parent{0}child.txt).GetType().Name", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual("String" + Environment.NewLine, result);
        }

        [Test]
        public void LeafTwoItems()
        {
            string command = string.Format("Split-Path -Leaf parent1{0}child1.txt,parent2{0}child2.txt", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual(NewlineJoin("child1.txt", "child2.txt"), result);
        }

        [Test]
        public void LeafWithNoChild()
        {
            string result = ReferenceHost.Execute("Split-Path -Leaf parent");

            Assert.AreEqual("parent" + Environment.NewLine, result);
        }

        [Test]
        public void LeafOfEnvironmentDriveWithOneChild()
        {
            string result = ReferenceHost.Execute("Split-Path -Leaf env:foo");

            Assert.AreEqual("foo" + Environment.NewLine, result);
        }

        [Test]
        public void LeafOfEnvironmentDriveWithNoChild()
        {
            string result = ReferenceHost.Execute("Split-Path -Leaf env:");

            Assert.AreEqual(Environment.NewLine, result);
        }

        [Test]
        public void LeafOfEnvironmentDriveWithDirectorySeparatorAndOneChild()
        {
            string command = string.Format("Split-Path -Leaf env:{0}foo", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual("foo" + Environment.NewLine, result);
        }

        [Test]
        public void LeafForFullFilePathIncludingDrive()
        {
            string fullPath = CreateFile(String.Empty, ".txt");
            string fileName = Path.GetFileName(fullPath);
            string result = ReferenceHost.Execute(string.Format("Split-Path -Leaf '{0}'", fullPath));

            Assert.AreEqual(fileName + Environment.NewLine, result);
        }

        [Test]
        public void IsAbsoluteIsFalseForNonAbsoluteFilePath()
        {
            string result = ReferenceHost.Execute("Split-Path -IsAbsolute abc");

            Assert.AreEqual("False" + Environment.NewLine, result);
        }

        [Test]
        public void IsAbsoluteIsTrueForFullPath()
        {
            string fullPath = CreateFile(String.Empty, ".txt");
            string result = ReferenceHost.Execute(string.Format("Split-Path -IsAbsolute -Path '{0}'", fullPath));

            Assert.AreEqual("True" + Environment.NewLine, result);
        }

        [Test]
        public void BackslashIsNotAbsoluteOnPlatformsWithCorrectSeparatorAsBackslash()
        {
            string result = ReferenceHost.Execute(@"Split-Path -IsAbsolute \ ");

            bool expected = (Path.DirectorySeparatorChar != '\\');
            Assert.AreEqual(expected.ToString() + Environment.NewLine, result);
        }

        [Test]
        public void NoQualifierForEnvironmentDriveWithOneChild()
        {
            string result = ReferenceHost.Execute("Split-Path -NoQualifier env:foo");

            Assert.AreEqual("foo" + Environment.NewLine, result);
        }

        [Test]
        public void NoQualifierForEnvironmentDriveWithNoChild()
        {
            string result = ReferenceHost.Execute("Split-Path -NoQualifier env:");

            Assert.AreEqual(Environment.NewLine, result);
        }

        [Test]
        public void NoQualifierForEnvironmentDriveWithDirectorySeparatorAndOneChild()
        {
            string command = string.Format("Split-Path -NoQualifier env:{0}foo", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual(Path.DirectorySeparatorChar + "foo" + Environment.NewLine, result);
        }

        [Test]
        public void NoQualifierForDriveWithTwoSubDirectories()
        {
            string command = string.Format("Split-Path -NoQualifier C:{0}foo{0}bar", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual(String.Format("{0}foo{0}bar", Path.DirectorySeparatorChar) + Environment.NewLine, result);
        }

        [Test]
        public void QualifierForEnvironmentDriveWithOneChild()
        {
            string result = ReferenceHost.Execute("Split-Path -Qualifier env:foo");

            Assert.AreEqual("env:" + Environment.NewLine, result);
        }

        [Test]
        public void QualifierForEnvironmentDriveWithNoChild()
        {
            string result = ReferenceHost.Execute("Split-Path -Qualifier env:");

            Assert.AreEqual("env:" + Environment.NewLine, result);
        }

        [Test]
        public void QualifierForEnvironmentDriveWithDirectorySeparatorAndOneChild()
        {
            string command = string.Format("Split-Path -Qualifier env:{0}foo", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual("env:" + Environment.NewLine, result);
        }

        [Test]
        public void QualifierForDriveWithTwoSubDirectories()
        {
            string command = string.Format("Split-Path -Qualifier C:{0}foo{0}bar", Path.DirectorySeparatorChar);
            string result = ReferenceHost.Execute(command);

            Assert.AreEqual("C:" + Environment.NewLine, result);
        }

        [Test]
        public void QualifierForPathWithNoDrive()
        {
            string command = string.Format("Split-Path -Qualifier foo{0}bar", Path.DirectorySeparatorChar);
            Assert.Throws(Is.InstanceOf(typeof(Exception)), delegate
            {
                ReferenceHost.Execute(command);
            });
        }
    }
}