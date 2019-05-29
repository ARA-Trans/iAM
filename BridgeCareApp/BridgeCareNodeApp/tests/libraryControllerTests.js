const should = require('should');
const sinon = require('sinon');
const libraryController = require('../controllers/libraryController');

describe('Library Controller Tests:', () => { 
    describe('Post', () => {
        it('should not allow an empty name on post', () => {
          const Library = function(library){this.save = () => {}};

          const req = {
              body:{
                  description: 'from mocha'
              }
          };

          const res = {
              status: sinon.spy(),
              send: sinon.spy(),
              json: sinon.spy()
          };

          const controller = libraryController(Library);
          controller.post(req,res);


          res.status.calledWith(400).should.equal(true, `Bad Status ${res.status.args}`);
          res.send.calledWith('name is required').should.equal(true);
        });
    });
}); 