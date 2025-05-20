using MattEland.Demos.KernelMemory.DocumentSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Spectre.Console;

// Everything is better with a nice header
IAnsiConsole console = AnsiConsole.Console;
console.Write(new FigletText("Kernel Memory").Color(Color.Yellow));
console.MarkupLine("[cyan]Kernel Memory Document Search Demo[/]");
console.WriteLine();

// Load Settings
IConfiguration config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
	.AddEnvironmentVariables()
	.AddUserSecrets<Program>()
	.AddCommandLine(args)
	.Build();
DocSearchDemoSettings settings = config.Get<DocSearchDemoSettings>()!;

// Initialize MemoryKernel
OpenAIConfig openAiConfig = new()
{
	APIKey = settings.OpenAIKey,
	Endpoint = settings.OpenAIEndpoint,
	EmbeddingModel = settings.EmbeddingModelName,
	TextModel = settings.TextModelName,
};
IKernelMemory memory = new KernelMemoryBuilder()
	.WithOpenAI(openAiConfig)
	.Build();

console.MarkupLine("[green]KernelMemory initialized.[/]");

// Index documents and web content
console.MarkupLine("[yellow]Importing documents...[/]");

await memory.ImportTextAsync("KernelMemory allows you to import web pages, documents, and text");
await memory.ImportTextAsync("KernelMemory supports PDF, md, txt, docx, pptx, xlsx, and other formats");
await memory.ImportDocumentAsync("Facts.txt", "Repository-Facts");
await memory.ImportWebPageAsync("https://LeadingEDJE.com", "Leading-EDJE-Web-Page");
await memory.ImportWebPageAsync("https://microsoft.github.io/kernel-memory/",
	"KernelMemory-Web-Page", new TagCollection { "GitHub"});

console.MarkupLine("[green]Documents imported.[/]");

// Search API
string search = console.Ask<string>("What do you want to search for?");
console.MarkupLineInterpolated($"[yellow]Searching for '{search}'...[/]");

SearchResult results = await memory.SearchAsync(search);

Table table = new Table()
	.AddColumns("Document", "Partition", "Section", "Score", "Text");
foreach (var citation in results.Results)
{
	foreach (var part in citation.Partitions)
	{
		string snippet = part.Text;
		if (part.Text.Length > 100)
		{
			snippet = part.Text[..100] + "...";
		}

		table.AddRow(citation.DocumentId, part.PartitionNumber.ToString(), part.SectionNumber.ToString(), part.Relevance.ToString("P2"), snippet);
	}
}

table.Expand();
console.Write(table);
console.WriteLine();

// Ask API
string question = console.Ask<string>("What do you want to ask?");
console.MarkupLineInterpolated($"[yellow]Asking '{question}'...[/]");

MemoryAnswer answer = await memory.AskAsync(question);

console.WriteLine(answer.Result);
