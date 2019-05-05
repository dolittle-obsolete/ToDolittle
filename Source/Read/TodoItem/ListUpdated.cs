using System.Linq;
using System.Threading.Tasks;
using Concepts.TodoItem;
using Dolittle.Events.Processing;
using Dolittle.ReadModels;
using Events.TodoItem;

namespace Read.TodoItem
{
    public delegate void ListUpdated(ListId list);
}