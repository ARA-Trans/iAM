module.exports = {
  development: {
    db: 'mongodb://localhost:27017/BridgeCare?replicaSet=rs0',
    port: process.env.PORT || 5000
  },
  production: {
    db: 'mongodb://admin:BridgecareARA123@localhost:27017/BridgeCare?replicaSet=r1',
    port: process.env.PORT || 80
  }
}