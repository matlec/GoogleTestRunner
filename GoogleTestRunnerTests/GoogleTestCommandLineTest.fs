﻿module GoogleTestCommandLineTest
open FsUnit
open Microsoft.VisualStudio.TestTools.UnitTesting
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open GoogleTestRunner

[<TestClass>]
type ``Test GoogleTestCommandLine`` () =
    let toTestCase (a:string)=
        let tc = TestCase(a, System.Uri("http://none"), "ff.exe")
        tc

    [<TestMethod>] member x.``test arguments when running all tests`` () =
                    GoogleTestCommandLine(true, List.Empty, List.Empty, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error "

    [<TestMethod>] member x.``combines common tests in suite`` () =
                    let commonSuite = ["FooSuite.BarTest"; "FooSuite.BazTest"] |> List.map toTestCase
                    GoogleTestCommandLine(false, commonSuite, commonSuite, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error --gtest_filter=FooSuite.*:"

    [<TestMethod>] member x.``combines common tests in suite, in different order`` () =
                    let commonSuite = ["FooSuite.BarTest"; "FooSuite.BazTest"; "FooSuite.gsdfgdfgsdfg"; "FooSuite.23453452345"; "FooSuite.bxcvbxcvbxcvb"] |> List.map toTestCase
                    let commonSuiteBackwards = commonSuite |> List.rev
                    GoogleTestCommandLine(false, commonSuite, commonSuiteBackwards, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error --gtest_filter=FooSuite.*:"
                    GoogleTestCommandLine(false, commonSuiteBackwards, commonSuite, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error --gtest_filter=FooSuite.*:"

    [<TestMethod>] member x.``does not combine cases not having common suite`` () =
                    let cases = ["FooSuite.BarTest"; "BarSuite.BazTest1"] |> List.map toTestCase
                    let allCases = ["FooSuite.BarTest"; "FooSuite.BazTest"; "BarSuite.BazTest1"; "BarSuite.BazTest2"] |> List.map toTestCase
                    GoogleTestCommandLine(false, allCases, cases, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error --gtest_filter=FooSuite.BarTest:BarSuite.BazTest1"

    [<TestMethod>] member x.``does not combine cases not having common suite, in different order`` () =
                    let cases = ["BarSuite.BazTest1"; "FooSuite.BarTest"] |> List.map toTestCase
                    let allCases = ["BarSuite.BazTest1"; "FooSuite.BarTest"; "FooSuite.BazTest"; "BarSuite.BazTest2"] |> List.map toTestCase
                    GoogleTestCommandLine(false, allCases, cases, "").GetCommandLine()
                        |> should equal "--gtest_output=\"xml:\" --gtest_catch_exceptions=0 --gmock_verbose=error --gtest_filter=BarSuite.BazTest1:FooSuite.BarTest"
                