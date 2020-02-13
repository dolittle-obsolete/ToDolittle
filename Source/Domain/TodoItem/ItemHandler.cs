using Dolittle.Commands.Handling;
using Dolittle.Domain;

namespace Domain.TodoItem
{
    public class ItemHandler : ICanHandleCommands
    {
        readonly IAggregateOf<TodoList>  _todoList;

        public ItemHandler(
            IAggregateOf<TodoList>  todoList            
        )
        {
             _todoList =  todoList;
        }

        public void Handle(CreateItem cmd)
        {
            _todoList
                .Rehydrate(cmd.List.Value)
                .Perform(_ => _.Add(cmd.Text));
                
        }

        public void Handle(DeleteItem cmd)
        {
            _todoList
                .Rehydrate(cmd.List.Value)
                .Perform(_ => _.Remove(cmd.Text));
            // var todoList = _todoList.Get(cmd.List.Value);

            // todoList.Remove(cmd.Text);
        }

        public void Handle(MarkItemAsDone cmd)
        {
            _todoList
                .Rehydrate(cmd.List.Value)
                .Perform(_ => _.MarkAsDone(cmd.Text));
            // var todoList = _todoList.Get(cmd.List.Value);

            // todoList.MarkAsDone(cmd.Text);
        }

        public void Handle(MarkItemAsNotDone cmd)
        {
            _todoList
                .Rehydrate(cmd.List.Value)
                .Perform(_ => _.MarkAsNotDone(cmd.Text));
            // var todoList = _todoList.Get(cmd.List.Value);

            // todoList.MarkAsNotDone(cmd.Text);
        }
        
    }
}
