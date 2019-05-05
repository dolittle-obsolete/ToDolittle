using System;
using System.Linq;
using System.Threading.Tasks;
using Concepts.TodoItem;
using Dolittle.Logging;
using Dolittle.Queries;
using Dolittle.ReadModels;

namespace Read.TodoItem
{
    public class GetTaskListByListId : IQueryFor<TaskList>
    {
        readonly IAsyncReadModelRepositoryFor<TaskList> _repositoryForTaskList;
        private readonly ILogger _logger;

        public ListId ListId { get; set; } = ListId.None;

        public GetTaskListByListId(IAsyncReadModelRepositoryFor<TaskList> repositoryForTaskList, ILogger logger)
        {
            _repositoryForTaskList = repositoryForTaskList;
            _logger = logger;
        }

        public Task<IQueryable<TaskList>> Query
        {
            get
            {
                return Task.Run(
                    async() =>
                    {
                        Console.WriteLine($"GetTaskListByListId : {ListId}");
                        if (ListId == ListId.None) return new TaskList[0].AsQueryable();

                        var result = await _repositoryForTaskList.GetById(ListId);
                        if (result == null)
                        {
                            Console.WriteLine("No tasklist found");
                            return new TaskList[0].AsQueryable();
                        }

                        return new []
                        {
                            result
                        }.AsQueryable();
                    }
                );
            }
        }
    }
}