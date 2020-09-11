## Nexus Mods Monitor  

<p align="center">
   <a href="https://github.com/Aragas/NexusMods.Monitor" alt="Lines Of Code">
   <img src="https://tokei.rs/b1/github/Aragas/NexusMods.Monitor?category=code" /></a>
   <a href="https://www.codefactor.io/repository/github/aragas/nexusmods.monitor"><img src="https://www.codefactor.io/repository/github/aragas/nexusmods.monitor/badge" alt="CodeFactor" /></a>
</p>

Discord and Slack bots that monitor any changes in the NexusMods Posts/Bugs sections and reports them in subscribed channels.

Bugs | Posts
:-:|:-:
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
* ``!nmm subscribe [Game Id] [Mod Id]`` will start to monitor the desired mod and will report any changes in the current channel.
* ``!nmm unsubscribe [Game Id] [Mod Id]`` will stop monitoring in the current channel.
* ``!nmm subscriptions`` displays current subscriptions in the current channel.
* ``!nmm about``
* ``!nmm help``

### About
The repository consists of four services:
* Subscriptions - Web API based service that handles mod subscription.
* Scraper - parses NexusMods and raises events if changes are detected.
* Bot.Discord - listens for change events and posts them in Discord channels based on current subscriptions.
* Bot.Slack - listens for change events and posts them in Slack channels based on current subscriptions.

NATS is used for the Event Bus, via abstraction provided by [Enbiso.NLib.EventBus](https://github.com/enbiso/Enbiso.NLib/tree/master/Enbiso.NLib.EventBus).  
Mediatr for the command bus, directly.  
