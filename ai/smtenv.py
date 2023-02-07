from gym import Env
from gym.spaces import Discrete, Box
from nextState import *
import numpy as np
import random
import pyduktape
import json

## Have object that represents environment state
## Return struct that represents the next state

class SMTState():
    def __init__(self):
        with open('./state.json', 'r') as f:
            self.state = json.load(f)

## Action selector
## Process the results
## Return the next world state

class SMTEnv(Env):
    def __init__(self):
        # Actions we can take (8 moves, 3 targets)
        self.action_space_moves = Discrete(8)
        self.action_space_targets = Discrete(3)

        self.context = pyduktape.DuktapeContext()

        ## World state + agent state

        # Resistance array
        self.observation_space = [
            Box(low=np.array([0]), high=np.array([3])),
            Box(low=np.array([0]), high=np.array([3])),
            Box(low=np.array([0]), high=np.array([3])),
        ]

        # State array (player, dem1, dem2, dem3, oppPlayer, opp1, opp2, opp3)
        # State knows EVERYTHING about the world
        self.state = SMTState().state

        # Sets game as playable
        self.in_progress = True

    def step(self, action):

        # Agent performs an action
        # Look at world state and figure out how action affects the world state
        
        performAction(action)

        # Calculate reward
        if self.in_progress == False and self.state.turn == 0:
            reward = -1000
        elif self.in_progress == False:
            reward = 1000
        else:
            reward = -1

        # Check if battle is done
        if self.in_progress == True:
            done = False
        else:
            done = True

        # Set placeholder for info
        info = {}

        # Return step information
        return self.state, reward, done, info

    def render(self):
        pass

    def reset(self):
        # Reset state
        f = open('./state.json')
        self.state = json.load(f)

        # Resets game to playable
        self.in_progress = True

        return self.state

env = SMTEnv()

episodes = 10
for episode in range(1, episodes+1):
    state = env.reset()
    done = False
    score = 0

    while not done:
        
        # Ask who's turn it is, if agent continue
        if state['turn'] == 0:
            action = [env.action_space_moves.sample(), env.action_space_targets.sample()]
            n_state, reward, done, info = env.step(action)
            score += reward
        # Else choose a random action from the opposing team 
        else:
            action = chooseRandom()
            env.step(action)
    print('Episode:{} Score:{}'.format(episode, score))



from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense
from tensorflow.keras.optimizers import Adam

states = env.observation_space.shape
actions = env.action_space.n

def build_model(states, actions):
    model = Sequential()
    model.add(Dense(24, activation='relu', input_shape=states))
    model.add(Dense(24, activation='relu'))
    model.add(Dense(actions, activation='linear'))
    return model

model = build_model(states, actions)
model.summary()

### KERAS-RL AGENT

from rl.agents import DQNAgent
from rl.policy import BoltzmannQPolicy
from rl.memory import SequentialMemory

def build_agent(model, actions):
    policy = BoltzmannQPolicy()
    memory = SequentialMemory(limit=50000, window_length=1)
    dqn = DQNAgent(model=model, memory=memory, policy=policy,
        nb_actions=actions, nb_steps_warmup=10, target_model_update=1e-2)
    return dqn

dqn = build_agent(model, actions)
dqn.compile(Adam(lr=1e-3), metrics=['mae'])
dqn.fit(env, nb_steps=50000, visualize=False, verbose=1)

scores = dqn.test(env, nb_episodes=100, visualize=False)
print(np.mean(scores.history['episode_reward']))

_ = dqn.test(env, nb_episodes=15, visualize=False)

### RELOADING AGENT FROM MEMORY

dqn.save_weights('dqn_weights.h5f', overwrite=True)

del model
del dqn
del env

env = SMTEnv()
actions = env.action_space.n
states = env.observation_space.shape[0]
model = build_model(states, actions)
dqn = build_agent(model, actions)
dqn.compile(Adam(lr=1e-3), metrics=['mae'])

dqn.load_weights('dqn_weights.h5f')

_ = dqn.test(env, nb_episodes=5, visualize=False)