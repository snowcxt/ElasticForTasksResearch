using Nest;

var client = new ElasticClient();

InitTasks();

var documents = Query();

Console.WriteLine($"query result: {documents.Count()}");

void InitTasks()
{
	var tasks = new List<DsTask>();
	var baseTime = new DateTime(2024, 01, 01);
	var random = new Random();

	for (var i = 0; i < 100; i++)
	{
		baseTime = baseTime.AddDays(random.Next(1, 50));
		tasks.Add(new DsTask
		{
			Id = Guid.NewGuid(),
			Type = (DsTaskType)(i % 3),
			DueDate = baseTime,
			Assignee = new Assignee()
			{
				UserId = i % 2 == 0 ? Guid.NewGuid() : null,
				GroupId = i % 2 == 0 ? null : Guid.NewGuid(),
			}
		});
	}

	var indexManyResponse = client.IndexMany<DsTask>(tasks, "task-index");

	if (indexManyResponse.Errors)
	{
		foreach (var itemWithError in indexManyResponse.ItemsWithErrors)
		{
			Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
		}
	}
}

IEnumerable<DsTask> Query()
{
	var searchResponse = client.Search<DsTask>(s => s
		.Index("task-index")
		.Query(q => q
			.DateRange(r => r
				.Field(f => f.DueDate)
				.GreaterThanOrEquals(new DateTime(2024, 01, 01))
				.LessThan(new DateTime(2025, 01, 01))
			)
		)
	);

	if (searchResponse.IsValid)
	{
		return searchResponse.Documents;
	}

	return null;
}

public enum DsTaskType
{
	CLM,
	IDV,
	ESign
}

public class DsTask
{
	[Keyword] public Guid Id { get; set; }

	[Keyword] public DsTaskType Type { get; set; }

	[Date] public DateTime DueDate { get; set; }
	public Assignee Assignee { get; set; }
}

public class Assignee
{
	[Keyword] public Guid? UserId { get; set; }
	[Keyword] public Guid? GroupId { get; set; }
}