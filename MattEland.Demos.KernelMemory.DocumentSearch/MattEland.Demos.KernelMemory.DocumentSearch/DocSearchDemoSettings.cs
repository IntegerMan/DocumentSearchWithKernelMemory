﻿namespace MattEland.Demos.KernelMemory.DocumentSearch;

public class DocSearchDemoSettings
{
	public required string OpenAIEndpoint { get; init; }
	public required string OpenAIKey { get; init; }
	public required string TextModelName { get; init; }
	public required string EmbeddingModelName { get; init; }
}