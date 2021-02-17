import spotipy
import json
from spotipy.oauth2 import SpotifyOAuth
import os

from collections import OrderedDict 
from operator import getitem 

scope = "user-library-read"
client_id = 'c0dc9a5fb71247f38acb622ffede652e'
client_secret = '3fc6abefc36f44c8830642bad617336d'
redirect_uri = 'http://localhost'

sp = spotipy.Spotify(auth_manager = SpotifyOAuth(client_id = client_id, client_secret = client_secret, redirect_uri = redirect_uri, scope = scope))

albums = {}

THIS_FOLDER = os.path.dirname(os.path.abspath(__file__))
my_file = os.path.join(THIS_FOLDER, 'data.json')

with open(my_file, 'r', encoding = 'utf-8') as fp:
    albums = json.load(fp)

def retrieve_data(albums):
    offset = 0

    while True:
        results = sp.current_user_saved_albums(limit = 50, offset = offset)

        for album in range(len(results["items"])):
            if(albums.get(results["items"][album]["album"]["id"]) == None):
                key = results["items"][album]["album"]["id"]
                albums[key] = {}
                albums[key]["Title"] = results["items"][album]["album"]["name"]
                albums[key]["Artist"] = results["items"][album]["album"]["artists"][0]["name"]
                albums[key]["Cover"] = results["items"][album]["album"]["images"][2]["url"]
                albums[key]["Added"] = results["items"][album]["added_at"]
                albums[key]["Listened to"] = False
        
        if(len(results["items"]) < 50):
            break
        
        offset += 50
        
    return OrderedDict(sorted(albums.items(), key = lambda x: getitem(x[1], 'Added'), reverse = True))

def write_json(albums):
    with open(my_file, 'w+', encoding = 'utf-8') as fp:
        json.dump(albums, fp, indent=2, ensure_ascii=False)

albums = retrieve_data(albums)
write_json(albums)