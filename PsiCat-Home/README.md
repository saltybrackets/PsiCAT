# PsiCAT Home
A web interface for PsiCAT, utilizing server-side Blazor.

## Deployment
Instructions assume `dotnet` and `nginx` are installed already on a host Linux machine. For a non-nginx setup or different OS, you should consult official Blazor documentation.

Built project should be placed wherever web content is served (eg: `/var/www/html`)

### Basic nginx /etc/nginx/sites-available/default
```
server {
    listen        80;
    server_name   example.com *.example.com;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection "upgrade";
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

### Basic service /etc/systemd/psicat-home.service
```
[Unit]
Description=PsiCAT Home

[Service]
WorkingDirectory=/var/www/html
ExecStart=/usr/bin/dotnet /var/www/html/PsiCat-Home.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=psicat-home
User=<username>
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```
 - Replace `<username>` with user service should run as.
 - Be sure to chown -R <username>:<username> `/var/www/html` 
 - Install: `sudo systemctl enable psicat-home.service && sudo systemctl start psicat-home.service`
 - Verify: `sudo systemctl status psicat-home.service`
 - View logs: `sudo journalctl -fu psicat-home.service`
