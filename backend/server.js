require('dotenv').config();
const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');

const app = express();
const PORT = process.env.PORT || 3000;

app.use(cors());
app.use(express.json());

mongoose.connect(process.env.MONGODB_URI, {
  useNewUrlParser: true,
  useUnifiedTopology: true
});

const db = mongoose.connection;
db.on('error', console.error.bind(console, 'MongoDB connection error:'));
db.once('open', () => {
  console.log('Connected to MongoDB');
});

const { Player } = require('./models');

// Create a new player
app.post('/players', async (req, res) => {
  try {
    const player = new Player(req.body);
    await player.save();
    res.status(201).json(player);
  } catch (error) {
    res.status(400).json({ message: error.message });
  }
});

// Get all players
app.get('/players', async (req, res) => {
  try {
    const players = await Player.find();
    res.json(players);
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
});

// Get a specific player
app.get('/players/:id', async (req, res) => {
  try {
    const player = await Player.findById(req.params.id);
    if (!player) {
      return res.status(404).json({ message: 'Player not found' });
    }
    res.json(player);
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
});

// Update a player
app.put('/players/:id', async (req, res) => {
  try {
    const player = await Player.findByIdAndUpdate(req.params.id, req.body, { new: true });
    if (!player) {
      return res.status(404).json({ message: 'Player not found' });
    }
    res.json(player);
  } catch (error) {
    res.status(400).json({ message: error.message });
  }
});

// Delete a player
app.delete('/players/:id', async (req, res) => {
  try {
    const player = await Player.findByIdAndDelete(req.params.id);
    if (!player) {
      return res.status(404).json({ message: 'Player not found' });
    }
    res.json({ message: 'Player deleted successfully' });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
});

// Get top players (sorted by score)
app.get('/players/top/:limit', async (req, res) => {
  try {
    const limit = parseInt(req.params.limit) || 10;
    const players = await Player.find().sort({ score: -1 }).limit(limit);
    res.json(players);
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
});

// Update player stats
app.patch('/players/:id', async (req, res) => {
  try {
    const { score, level, experience, coins } = req.body;
    const updateData = {};
    if (score !== undefined) updateData.score = score;
    if (level !== undefined) updateData.level = level;
    if (experience !== undefined) updateData.experience = experience;
    if (coins !== undefined) updateData.coins = coins;

    const player = await Player.findByIdAndUpdate(
      req.params.id,
      { $set: updateData },
      { new: true }
    );
    if (!player) {
      return res.status(404).json({ message: 'Player not found' });
    }
    res.json(player);
  } catch (error) {
    res.status(400).json({ message: error.message });
  }
});

// Get player rank
app.get('/players/:id/rank', async (req, res) => {
  try {
    const player = await Player.findById(req.params.id);
    if (!player) {
      return res.status(404).json({ message: 'Player not found' });
    }
    const rank = await Player.countDocuments({ score: { $gt: player.score } }) + 1;
    res.json({ playerId: player._id, name: player.name, score: player.score, rank: rank });
  } catch (error) {
    res.status(500).json({ message: error.message });
  }
});

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});