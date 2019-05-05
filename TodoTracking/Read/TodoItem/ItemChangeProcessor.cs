using System.Linq;
using System.Threading.Tasks;
using Concepts.TodoItem;
using Dolittle.Events.Processing;
using Dolittle.ReadModels;
using Events.TodoItem;

namespace Read.TodoItem
{
    public class ItemChangeProcessor : ICanProcessEvents
    {
        readonly IAsyncReadModelRepositoryFor<TaskList> _repositoryForTaskList;
        private readonly ListUpdated _listUpdated;

        public ItemChangeProcessor(
            IAsyncReadModelRepositoryFor<TaskList> repositoryForTaskList,
            ListUpdated listUpdated
        )
        {
            _repositoryForTaskList = repositoryForTaskList;
            _listUpdated = listUpdated;
        }

        [EventProcessor("32dcfb33-8381-f278-daf5-40fa641f7435")]
        public void Process(ItemCreated evt)
        {
            Task.Run(async() =>
            {
                var taskList = await _repositoryForTaskList.GetById(evt.ListId);

                if (taskList == null)
                {
                    taskList = new TaskList
                    {
                    Id = evt.ListId,
                    Tasks = new []
                    {
                    new TodoTask
                    {
                    Text = evt.Text,
                    }
                    }
                    };

                    await _repositoryForTaskList.Insert(taskList);
                }
                else
                {
                    taskList.Tasks.Add(new TodoTask
                    {
                        Text = evt.Text,
                    });

                    await _repositoryForTaskList.Update(taskList);
                }

                _listUpdated(evt.ListId);
            });

        }

        [EventProcessor("BF3ACC31-6005-40B2-AFD8-05952706DC3E")]
        public void Process(ItemRemoved evt)
        {
            Task.Run(async() =>
            {
                var taskList = await _repositoryForTaskList.GetById(evt.ListId);

                var taskToRemove =
                    taskList
                    .Tasks
                    .FirstOrDefault(task => task.Text == evt.Text);

                taskList.Tasks.Remove(taskToRemove);

                await _repositoryForTaskList.Update(taskList);
                _listUpdated(evt.ListId);
            });
        }

        [EventProcessor("BF7E1935-9203-4E5A-BD05-FC65D2785866")]
        public void Process(ItemDone evt)
        {
            Task.Run(async () => 
            {
                var taskList = await _repositoryForTaskList.GetById(evt.ListId);

                var taskThatIsDone =
                    taskList
                    .Tasks
                    .FirstOrDefault(task => task.Text == evt.Text)
                    .Status = Concepts.TodoItem.TaskStatus.Done;

                await _repositoryForTaskList.Update(taskList);
                _listUpdated(evt.ListId);
            });
        }

        [EventProcessor("4696FA10-02EB-45E0-B9DC-A53D3171D257")]
        public void Process(ItemNotDone evt)
        {
            Task.Run(async () => 
            {
                var taskList = await _repositoryForTaskList.GetById(evt.ListId);

                var taskThatIsDone =
                    taskList
                    .Tasks
                    .FirstOrDefault(task => task.Text == evt.Text)
                    .Status = Concepts.TodoItem.TaskStatus.NotDone;

                await _repositoryForTaskList.Update(taskList);
                _listUpdated(evt.ListId);
            });
        }

        [EventProcessor("32CD85D7-AEDD-452F-9217-BF843BCBCF8B")]
        public void Process(ListDeleted evt)
        {
            Task.Run(async () => 
            {                
                var list = await _repositoryForTaskList.GetById(evt.ListId);

                if (list != null)
                {
                    await _repositoryForTaskList.Delete(list);;
                }
                _listUpdated(evt.ListId);
            });
        }
    }
}