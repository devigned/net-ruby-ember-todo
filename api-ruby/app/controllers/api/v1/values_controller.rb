module Api
  module V1
    class ValuesController < ApplicationController

      def index
        render json: [{value: 'value1'}, {value: 'value2'}]
      end

      def show
        render json: {value: 'value' + params['id']}
      end
    end

  end
end