﻿// Copyright (C) Pash Contributors. License: GPL/BSD. See https://github.com/Pash-Project/Pash/
using System;
using System.Linq;
using System.Management.Automation;
using NUnit.Framework;

namespace ReferenceTests.Commands
{
    [TestFixture]
    public class ClearVariableTests : ReferenceTestBase
    {
        [Test]
        public void ByName()
        {
            string result = ReferenceHost.Execute(new string[] {
                "$foo = 'bar'",
                "Clear-Variable foo",
                "$foo -eq $null"
            });

            Assert.AreEqual("True" + Environment.NewLine, result);
        }

        [Test]
        public void NameUsingNamedParametersAndAbbreviatedCommandName()
        {
            string result = ReferenceHost.Execute(new string[] {
                "$foo = 'bar'",
                "clv -name foo",
                "$foo -eq $null"
            });

            Assert.AreEqual("True" + Environment.NewLine, result);
        }

        [Test]
        public void MultipleNames()
        {
            string result = ReferenceHost.Execute(new string[] {
                "$a = 'abc'",
                "$b = 'abc'",
                "Clear-Variable a,b",
                "($a -eq $null).ToString() + \", \" + ($b -eq $null).ToString()"
            });

            Assert.AreEqual("True, True" + Environment.NewLine, result);
        }

        [Test]
        public void PassThru()
        {
            string result = ReferenceHost.Execute(new string[] {
                "$foo = 'abc'",
                "$output = Clear-Variable foo -passthru",
                "'Name=' + $output.Name + ', Value is null=' + ($output.Value -eq $null).ToString() + ', Type=' + $output.GetType().Name"
            });

            Assert.AreEqual("Name=foo, Value is null=True, Type=PSVariable" + Environment.NewLine, result);
        }

        [Test]
        public void PassThruTwoVariablesCleared()
        {
            string result = ReferenceHost.Execute(new string[] {
                "$foo = 'abc'",
                "$bar = 'abc'",
                "$v = Clear-Variable foo,bar -passthru",
                "'Names=' + $v[0].Name + ',' + $v[1].Name + ' Values are null=' + ($v[0].Value -eq $null).ToString() + ',' + ($v[1].Value -eq $null).ToString() + ' Type=' + $v.GetType().Name + ' ' + $v[0].GetType().Name"
            });

            Assert.AreEqual("Names=foo,bar Values are null=True,True Type=Object[] PSVariable" + Environment.NewLine, result);
        }

        [Test]
        public void UnknownNameCausesError()
        {
            Assert.Throws<ExecutionWithErrorsException>(delegate
            {
                ReferenceHost.Execute("Clear-Variable unknownvariable");
            });

            ErrorRecord error = ReferenceHost.GetLastRawErrorRecords().Single();
            Assert.AreEqual("Cannot find a variable with name 'unknownvariable'.", error.Exception.Message);
            Assert.AreEqual("VariableNotFound,Microsoft.PowerShell.Commands.ClearVariableCommand", error.FullyQualifiedErrorId);
            Assert.AreEqual("unknownvariable", error.TargetObject);
            Assert.IsInstanceOf<ItemNotFoundException>(error.Exception);
            Assert.AreEqual("Clear-Variable", error.CategoryInfo.Activity);
            Assert.AreEqual(ErrorCategory.ObjectNotFound, error.CategoryInfo.Category);
            Assert.AreEqual("ItemNotFoundException", error.CategoryInfo.Reason);
            Assert.AreEqual("unknownvariable", error.CategoryInfo.TargetName);
            Assert.AreEqual("String", error.CategoryInfo.TargetType);
        }

        [Test]
        public void TwoUnknownNamesCausesTwoErrors()
        {
            Assert.Throws<ExecutionWithErrorsException>(delegate
            {
                ReferenceHost.Execute("Clear-Variable unknownvariable1,unknownvariable2");
            });

            ErrorRecord error1 = ReferenceHost.GetLastRawErrorRecords().First();
            ErrorRecord error2 = ReferenceHost.GetLastRawErrorRecords().Last();
            Assert.AreEqual(2, ReferenceHost.GetLastRawErrorRecords().Count());
            Assert.AreEqual("Cannot find a variable with name 'unknownvariable1'.", error1.Exception.Message);
            Assert.AreEqual("Cannot find a variable with name 'unknownvariable2'.", error2.Exception.Message);
        }
    }
}