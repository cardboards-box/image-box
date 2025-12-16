using ImageBox.Scripting;

namespace ImageBox.Tests;

[TestClass]
public class ScriptEngineTests : TestSetup
{
	[TestMethod]
	public void TestSectionParser_HappyPath()
	{
		const string TEST_SCRIPT = @"import test from 'hello';
//This is a single line comment
import { something } from 'everything';
/*
    This is a multi-line comment
    import { comment } from 'this-is-commented-out';
*/import { howdy } from 'hello-world';
import * as test from 'hi'; //hi

console.log('This is the body of the script')";

		var scriptEngine = (ScriptEngineService)GetService<IScriptEngineService>();

		var sections = scriptEngine
			.ParseScriptSections(TEST_SCRIPT)
			.ToArray();

		Assert.IsNotNull(sections, "Sections are present");
		Assert.AreEqual(8, sections.Length, "Section lengths match");
		Assert.AreEqual("import test from 'hello';", sections[0], "First section matches");
		Assert.AreEqual("//This is a single line comment", sections[1], "Single line comment section matches");
		Assert.AreEqual("import { something } from 'everything';", sections[2], "Second section matches");
		Assert.AreEqual("/*\n    This is a multi-line comment\n    import { comment } from 'this-is-commented-out';\n*/", sections[3], "Multi-line comment section matches");
		Assert.AreEqual("import { howdy } from 'hello-world';", sections[4], "Second to last section matches");
		Assert.AreEqual("import * as test from 'hi';", sections[5], "Last import section matches");
		Assert.AreEqual("//hi", sections[6], "Last comment section matches");
		Assert.AreEqual("console.log('This is the body of the script')", sections[7], "Body section matches");
	}

	[TestMethod]
	public void TestSectionParser_HappyPathComplex()
	{
		const string TEST_SCRIPT = @"import test from 'hello';
//This is a single line comment
import { something } from 'everything';
/*
    This is a multi-line comment
    import { comment } from 'this-is-commented-out';
*/import { howdy } from 'hello-world';
import * as test from 'hi'; //hi

console.log('This is the body of the script');
//How are you today?
import { this-is-invalid } from 'hello-world';";

		var scriptEngine = (ScriptEngineService)GetService<IScriptEngineService>();

		var sections = scriptEngine
			.ParseScriptSections(TEST_SCRIPT)
			.ToArray();

		Assert.IsNotNull(sections, "Sections are present");
		Assert.AreEqual(8, sections.Length, "Section lengths match");
		Assert.AreEqual("import test from 'hello';", sections[0], "First section matches");
		Assert.AreEqual("//This is a single line comment", sections[1], "Single line comment section matches");
		Assert.AreEqual("import { something } from 'everything';", sections[2], "Second section matches");
		Assert.AreEqual("/*\n    This is a multi-line comment\n    import { comment } from 'this-is-commented-out';\n*/", sections[3], "Multi-line comment section matches");
		Assert.AreEqual("import { howdy } from 'hello-world';", sections[4], "Second to last section matches");
		Assert.AreEqual("import * as test from 'hi';", sections[5], "Last import section matches");
		Assert.AreEqual("//hi", sections[6], "Last comment section matches");
		Assert.AreEqual("console.log('This is the body of the script');\n//How are you today?\nimport { this-is-invalid } from 'hello-world';", sections[7], "Body section matches");
	}

	[TestMethod]
	public void TestSectionParser_MethodPrep_Happy()
	{
		const string TEST_SCRIPT = @"import test from 'hello';

console.log('This is the body of the script');";

		var scriptEngine = (ScriptEngineService)GetService<IScriptEngineService>();

		var settings = new ScriptEngineSettings();

		var prepMethod = scriptEngine.PrepareMainMethod(TEST_SCRIPT, "main", settings);
		Assert.IsNotNull(prepMethod, "Prepared method is not null");
		Assert.AreEqual("import test from 'hello';\n\n" +
			"export async function main() {\n" +
			"\tconsole.log('This is the body of the script');\n" +
			"}", prepMethod, "Test Method is as expected");
	}

	[TestMethod]
	public async Task TestModuleExecution_HappyPath()
	{
		const string TEST_SCRIPT = @"import { helloWorld } from 'modules';

helloWorld.Greeting = 'Hello World';";
		var output = new TestModule();
		var result = await GetService<IScriptEngineService>()
			.Execute(TEST_SCRIPT, t => t
				.AddCommonModules()
				.AddModule(output));
		Assert.IsNull(result, "Result is null - there is no return value");

		Assert.AreEqual("Hello World", output.Greeting, "Greeting property was set correctly");
	}

	[TestMethod]
	public async Task TestModuleExecution_HappyPath_Tasks()
	{
		var output = new TestModule();
		var engine = GetService<IScriptEngineService>();

		(string, Func<int>)[] tests =
		[
			("import { helloWorld } from 'modules'; helloWorld.Reset();", () => 1),
			("import { helloWorld } from 'modules'; helloWorld.Set(42)", () => 42),
			("import { helloWorld } from 'modules'; await helloWorld.DelaySet(69)", () => 69),
			("import { helloWorld } from 'modules'; await helloWorld.DelaySetReturn(96);", () => 96),
			("import { helloWorld } from 'modules'; await helloWorld.WaitRandomNumber();", () => output.RandomNumber)
		];

		for (var i = 0; i < tests.Length; i++)
		{
			var (script, expected) = tests[i];
			await engine.Execute(script, t => t
				.AddCommonModules()
				.AddModule(output));
			Assert.AreEqual(expected(), output.RandomNumber, $"RandomNumber property was set correctly for script #{i + 1}");
		}
	}

	[Module("helloWorld")]
	internal class TestModule : IScriptModule
	{
		public string Greeting { get; set; } = "hi";

		public int RandomNumber { get; set; } = 0;

		public void Reset()
		{
			RandomNumber = 1;
		}

		public void Set(int number)
		{
			RandomNumber = number;
		}

		public async Task DelaySet(int number)
		{
			await Task.Delay(1000);
			RandomNumber = number;
		}

		public async Task<int> DelaySetReturn(int number)
		{
			await DelaySet(number);
			return RandomNumber;
		}

		public async Task<int> WaitRandomNumber()
		{
			await Task.Delay(1000);
			return RandomNumber = new Random().Next(1, 100);
		}
	}
}
