const mongoose = require('mongoose');

const playerSchema = new mongoose.Schema({
  name: String,
  score: Number,
  level: Number,
  experience: Number,
  coins: Number,
});

const Player = mongoose.model('Player', playerSchema);

module.exports = { Player };