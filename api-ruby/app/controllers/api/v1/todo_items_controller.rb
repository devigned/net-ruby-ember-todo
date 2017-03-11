module Api
  module V1
    class TodoItemsController < ApplicationController
      before_action :set_todo_list
      before_action :set_todo_item, except: [:create]

      def create
        render json: @todo_list.todo_items.create(todo_item_params), status: :created
      end

      def destroy
        if @todo_item.destroy
          render json: nil, status: 204
        else
          render json: @todo_item.errors, status: 400
        end
      end

      def complete
        @todo_item.update_attribute(:completed_at, Time.now)
        render json: @todo_item
      end

      private

      def set_todo_list
        @todo_list = TodoList.find(params[:todo_list_id])
      end

      def set_todo_item
        @todo_item = @todo_list.todo_items.find(params[:id])
      end

      def todo_item_params
        params[:todo_item].permit(:content)
      end
    end
  end
end
