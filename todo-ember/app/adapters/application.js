import DS from 'ember-data';

export default DS.JSONAPIAdapter.extend({
  host: 'http://localhost:3000',
  namespace: 'api/v1',
  pathForType: function(type) {
    return Ember.String.pluralize(Ember.String.underscore(type));
  }
});
