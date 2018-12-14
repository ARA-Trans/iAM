<template>
    <v-container>
        <h1>Things to be done</h1>
        <ul v-if="todos.length">
            <li v-for="item in todos">
                {{item.description}} -
                <button v-on:click="completeItem(item)" class="btn btn-link">
                    Completed
                </button>
            </li>
        </ul>
        <p v-else class="text-success">Nothing to do right now.</p>

        <form>
            <div class="row">
                <div class="col-xs-6">
                    <input v-model="newItemDescription" class="form-control"
                           placeholder="I have to..." />
                </div>
                <button v-on:click="addItem($event)" class="btn btn-primary">
                    Add
                </button>
            </div>
        </form>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component } from 'vue-property-decorator';
    import axios from 'axios'

    axios.defaults.baseURL = process.env.VUE_APP_URL

    interface TodoItem {
        Description: string;
        Done: boolean;
        Id: string;
    }

    @Component({
    })
    export default class Todo extends Vue {
        todos: TodoItem[] = [];
        newItemDescription: string = "";

        //data() {
        //    return {
        //        todos: [],
        //        newItemDescription: null
        //    };
        //}

        mounted() {
            axios
                .get('/api/todos' )
                .then(response => (response.data as Promise<TodoItem[]>))
                .then(data => {
                    this.todos = data
                }, error => {
                    console.log(error);
                });
        }

        completeItem(item: TodoItem) {
            axios
                .delete(`/api/todos/${item.Id}`, {
            })
                .then(() => {
                    this.todos = this.todos.filter(t => t.Id != item.Id);
                })
        }

        addItem(event: any) {
            if (event) {
                event.preventDefault();
            }
            axios
                .post('api/todos', {
                    description: this.newItemDescription
                })
                .then(response => response.data as Promise<TodoItem>)
                .then((newItem) => {
                    this.todos.push(newItem);
                    this.newItemDescription = "";
                });
        }
    }
</script>

<style scoped>
</style>