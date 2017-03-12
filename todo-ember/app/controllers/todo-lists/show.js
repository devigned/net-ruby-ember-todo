import Ember from 'ember';

export default Ember.Controller.extend({
  actions: {
    deleteList() {
      this.model.destroyRecord().then(this.transitionToRoute('todo-lists'));
    }
  }
});
