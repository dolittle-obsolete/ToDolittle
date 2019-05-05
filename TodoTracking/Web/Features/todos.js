import { ObserverLocator, observable, computedFrom } from 'aurelia-binding';
import { TodoItem } from './todo-item';
import _ from 'underscore';
import { inject } from 'aurelia-framework';
import { CreateItem } from "./TodoItem/CreateItem";
import { CommandCoordinator } from "@dolittle/commands";
import { Guid } from "@dolittle/core";
import { QueryCoordinator } from "@dolittle/queries";
import { GetTaskListByListId } from "./TodoItem/GetTaskListByListId";
import { DeleteItem } from "./TodoItem/DeleteItem";


const STORAGE_NAME = 'todomvc-aurelia';
const ENTER_KEY = 13;

@inject(ObserverLocator, CommandCoordinator, QueryCoordinator)
export class todos {
    @observable items = [];
    @observable filter = '';
    @observable newTodoTitle = null;
    @observable areAllChecked = false;

    loading = true;
    isOffline = false;
    wasmEnabled = false;

    listId;

    @computedFrom('items', 'filter')
    get filteredItems() {
        const filter = this.filter || '!';

        switch (filter) {
            case 'active':
                return _(this.items).filter(i => !i.isCompleted);
            case 'completed':
                return _(this.items).filter(i => i.isCompleted);
            default:
                return this.items;
        }
    }


    constructor(observerLocator, commandCoordinator, queryCoordinator, storage = null) {
        this.observerLocator = observerLocator;
        this.storage = storage || localStorage;
        this.commandCoordinator = commandCoordinator;
        this.queryCoordinator = queryCoordinator;
        
        window.addEventListener('beforeinstallprompt', (e) => {
            e.prompt();
        });

        if (window._wasmEnabled === true) {
            this.wasmEnabled = true;


            navigator.serviceWorker.getRegistration().then((serviceWorker) => {
                this.isOffline = serviceWorker ? true : false;
            });

            let before = new Date();
            window._dolittleLoaded = () => {
                this.loading = false;
                let after = new Date();
                console.log(`Total time : ${after - before}`);
                this.totalLoadTime = after - before;
    
                this.loaded = false;
    
                setTimeout(() => this.#loadTaskList(), 100);
            };
    
    
            window._listUpdated = (list) => {
                this.#loadTaskList();
            };
    
        } 
    }

    activate(params) {
        this.filter = params.filter;
        // this.listId = Guid.create();
        this.listId = "9778bad6-71b3-8a63-75cf-1d72cc8fc387" //Guid.create();
        if( !this.wasmEnabled ) {
            this.#loadTaskList();
        }
    }

    bind(bindingContext, overrideContext) {
        this.load();
    }

    #loadTaskList() {
        let queryForTaskList = new GetTaskListByListId();
        queryForTaskList.listId = this.listId;

        this.queryCoordinator.execute(queryForTaskList).then(queryResult => {
            console.log(queryResult);
            if (queryResult.success) {
                if (queryResult.items.length > 0) {
                    const taskList = queryResult.items[0];
                    this.items = taskList.tasks.map(task => new TodoItem(task.text));
                    this.newTodoTitle = null;
                }
            }
        });
    }

    onKeyUp(ev) {
        if (ev.keyCode === ENTER_KEY) {
            this.addNewTodo(this.newTodoTitle);
        }
    }

    addNewTodo(title = this.newTodoTitle) {
        if (title == undefined) { return; }

        title = title.trim();
        if (title.length === 0) { return; }

        let createItem = new CreateItem();
        createItem.list = this.listId;
        createItem.text = title;

        this.commandCoordinator.handle(createItem).then(commandResult => {
            console.log(commandResult);
            if (commandResult.success && !this.wasmEnabled) {
                this.#loadTaskList();
            }
        })

    }

    observeItem(todoItem) {
        this.observerLocator
            .getObserver(todoItem, 'title')
            .subscribe((o, n) => this.onTitleChanged(todoItem));

        this.observerLocator
            .getObserver(todoItem, 'isCompleted')
            .subscribe(() => this.onIsCompletedChanged());
    }

    onTitleChanged(todoItem) {
        if (todoItem.title === '') {
            this.deleteTodo(todoItem);
            this.updateAreAllCheckedState();
        }

        this.save();
    }

    onIsCompletedChanged() {
        this.updateAreAllCheckedState();

        this.save();
    }

    deleteTodo(todoItem) {
        let command = new DeleteItem();
        command.list = this.listId;
        command.text = todoItem.title;
        this.commandCoordinator.handle(command).then(commandResult => {
            if (commandResult.success) {
                this.items = _(this.items).without(todoItem);
                this.updateAreAllCheckedState();
                this.save();
            }
        });
    }

    onToggleAllChanged() {
        this.items = _.map(this.items, item => {
            item.isCompleted = this.areAllChecked;
            return item;
        });

    }

    clearCompletedTodos() {
        this.items = _(this.items).filter(i => !i.isCompleted);
        this.areAllChecked = false;
        this.save();
    }

    get countTodosLeft() {
        return _(this.items).filter(i => !i.isCompleted).length;
    }

    updateAreAllCheckedState() {
        this.areAllChecked = _(this.items).all(i => i.isCompleted);
    }

    load() {
        const storageContent = this.storage.getItem(STORAGE_NAME);
        if (storageContent == undefined) { return; }

        const simpleItems = JSON.parse(storageContent);
        this.items = _.map(simpleItems, item => {
            const todoItem = new TodoItem(item.title);
            todoItem.isCompleted = item.completed;

            this.observeItem(todoItem);

            return todoItem;
        });
        this.updateAreAllCheckedState();
    }

    save() {
        const simpleItems = _.map(this.items, item => {
            return {
                title: item.title,
                completed: item.isCompleted
            }
        });

        this.storage.setItem(STORAGE_NAME, JSON.stringify(simpleItems));
    }

    goOffline() {
        navigator.serviceWorker.register('service-worker.js').then(() => {
            this.isOffline = true;
        });
    }

    goOnline() {
        caches.keys().then(names => {
            for (let name of names)
                caches.delete(name);
        });


        navigator.serviceWorker.getRegistrations().then(registrations => {

            for (let registration of registrations) {
                registration.unregister();
            }

            this.isOffline = false;
        });

    }
}
