'''
Basic script to send scores to the server
'''

import requests

GAME_NAME = 'forbidden access'
GAME_KEY = None


def init():
    global GAME_KEY

    url = 'https://www.protohacks.net/LATech/477/scores/get_game_key.php'

    payload = {
        'game_name': GAME_NAME
    }

    req = requests.post(url, data=payload)

    print(req.status_code, req.text)

    if (req.status_code == 200):
        GAME_KEY = req.text

def submit_score(score: int, name: str):
    global GAME_KEY

    if (GAME_KEY is None):
        init()

    url = 'https://www.protohacks.net/LATech/477/scores/new_high_score.php'
    
    payload = {
        'game_key': GAME_KEY,
        'player_name': name,
        'player_score': score
    }

    req = requests.post(url, data=payload)

    print(req.status_code, req.text)

def reset_scores():
    global GAME_KEY

    url = 'https://www.protohacks.net/LATech/477/scores/reset_scores.php'

    payload = {
        'game_key': GAME_KEY
    }

    req = requests.post(url, data=payload)

    print(req.status_code, req.text)

reset_scores()

