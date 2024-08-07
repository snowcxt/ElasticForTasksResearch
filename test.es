GET /task-index

GET /task-index/_search

GET /task-index/_search
{
    "query": {
        "constant_score": {
            "filter": {
                "term": {
                    "dueDate": "2024-01-15T00:00:00"
                }
            }
        }
    }
}
