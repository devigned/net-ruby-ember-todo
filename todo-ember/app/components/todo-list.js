import Ember from 'ember';

export default Ember.Component.extend({
  actions: {
    deleteList: function() {
      this.get('deleteList')(this.model);
    }
  }
});
