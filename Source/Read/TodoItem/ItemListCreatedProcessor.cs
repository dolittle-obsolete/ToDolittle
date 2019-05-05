using System;
using System.Linq;
using System.Threading.Tasks;
using Concepts.TodoItem;
using Dolittle.Events.Processing;
using Dolittle.ReadModels;
using Events.TodoItem;

namespace Read.TodoItem
{
    public class ItemListProcessor : ICanProcessEvents
    {
        readonly IAsyncReadModelRepositoryFor<AvailableLists> _repositoryForAvailableLists;

        public ItemListProcessor(
            IAsyncReadModelRepositoryFor<AvailableLists> repositoryForAvailableLists
        )
        {
            _repositoryForAvailableLists = repositoryForAvailableLists;
        }

        [EventProcessor("eac6222e-1e7e-0fe2-2213-2715c8ebffce")]
        public void Process(ItemCreated evt)
        {
            Task.Run(async() =>
            {
                var allLists = await _repositoryForAvailableLists.GetById(default(Guid));

                if (allLists != null)
                {
                    if (!allLists.Lists.Any(list => list == evt.ListId))
                    {
                        allLists.Lists.Add(evt.ListId);
                        await _repositoryForAvailableLists.Update(allLists);
                    }
                }
                else
                {
                    allLists = new AvailableLists
                    {
                        Id = default,
                        Lists = new ListId[]
                        {
                        evt.ListId
                        }
                    };

                    await _repositoryForAvailableLists.Insert(allLists);
                }
            });
        }

        [EventProcessor("25583425-CC44-4EDD-ABA5-337418EE7FB9")]
        public void Process(ListDeleted evt)
        {
            Task.Run(async() =>
            {
                var allLists = await _repositoryForAvailableLists.GetById(default);

                if (allLists != null)
                {
                    allLists.Lists =
                        allLists
                        .Lists
                        .Where(list => list != evt.ListId)
                        .ToList();

                    await _repositoryForAvailableLists.Update(allLists);
                }
            });
        }
    }
}