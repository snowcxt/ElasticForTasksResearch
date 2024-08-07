using System;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

var client = new ElasticsearchClient();


// client.setDefaultHeaders(arrayOf(BasicHeader("Content-type", "application/json")));

var task1 = new DsTask
{
	Id = Guid.NewGuid(),
	Type = Type.CLM,
	DueDate =
		new DateTime(2009, 11, 15),
	Assignee = new Assignee()
	{
		UserId = Guid.NewGuid()
	}
};

var task2 = new DsTask
{
	Id = Guid.NewGuid(),
	Type = Type.ESign,
	DueDate =
		new DateTime(2024, 01, 15),
	Assignee = new Assignee()
	{
		GroupId = Guid.NewGuid()
	}
};

// await AddTask(task1);
// await AddTask(task2);

await Query();

// var response = await client.GetAsync<DsTask>("85978e60-b6e8-4f23-a919-138236d6490f", idx => idx.Index("task-index"));
//
// if (response.IsValidResponse)
// {
// 	var tweet = response.Source;
// }


async Task<string> AddTask(Task task)
{
	var response = await client.IndexAsync(task, index: "task-index");


	if (response.IsValidResponse)
	{
		Console.WriteLine($"Index document with ID {response.Id} succeeded.");
		return response.Id;
	}

	return string.Empty;
}

async Task Query()
{
	var request = new SearchRequest("task-index")
	{
		From = 0,
		Size = 10,
		Query = new TermQuery("dueDate") { Value = "2024-01-15T00:00:00" }
	};

	try
	{
		// Console.WriteLine(request.to);
		var response = await client.SearchAsync<DsTask>(request);


		if (response.IsValidResponse)
		{
			var tweet = response.Documents.FirstOrDefault();
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}
}

public enum Type
{
	CLM,
	IDV,
	ESign
}

public class DsTask
{
	[T]
	public Guid Id { get; set; }
	public Type Type { get; set; }
	public DateTime DueDate { get; set; }
	public Assignee Assignee { get; set; }
}

public class Assignee
{
	public Guid Id { get; set; }
	public Guid? UserId { get; set; }
	public Guid? GroupId { get; set; }
}