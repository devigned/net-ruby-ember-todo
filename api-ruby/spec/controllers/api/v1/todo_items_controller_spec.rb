require 'rails_helper'

module Api
  module V1
    RSpec.describe TodoItemsController do

      describe 'create todo item' do
        before(:each) do
          @list = create(:todo_list)
        end

        it 'has a 201 status code' do
          post :create,
               params: {todo_item: attributes_for(:todo_item)}.merge(todo_list_id: @list.id),
               format: :json
          expect(response.status).to eq(201)
        end

        it 'stored the todo item' do
          expect { post :create,
                        params: {todo_item: attributes_for(:todo_item)}.merge(todo_list_id: @list.id),
                        format: :json }.
              to change { TodoItem.count }.by(1)
        end
      end

      describe 'delete todo item' do
        before(:each) do
          @item = create(:todo_item, todo_list: create(:todo_list))
        end

        it 'it should remove the todo item' do
          expect { delete :destroy, params: {id: @item.id, todo_list_id: @item.todo_list.id} }.
              to change { TodoItem.count }.by(-1)
        end
      end
    end
  end
end