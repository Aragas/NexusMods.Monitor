## Nexus Mods Monitor  

### How to deploy
Experience with ``docker`` and ``docker-compose`` is required.  
Go to ``src\docker-compose`` and edit the ``docker-compose.yml`` file.  
You need to add real API Keys to ``Discord__BotToken`` (for Discord to work), ``Slack__BotToken`` (for Slack to work), ``NexusMods__APIKey`` (for the site scraper to work).  
If you don't need either Discord or Slack bot, just comment the service out.  
Run ``docker-compose up -d`` as usual.  
You can check http://localhost:30080 for logs.  
