## Nexus Mods Monitor  

Bugs | Posts
:-------------------------:|:-------------------------:
<img src="https://media.discordapp.net/attachments/751093375943573511/754076219838038046/unknown.png" alt="drawing" width="450"/> | <img src="https://media.discordapp.net/attachments/751093375943573511/754077086649548830/unknown.png" alt="drawing" width="300"/>


### How to deploy
Experience with ``docker`` and ``docker-compose`` is required.  
Go to ``src\docker-compose`` and edit the ``docker-compose.yml`` file.  
You need to add real API Keys to ``Discord__BotToken`` (for Discord to work), ``Slack__BotToken`` (for Slack to work), ``NexusMods__APIKey`` (for the site scraper to work).  
If you don't need either Discord or Slack bot, just comment the service out.  
Run ``docker-compose up -d`` as usual.  
You can check http://localhost:30080 for logs.  

### Usage
Once the bots are deployed, you have the following commands:
* ``!nmm subscribe [gameId] [modId]`` will start to monitor the desired mod and will report any changes in the current channel.
* ``!nmm unsubscribe [gameId] [modId]`` will stop monitoring.
* ``!nmm help``
* ``!nmm about``
